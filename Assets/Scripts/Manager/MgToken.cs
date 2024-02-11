using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgToken : MgGeneric<MgToken>
{
    /*��� ��ū�� ���� �����ϴ� ��
    1. �� ��ū - �� ����Ŀ�� �����ؼ� �����¿� �°� ����
    2. ĳ���� ��ū - �� ���迡 �°� ���� 
    3. �׼� ��ū - ĳ���Ϳ� �°� ����
    4. �̺�Ʈ ��ū, ��� ��ū �� �׹ۿ� ��ū�� ���� ����
    */

    //----------- �� ���� ���� ---- �� ����Ŀ�� ������Ű��, ȣ���ų�� �� �ֵ��� 
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
    private TokenTile[,] m_tileTokenes; //���� ���� Ÿ�� ��ū
    private HideTile[,] m_hideTiles;
    private List<TokenChar> m_npcTokens; //���� �ʿ� ������ npc ��ū��

    private TokenAction[] m_charActions; //ĳ���͵��� ����� �׼� ������ - ���߿� ������� ����
    private TokenAction[] m_tileActions; // Ÿ�Ͽ��� ��밡���� �������� �׼� ������

    public override void InitiSet()
    {
        base.InitiSet();

        MakeMonsterToken();
        MakeTileActionToken();
    }

    public override void ReferenceSet()
    {
        MakeMap();
        for (int i = 0; i < m_npcTokens.Count; i++)
        {
            TokenChar monsterToken = m_npcTokens[i];
            int coordX = monsterToken.GetXIndex();
            int coordY = monsterToken.GetYIndex();
            RuleBook.Migrate(monsterToken, m_tileTokenes[coordX, coordY]); //Ÿ����ū�� ĳ����ū �Ҵ�

            monsterToken.SetObjectPostion(coordX, coordY); //������Ʈ ��ġ ����ȭ
        }
        
    }

    #region �ʸ����
    public void MakeMap()
    {
        ResetMapTileObject();
        TMapBluePrint mapBluePrint = new TMapBluePrint(m_xLength, m_yLength, m_rLength, m_seed, m_noise, m_tiles, m_tileBox, m_hideTile, m_hideBox);
        m_tileMaker.MakeTopTypeMap(mapBluePrint);

        //�� ũ�⿡ ���� �巡�� �� ����
        ClickToken.SetCamRestrict();
        ClickToken.SetDragRatio(m_rLength);
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

    #region �÷��̾� ��ū ����
    public ObjectTokenBase m_playerObject;
    public void MakePlayerToken()
    {
        TokenChar playerToken = new TokenChar().MakePlayerToken();
        m_playerObject.SetToken(playerToken, TokenType.Player);
    }
    #endregion

    #region ���� ��ū ����
    public List<ObjectTokenBase> m_mosterObject;
    public void MakeMonsterToken()
    {
        m_npcTokens = new List<TokenChar>();
      
        for (int i = 0; i < 2; i++)
        {
            TokenChar monsterToken = TokenChar.MakeTestMonsterToken("ȣȣ" +i.ToString(), i);
            m_mosterObject[i].SetToken(monsterToken, TokenType.Char);
            m_npcTokens.Add(monsterToken);
            MakeTestCharActionToken(monsterToken);
            int ranX = Random.Range(0, m_xLength);
            int ranY = Random.Range(0, m_yLength);
            monsterToken.SetMapIndex(ranX, ranY); //��ǥ�� �Է� - ���� �̵��� �ȵȻ���

        }
    }
    #endregion
 
    #region �׼� ��ū ����
    public void MakeTestCharActionToken(TokenChar _tokenChar)
    {
        TokenAction moveAction = new TokenAction().MakeTestAction(ActionType.Move);
        TokenAction attackAction = new TokenAction().MakeTestAction(ActionType.Attack);
        _tokenChar.AddActionToken(moveAction);
        _tokenChar.AddActionToken(attackAction);
    }

    public void MakeTileActionToken()
    {
        Debug.Log("db Dic ã�Ƽ� ����");
        MgParsing.ParseContainer parseContainer = MgParsing.GetInstance().GetMasterData(ActionStat.MaxCountInTurn);
        List<TokenAction> tileactions = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            TokenAction tileAction = new TokenAction(parseContainer.MatchCode, parseContainer.DbValueList[i]);
            tileactions.Add(tileAction);
        }
        m_tileActions = tileactions.ToArray();
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
        //�ӽ÷� 0��°

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

