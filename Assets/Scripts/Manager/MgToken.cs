using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgToken : MgGeneric<MgToken>
{
    #region 맵생성 변수
    [SerializeField]
    TileMaker m_tileMaker;
    public GameObject[] m_tiles;
    public Transform m_tileBox;

    public Transform m_hideBox;
    public GameObject m_hideTile;
    public int m_xLength;
    public int m_yLength;
    public float m_rLength;

    public int m_chunkLength;
    public List<Chunk> ChunkList;

    public int m_seed = 0;
    public float m_noise = 0.25f;
   
    public struct TMapBluePrint
    {
        public int t_xLength;
        public int t_yLength;
        public float t_rLength;

        public GameObject[] t_tiles; //타일로 깔 샘플 - 필요가없음 나중에 동일 tile로 깔고, 
        public Transform t_box;
        public GameObject t_hideTile;
        public Transform t_hideBox;
        public int t_seed;
        public float t_noise;

        public TMapBluePrint(int xLength, int yLength, float rLength, int seed, float noise, GameObject[] tiles, Transform tileBox, GameObject hideTile, Transform hideBox)
        {
            t_xLength = xLength;
            t_yLength = yLength;
            t_rLength = rLength;
            t_tiles = tiles;
            t_box = tileBox;
            t_seed = seed;
            t_noise = noise;
            t_hideBox = hideBox;
            t_hideTile = hideTile;
        }
    }
    private TokenTile[,] m_tileTokenes; //현재 맵의 타일 토큰
    private HideTile[,] m_hideTiles;
    #endregion

    #region 토큰들 - 마스터 토큰과 생성된 토큰 모두 관리 
    private List<TokenChar> m_npcTokens; //현재 맵에 생성된 npc 토큰들
    #endregion

    #region 리셋
    public override void InitiSet()
    {
        base.InitiSet();

        SetTileSize();
        MakeMap();
        MakeMonsterToken();
    }

    private void SetTileSize()
    {
        for (int i = 0; i < m_tiles.Length; i++)
        {
            float size = m_rLength * 0.6666f; //1.5가 size 1에 비례
            Vector3 sizeVect = new Vector3(size, size, 1);
            m_tiles[i].transform.localScale = sizeVect;
        }
    }

    public override void ReferenceSet()
    {
     
        for (int i = 0; i < m_npcTokens.Count; i++)
        {
            TokenChar monsterToken = m_npcTokens[i];
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
        List<int[]> chunkCoordList = m_tileMaker.DivideChunk(m_chunkLength);
        ChunkList = m_tileMaker.MakeChunk(chunkCoordList);
        //맵 크기에 따라 드래그 값 조정
        MgInput.SetCamRestrict();
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
    [SerializeField] private List<ObjectTokenBase> m_mosterObject;
    public void MakeMonsterToken()
    {
        m_npcTokens = new List<TokenChar>();
      
        for (int i = 0; i < 2; i++)
        {
            TokenChar monsterToken = TokenChar.MakeTestMonsterToken("호호" +i.ToString(), i);
            m_mosterObject[i].SetToken(monsterToken, TokenType.Char);
            m_npcTokens.Add(monsterToken);
            MakeTestCharActionToken(monsterToken);
            int ranX = Random.Range(0, m_xLength);
            int ranY = Random.Range(0, m_yLength);
            monsterToken.SetMapIndex(ranX, ranY); //좌표값 입력 - 실제 이동은 안된상태

        }
    }

    private TokenChar MakeMonster(int _monsterPId)
    {
        TokenChar masterDataChar = MgMasterData.g_instance.GetCharData(_monsterPId);
        TokenChar newMonToken = new TokenChar(masterDataChar);
        ObjectTokenBase monObj = Instantiate(m_monsterObjSample);
        monObj.gameObject.transform.SetParent(m_monsterBox);
        monObj.SetToken(newMonToken, TokenType.Char);
        return newMonToken;
    }

    public TokenChar SpawnMonster(int[] _position, int _monsterPid)
    {
        //좌표에 몬스터 생성
        TokenChar spawnMonster = MakeMonster(_monsterPid);
        RuleBook.FirstMigrate(spawnMonster, _position);
        return spawnMonster;
    }

    #endregion
 
    #region 액션 토큰 생성
    public void MakeTestCharActionToken(TokenChar _tokenChar)
    {
        TokenAction moveAction = new TokenAction().MakeTestAction(ActionType.Move);
        TokenAction attackAction = new TokenAction().MakeTestAction(ActionType.Attack);
        _tokenChar.AddActionToken(moveAction);
        _tokenChar.AddActionToken(attackAction);
    }
    #endregion

    #region Get
    public List<TokenChar> GetNpcPlayerList()
    {
        return m_npcTokens;
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

        return m_npcTokens[0];
    }
    #endregion

    public void TempPosRandomPlayer(TokenChar _char)
    {
        int ranX = Random.Range(0, m_xLength);
        int ranY = Random.Range(0, m_yLength);
        RuleBook.Migrate(_char, m_tileTokenes[ranX, ranY]);
        _char.SetObjectPostion(ranX, ranY);
    }
}

