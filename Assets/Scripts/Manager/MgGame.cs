using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGame : MgGeneric<MgGame>
{
    //�ΰ��ӿ��� ������ ����(�ε�) ���� �帧�� ����.

    #region �Ŵ���
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
    CapitalObjManager m_capitalManager;

    #endregion
 
    void Start()
    {
        MakeSingleton(); 
        SetGame();
    }

    private bool doneSetDataPart = false;
    private bool doneSetUIPart = false;
    private bool donePlayGame = false;
    private void SetGame()
    {
        //������ ��Ʈ 
        if(doneSetDataPart == false)
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

    #region ������ ���� �κ�
    private bool doneDataManager = false;
    private bool doneLoadData = false;
    private bool doneInitiGameSetting = false;
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

   Queue<Action> initiManagerStack; 
    //�Ŵ����� �ʱ�ȭ - ������ ��Ʈ ���� ����. 
    private void InitiDataManager()
    {
        //���� �Ŵ��� �ʱ���ÿ� ������ �߿�ġ ����.
        initiManagerStack = new();
        
        Action mgToken = delegate { m_tokenManager.InitiSet(); };
        Action mgMc = delegate { m_gamePlayMaster.InitiSet(); DoneInitiDataManager("mg������"); } ;
        Action mgPlayer = delegate { m_playerManager.InitiSet(); DoneInitiDataManager("mg�÷��̾�³�"); };
        Action mgSound = delegate { m_soundManager.InitiSet(); DoneInitiDataManager("mg����³�"); };
        Action mgCapital = delegate { m_capitalManager.InitiSet(); DoneInitiDataManager("mg�ڿ��³�"); };
        initiManagerStack.Enqueue(mgToken);
        initiManagerStack.Enqueue(mgMc);
        initiManagerStack.Enqueue(mgPlayer);
        initiManagerStack.Enqueue(mgSound);
        initiManagerStack.Enqueue(mgCapital);

        DoneInitiDataManager("mgToken��");
    }

    public void DoneInitiDataManager(string message)
    {
        Debug.Log(message);
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

    private void InitiGameSetting()
    {
        m_tokenManager.ReferenceSet(); //�� Ŭ������ �ν��Ͻ� ������ �ʿ��ؼ� ���߿� �ؾ���.
        DoneGameSetting();
    }

    public void DoneGameSetting()
    {
        doneInitiGameSetting = true;
        SetDataPart();
    }
    #endregion

    #region UI ���� �κ� - �������� ǥ�� �� �÷��̾��� �Է¸��� ���.
    private void SetUIPart()
    {
        //1. �Ŵ���
        InitiUIManager();
    }
    private void InitiUIManager()
    {

        doneSetUIPart = true;
        SetGame();
    }


    #endregion

    private void PlayGame()
    {
        //4. �����÷��̰� �÷��� ����
        m_gamePlayMaster.FirstStart();
    }

}
