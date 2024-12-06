using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGameIniti : MgGeneric<MgGameIniti>
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
    MgInput m_inputManager;
    [SerializeField]
    PlayerManager m_playerManager;
    [SerializeField]
    SoundManager m_soundManager;
    [SerializeField]
    MgNaviPin m_naviManager;
    [SerializeField]
    MgWorkOrderPin m_workOrderManager;
    [SerializeField]
    MgUI m_playerGameUI;
    [SerializeField]
    TempSpriteBox m_tempSpriteBox;

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
        //���� �����
        initiManagerStack = new();
        //������� initi. 
        //�ʱ�ȭ �۾��� �񵿱�� ����Ǵ� ��� PlayMgInitiWorkStep�� �ش� InitiSet���� ���� ȣ��

        InputDataMGWorkStep(delegate { m_tempSpriteBox.ManageInitiSet(); PlayMgInitiWorkStep("mgSprite�ε峡"); });

        //1. ������ �Ľ�
        InputDataMGWorkStep(delegate { m_parseManager.ManageInitiSet(); });
        //2. ������ ������ ���� 
        InputDataMGWorkStep(delegate { m_MasterDataManager = new MgMasterData(); PlayMgInitiWorkStep("�⺻������ ���� ��"); });
        //3. ���� ���� ����
        InputDataMGWorkStep(delegate { m_loadManager.ManageInitiSet(); PlayMgInitiWorkStep("mg�ε峡"); });

        InputDataMGWorkStep(delegate { m_tokenManager.ManageInitiSet(); PlayMgInitiWorkStep("mg��ū��"); });

        InputDataMGWorkStep(delegate { m_gamePlayMaster.ManageInitiSet(); PlayMgInitiWorkStep("mg������"); });
        
        InputDataMGWorkStep(delegate { m_playerManager.ManageInitiSet(); PlayMgInitiWorkStep("mg�÷��̾�³�"); });

        InputDataMGWorkStep(delegate { m_soundManager.ManageInitiSet(); PlayMgInitiWorkStep("mg����³�"); });

        InputDataMGWorkStep(delegate { m_naviManager.ManageInitiSet(); PlayMgInitiWorkStep("mg�׺�³�"); });

        InputDataMGWorkStep(delegate { m_workOrderManager.ManageInitiSet(); PlayMgInitiWorkStep("mg�ɼ³�"); });

        InputDataMGWorkStep(delegate { contentManager = new MGContent(); PlayMgInitiWorkStep("������ �Ŵ��� ��"); });

        InputDataMGWorkStep(delegate { m_inputManager.ManageInitiSet(); PlayMgInitiWorkStep("mg��ǲ ��"); });

        PlayMgInitiWorkStep("������ �Ŵ��� �ʱ�ȭ ����");
    }

    private void InputDataMGWorkStep(Action _do)
    {
        initiManagerStack.Enqueue(_do);
    }

    public void PlayMgInitiWorkStep(string message)
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
        m_playerManager.ReferenceSet();
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
        DoPreUIJob();
    }
    private void InitiUIManager()
    {
        m_playerGameUI.ManageInitiSet();
        doneSetUIPart = true;
        SetGame();
    }

    private void DoPreUIJob()
    {
        m_playerGameUI.SetMapBlock();
    }

    #endregion

    private void PlayGame()
    {
        //4. �����÷��̰� �÷��� ����
        m_gamePlayMaster.GameInitialSetting();
        m_cutton.SetActive(false);
    }

    public GameObject m_cutton;
}
