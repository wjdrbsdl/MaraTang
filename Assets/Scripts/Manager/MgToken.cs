using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgToken : MgGeneric<MgToken>
{
    #region 맵생성 변수
    [SerializeField]
    public TileMaker m_tileMaker;
    public GameObject m_tiles;
    public Transform m_tileBox;

    public Transform m_hideBox;
    public GameObject m_hideTile;
    public int m_xLength;
    public int m_yLength;
    public int m_chunkLength;
    public float m_rLength;

    public float m_padding = 1.05f;
    public int m_seed = 0;
    public float m_noise = 0.25f;
    
    public struct TMapBluePrint
    {
        public int t_xLength;
        public int t_yLength;
        public float t_rLength;

        public GameObject t_tile; //타일로 깔 샘플 - 필요가없음 나중에 동일 tile로 깔고, 
        public Transform t_box;
        public GameObject t_hideTile;
        public Transform t_hideBox;
        public int t_seed;
        public float t_noise;

        public TMapBluePrint(int xLength, int yLength, float rLength, int seed, float noise, GameObject tile, Transform tileBox, GameObject hideTile, Transform hideBox)
        {
            t_xLength = xLength;
            t_yLength = yLength;
            t_rLength = rLength;
            t_tile = tile;
            t_box = tileBox;
            t_seed = seed;
            t_noise = noise;
            t_hideBox = hideBox;
            t_hideTile = hideTile;
        }
    }
    #endregion

    #region 토큰들 - 마스터 토큰과 생성된 토큰 모두 관리 
    private TokenTile[,] m_tileTokenes; //현재 맵의 타일 토큰
    private HideTile[,] m_hideTiles;
    private List<TokenChar> m_charList = new(); //현재 맵에 생성된 캐릭 토큰들
    #endregion

    #region 리셋
    public override void ManageInitiSet()
    {
        base.ManageInitiSet();

        SetTileSize();
        MakeMap();
        MakePlayer();
    }

    public void LoadTileToken(TokenTile[,] _loadTile)
    {
        Debug.Log("로드한 타일로 타일 생성");
        ResetMapTileObject(); //기존 타일오브젝트 없애고 
        m_tileTokenes = _loadTile;
       
        TMapBluePrint mapBluePrint = new TMapBluePrint(m_xLength, m_yLength, m_rLength, m_seed, m_noise, m_tiles, m_tileBox, m_hideTile, m_hideBox);
        m_tileMaker.MakeTopTypeMap(mapBluePrint, m_tileTokenes);
        //맵 크기에 따라 드래그 값 조정
        CamRestrict.SetCamRestrict();
        MgInput.SetDragRatio(m_rLength);
    }

    public void LoadCharTokens(TokenChar[] _char)
    {
        ResetCharObject(); //캐릭 다 제거 하고 
        m_charList = new(); //보유캐릭리스트 리셋
        for (int i = 0; i < _char.Length; i++)
        {
            //캐릭대로 스폰?
            SpawnLoadCharactor(_char[i]); //만들어서 위치까지 
        }
    }

    private void SetTileSize()
    {
        float size = m_rLength * 0.6666f * m_padding; //1.5가 size 1에 비례
        Vector3 sizeVect = new Vector3(size, size, 1);
        m_tiles.transform.localScale = sizeVect;
        m_hideTile.transform.localScale = sizeVect;
    }

    public override void ReferenceSet()
    {
     
        for (int i = 0; i < m_charList.Count; i++)
        {
            TokenChar monsterToken = m_charList[i];
            int coordX = monsterToken.GetXIndex();
            int coordY = monsterToken.GetYIndex();
            RuleBook.Migrate(monsterToken, m_tileTokenes[coordX, coordY]); //타일토큰에 캐릭토큰 할당

            monsterToken.SetObjectPostion(coordX, coordY); //오브젝트 위치 동기화
        }
        
    }
    #endregion

    #region 맵만들기
    public void MakeMap()
    {
        ResetMapTileObject();
        TMapBluePrint mapBluePrint = new TMapBluePrint(m_xLength, m_yLength, m_rLength, m_seed, m_noise, m_tiles, m_tileBox, m_hideTile, m_hideBox);
        m_tileMaker.MakeTopTypeMap(mapBluePrint);
        //맵 크기에 따라 드래그 값 조정
        CamRestrict.SetCamRestrict();
        MgInput.SetDragRatio(m_rLength);
    }

    public void ResetMapTileObject()
    {
        if (m_tileTokenes == null)
            return;

        foreach(TokenTile objectA in m_tileTokenes)
        {
            Destroy(objectA.GetObject().gameObject);
        }
    }

    public void ResetCharObject()
    {
        foreach (TokenChar objectA in m_charList)
        {
            Destroy(objectA.GetObject().gameObject);
        }
    }

    public void SetMapTiles(TokenTile[,] _tiles)
    {
        m_tileTokenes = _tiles;
    }

    
    public void SetHideTiles(HideTile[,] _tiles)
    {
        m_hideTiles = _tiles;
    }
    #endregion

    #region 몬스터 토큰 생성
    [Header("캐릭터 생성")]
    [SerializeField] private Transform m_monsterBox;
    [SerializeField] private ObjectTokenBase m_monsterObjSample;
    [SerializeField] private ObjectTokenBase m_eventGO;
    public void MakePlayer()
    {
        int ranX = Random.Range(0, m_xLength);
        int ranY = Random.Range(0, m_yLength);
        int playerCharPid = 1;
        SpawnCharactor(new int[] { ranX, ranY }, playerCharPid, true);
    }

    //캐릭 Token에 오브젝트 씌우기 
    public void MakeCharObject(TokenChar _char)
    {
        //2. 캐릭 GO 생성
        ObjectTokenBase charObj = Instantiate(m_monsterObjSample);
        charObj.gameObject.transform.SetParent(m_monsterBox);
        charObj.gameObject.SetActive(false); //비활성

        //3. Go에 정보 Token 세팅
        charObj.SetObjectToken(_char, TokenType.Char);

        //4. 오브젝트 스프라이트 변경
        _char.SetSprite();
    }

    //캐릭 +오브젝트까지
    public TokenChar MakeCharToken(int _monsterPId)
    {
        //1. 마스터 데이터 복사로 새 캐릭 토큰 객체 생성
        TokenChar masterDataChar = MgMasterData.GetInstance().GetCharData(_monsterPId);
        TokenChar newCharToken = new TokenChar(masterDataChar);

        //2. GO 생성
        MakeCharObject(newCharToken);

        return newCharToken;
    }

    //캐릭을 장소 스폰 까지 
    public TokenChar SpawnCharactor(int[] _position, int _charPid, bool _isPlayer = false)
    {
        //스폰시킬 장소 없으면 취소 
        int[] spawnPos = GameUtil.GetPosEmptyChar(_position);
        if (spawnPos == null)
            return null;

        //좌표에 몬스터 생성
        TokenChar spawnCharactor = MakeCharToken(_charPid);
        spawnCharactor.GetObject().gameObject.SetActive(true);
        m_charList.Add(spawnCharactor); //생성된 녀석은 npc리스트에 추가; 
        spawnCharactor.m_isPlayerChar = _isPlayer;
        if(_isPlayer)
            spawnCharactor.m_Side = SideEnum.Player;
        RuleBook.FirstMigrate(spawnCharactor, _position);
        return spawnCharactor;
    }

    private void SpawnLoadCharactor(TokenChar _char)
    {
        MakeCharObject(_char);
        _char.GetObject().gameObject.SetActive(true);
        m_charList.Add(_char); //생성된 녀석은 npc리스트에 추가; 
        RuleBook.FirstMigrate(_char, _char.GetMapIndex());
      
    }
    #endregion
 
    #region 액션 토큰 생성
    public void MakeTestCharActionToken(TokenChar _tokenChar)
    {
        TokenAction moveAction = new TokenAction().MakeTestAction(ActionType.Move);
        TokenAction attackAction = new TokenAction().MakeTestAction(ActionType.Attack);
        _tokenChar.AquireAction(moveAction); 
        _tokenChar.AquireAction(attackAction);
    }
    #endregion

    #region Get
    public List<TokenChar> GetCharList()
    {
        return m_charList;
    }

    public TokenTile[,] GetMaps()
    {
        return m_tileTokenes;
    }
    public HideTile[,] GetHideMaps()
    {
        return m_hideTiles;
    }

    public TokenChar GetMainChar()
    {
        //임시로 0번째

        return m_charList[0];
    }
    #endregion

    public void RemoveCharToken(TokenChar _removeChar)
    {
        m_charList.Remove(_removeChar);
    }

}

