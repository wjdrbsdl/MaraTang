using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgToken : MgGeneric<MgToken>
{
    #region �ʻ��� ����
    [SerializeField]
    public TileMaker m_tileMaker;
    public GameObject m_tiles;
    public Sprite[] m_tilesSprite;
    public Sprite[] m_hideSprite;
    public Sprite[] m_charSprite;
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

        public GameObject t_tile; //Ÿ�Ϸ� �� ���� - �ʿ䰡���� ���߿� ���� tile�� ���, 
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
    private TokenTile[,] m_tileTokenes; //���� ���� Ÿ�� ��ū
    private HideTile[,] m_hideTiles;
    #endregion

    #region ��ū�� - ������ ��ū�� ������ ��ū ��� ���� 
    private List<TokenChar> m_charList = new(); //���� �ʿ� ������ ĳ�� ��ū��
    #endregion

    #region ����
    public override void InitiSet()
    {
        base.InitiSet();

        SetTileSize();
        MakeMap();
        MakePlayer();
    }

    private void SetTileSize()
    {
        float size = m_rLength * 0.6666f * m_padding; //1.5�� size 1�� ���
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
            RuleBook.Migrate(monsterToken, m_tileTokenes[coordX, coordY]); //Ÿ����ū�� ĳ����ū �Ҵ�

            monsterToken.SetObjectPostion(coordX, coordY); //������Ʈ ��ġ ����ȭ
        }
        
    }
    #endregion

    #region �ʸ����
    public void MakeMap()
    {
        ResetMapTileObject();
        TMapBluePrint mapBluePrint = new TMapBluePrint(m_xLength, m_yLength, m_rLength, m_seed, m_noise, m_tiles, m_tileBox, m_hideTile, m_hideBox);
        m_tileMaker.MakeTopTypeMap(mapBluePrint);
        //�� ũ�⿡ ���� �巡�� �� ����
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

    public void SetMapTiles(TokenTile[,] _tiles)
    {
        m_tileTokenes = _tiles;
    }

    
    public void SetHideTiles(HideTile[,] _tiles)
    {
        m_hideTiles = _tiles;
    }
    #endregion

    #region ���� ��ū ����
    [Header("ĳ���� ����")]
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

    private TokenChar MakeCharToken(int _monsterPId)
    {
        //1. ������ ������ ����� �� ĳ�� ��ū ��ü ����
        TokenChar masterDataChar = MgMasterData.GetInstance().GetCharData(_monsterPId);
        TokenChar newCharToken = new TokenChar(masterDataChar);

        //2. ĳ�� GO ����
        ObjectTokenBase charObj = Instantiate(m_monsterObjSample);
        charObj.gameObject.transform.SetParent(m_monsterBox);

        //3. Go�� ���� Token ����
        charObj.SetObjectToken(newCharToken, TokenType.Char);

        //4. ������Ʈ ��������Ʈ ����
        newCharToken.SetSprite();
        m_charList.Add(newCharToken); //������ �༮�� npc����Ʈ�� �߰�; 
        return newCharToken;
    }

    public TokenChar SpawnCharactor(int[] _position, int _charPid, bool _isPlayer = false)
    {
        //��ǥ�� ���� ����
        TokenChar spawnCharactor = MakeCharToken(_charPid);
        spawnCharactor.m_isPlayerChar = _isPlayer;
        RuleBook.FirstMigrate(spawnCharactor, _position);
        return spawnCharactor;
    }

    public TokenEvent SpawnEvent(TokenTile _tile, int _eventPid)
    {
        //���� ���� �������� - ������ ����
        //TokenEvent spawnEvent = MgMasterData.GetInstance()
        TokenEvent masterEvent = new TokenEvent(); //�ӽ÷� 
        TokenEvent spawnEventToken = new TokenEvent(masterEvent);
        ObjectTokenBase eventObj = Instantiate(m_eventGO);
        eventObj.SetObjectToken(spawnEventToken, TokenType.Event);
        spawnEventToken.SetMapIndex(_tile.GetXIndex(), _tile.GetYIndex());
        eventObj.SyncObjectPosition();
        _tile.SetEnteraceEvent(spawnEventToken);
        return spawnEventToken;
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
    #endregion

    #region Get
    public List<TokenChar> GetNpcPlayerList()
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
        //�ӽ÷� 0��°

        return m_charList[0];
    }
    #endregion

    public void RemoveCharToken(TokenChar _removeChar)
    {
        m_charList.Remove(_removeChar);
    }

    public void TempPosRandomPlayer(TokenChar _char)
    {
        int ranX = Random.Range(0, m_xLength);
        int ranY = Random.Range(0, m_yLength);
        RuleBook.Migrate(_char, m_tileTokenes[ranX, ranY]);
        _char.SetObjectPostion(ranX, ranY);
    }
}

