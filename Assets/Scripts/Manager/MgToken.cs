using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgToken : MgGeneric<MgToken>
{
    /*모든 토큰을 생성 관리하는 곳
    1. 맵 토큰 - 맵 메이커와 연계해서 맵형태에 맞게 생성
    2. 캐릭터 토큰 - 맵 생계에 맞게 생성 
    3. 액션 토큰 - 캐릭터에 맞게 생성
    4. 이벤트 토큰, 재앙 토큰 등 그밖에 토큰들 생성 관리
    */

    //----------- 맵 생성 변수 ---- 맵 메이커로 이전시키고, 호출시킬때 값 넣도록 
    [SerializeField]
    TileMaker m_tileMaker;
    public GameObject[] m_tiles;
    public Transform m_tileBox;
    
    public Transform m_hideBox;
    public GameObject m_hideTile;
    public int m_xLength;
    public int m_yLength;
    public float m_rLength;

    public int m_seed = 0;
    public float m_noise = 0.25f;

    public struct TMapBluePrint
    {
        public int t_xLength;
        public int t_yLength;
        public float t_rLength;
        public GameObject[] t_tiles;
        public Transform t_box;
        public GameObject t_hideTile;
        public Transform t_hideBox;
        public int t_seed ;
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
    private List<TokenChar> m_npcTokens; //현재 맵에 생성된 npc 토큰들

    private TokenAction[] m_charActions; //캐릭터들이 사용할 액션 모음집 - 나중엔 딕션으로 관리
    private TokenAction[] m_tileActions; // 타일에서 사용가능한 보편적인 액션 모음집


    public override void InitiSet()
    {
        base.InitiSet();

        MakeMonsterToken();
        MakeTestTileActionToken();
    }

    public override void ReferenceSet()
    {
        MakeMap();
        for (int i = 0; i < m_npcTokens.Count; i++)
        {
            TokenChar monsterToken = m_npcTokens[i];
            int coordX = monsterToken.GetXIndex();
            int coordY = monsterToken.GetYIndex();
            RuleBook.Migrate(monsterToken, m_tileTokenes[coordX, coordY]); //타일토큰에 캐릭토큰 할당

            monsterToken.SetObjectPostion(coordX, coordY); //오브젝트 위치 동기화
        }
        
    }

    #region 맵만들기
    public void MakeMap()
    {
        ResetMapTileObject();
        TMapBluePrint mapBluePrint = new TMapBluePrint(m_xLength, m_yLength, m_rLength, m_seed, m_noise, m_tiles, m_tileBox, m_hideTile, m_hideBox);
        m_tileMaker.MakeTopTypeMap(mapBluePrint);
        ClickToken.SetCamRestrict();
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

    #region 플레이어 토큰 생성
    public ObjectTokenBase m_playerObject;
    public void MakePlayerToken()
    {
        TokenChar playerToken = new TokenChar().MakePlayerToken();
        m_playerObject.SetToken(playerToken, TokenType.Player);
    }
    #endregion

    #region 몬스터 토큰 생성
    public List<ObjectTokenBase> m_mosterObject;
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
    #endregion
    private void ParsingTileActionToken(bool _successLoad, string message)
    {
       if (_successLoad)
        {
            List<TokenAction> parseActions = new();
            string[] enterDivde = message.Split('\n'); //엔터 - 행 분리

            for (int i = 1; i < enterDivde.Length; i++) //1행부터 자료 값
            {
                string[] spaceDivde = enterDivde[i].Split('\t'); //탭 - 열 분리 
                string itemName = spaceDivde[1]; //1열이 아이템네임
                TokenAction newAction = new TokenAction().MakeTestTileAction(ActionType.Move, itemName);
                parseActions.Add(newAction);
            }

            m_tileActions = parseActions.ToArray();
        }
        else
            Debug.Log("실패");

        MgGame.GetInstance().DoneInitiDataManager("파싱 끝");
    }

    #region 액션 토큰 생성
    public void MakeTestCharActionToken(TokenChar _tokenChar)
    {
        TokenAction moveAction = new TokenAction().MakeTestAction(ActionType.Move);
        TokenAction attackAction = new TokenAction().MakeTestAction(ActionType.Attack);
        _tokenChar.AddActionToken(moveAction);
        _tokenChar.AddActionToken(attackAction);
    }

    public void MakeTestTileActionToken()
    {
        Debug.Log("시트 요청해봄");
        StartCoroutine(GameUtil.GetSheetDataCo("19xXN_chVCf-ZEsvAly-j-c69gjok0HIKYMaFcAk1Lqg", "0", ParsingTileActionToken));

    }

    public TokenAction[] GetTileActions()
    {
        return m_tileActions;
    }
    #endregion

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

    public void TempPosRandomPlayer(TokenChar _char)
    {
        int ranX = Random.Range(0, m_xLength);
        int ranY = Random.Range(0, m_yLength);
        RuleBook.Migrate(_char, m_tileTokenes[ranX, ranY]);
        _char.SetObjectPostion(ranX, ranY);
    }
}

