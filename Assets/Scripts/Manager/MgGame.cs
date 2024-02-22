using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGame : MgGeneric<MgGame>
{
    //�ΰ��ӿ��� ������ ����(�ε�) ���� �帧�� ����.

    #region �Ŵ���
    [SerializeField]
    MgParsing m_parseManager;
    [SerializeField]
    MgToken m_tokenManager;
    [SerializeField]
    MgGameLoader m_loadManager;
    [SerializeField]
    GamePlayMaster m_gamePlayMaster;
    [SerializeField]
    PlayerManager m_playerManager;
    [SerializeField]
    SoundManager m_soundManager;
    [SerializeField]
    ObjTokenManager m_capitalManager;
    
    [SerializeField]
    MgUI m_playerGameUI;

    MGContent contentManager;
    MgMasterData m_MasterDataManager;
    #endregion

    void Start()
    {
        m_cutton.SetActive(true);
        doneSetDataPart = false;
        doneSetUIPart = false;
        MakeSingleton(); 
        SetGame();
    }

    private bool doneSetDataPart = false;
    private bool doneSetUIPart = false;
    
    private void SetGame()
    {
        
        //������ ��Ʈ 
        if (doneSetDataPart == false)
        {
            SetDataPart();
            return;
        }
        if (doneSetUIPart == false)
        {
            //������ ��Ʈ 
            SetUIPart();
            return;
        }
        //���� ����
        PlayGame();
    }

    #region SetGame - ������ ���� �κ�

    #region ������ ���� ���� ����
    private bool doneDataManager = false;
    private bool doneLoadData = false;
    private bool doneInitiGameSetting = false;
    #endregion

    private void SetDataPart()
    {
        if(doneDataManager == false)
        {
            //1. �Ŵ��� �غ�
            InitiDataManager();
            return;
        }
        if (doneLoadData == false)
        {
            //2. ������ �ε�
            LoadMasterData();
            return;
        }
        if (doneInitiGameSetting == false)
        {
            //3. ���� �÷��� ����
            InitiGameSetting(); //���⼱ GO������ ��.
            return;
        }
        doneSetDataPart = true;
        SetGame();
    }

    #region 1. �Ŵ��� ����
    Queue<Action> initiManagerStack; 
    private void InitiDataManager()
    {
        //
        initiManagerStack = new();

        //1. �Ľ�
        Action parse = delegate { m_parseManager.InitiSet(); };
        initiManagerStack.Enqueue(parse);
        //2. ������ ���� 
        Action makeMasterData = delegate { m_MasterDataManager = new MgMasterData(); DoneInitiDataManager("�⺻������ ���� ��"); };
        initiManagerStack.Enqueue(makeMasterData);
        //3. ���� ���� ����
        Action mgToken = delegate { m_tokenManager.InitiSet(); DoneInitiDataManager("mg��ū��"); };
        initiManagerStack.Enqueue(mgToken);

        Action mgMc = delegate { m_gamePlayMaster.InitiSet(); DoneInitiDataManager("mg������"); } ;
        initiManagerStack.Enqueue(mgMc);

        Action mgPlayer = delegate { m_playerManager.InitiSet(); DoneInitiDataManager("mg�÷��̾�³�"); };
        initiManagerStack.Enqueue(mgPlayer);

        Action mgSound = delegate { m_soundManager.InitiSet(); DoneInitiDataManager("mg����³�"); };
        initiManagerStack.Enqueue(mgSound);

        Action mgCapital = delegate { m_capitalManager.InitiSet(); DoneInitiDataManager("mg�ڿ��³�"); };
        initiManagerStack.Enqueue(mgCapital);

        contentManager = new MGContent(); //�ν��Ͻ�ȭ

        DoneInitiDataManager("�Ľ� ����");
    }
    public void DoneInitiDataManager(string message)
    {
      //  Debug.Log(message);
        if (initiManagerStack.Count >= 1)
        {
            Action nextIniti = initiManagerStack.Dequeue();
            nextIniti();
            return;
        }
        //������ �Ŵ��� �ʱ�ȭ ��
        doneDataManager = true;
        SetDataPart();
    }
    #endregion

    #region 2 ������ �ε�
    private void LoadMasterData()
    {
        m_loadManager.MasterDataLoad(); //��ū�� ���� ������ ������ �ε�. 
        DoneLoad();
    }
    public void DoneLoad()
    {
        doneLoadData = true;
        SetDataPart();
    }
    #endregion 

    #region 3 InitiGameSetting
    //3 ������ ���� ����
    private void InitiGameSetting()
    {
        m_tokenManager.ReferenceSet(); //�� Ŭ������ �ν��Ͻ� ������ �ʿ��ؼ� ���߿� �ؾ���.
        contentManager.ReferenceSet();
        DoneGameSetting();
    }
    public void DoneGameSetting()
    {
        doneInitiGameSetting = true;
        SetDataPart();
    }
    #endregion

    #endregion

    #region SetGame - UI ���� �κ�
    private void SetUIPart()
    {
        //1. �Ŵ���
        InitiUIManager();
    }
    private void InitiUIManager()
    {
        m_playerGameUI.InitiSet();
       doneSetUIPart = true;
        SetGame();
    }


    #endregion

    private void PlayGame()
    {
        //4. �����÷��̰� �÷��� ����
        m_gamePlayMaster.FirstStart();
        m_cutton.SetActive(false);
    }

    public GameObject m_cutton;
}
