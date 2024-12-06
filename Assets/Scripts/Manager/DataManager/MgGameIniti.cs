using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGameIniti : MgGeneric<MgGameIniti>
{
    //인게임에서 게임의 시작(로드) 저장 흐름을 제어.

    #region 매니저
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
        
        //데이터 파트 
        if (doneSetDataPart == false)
        {
            SetDataPart();
            return;
        }
        if (doneSetUIPart == false)
        {
            //유아이 파트 
            SetUIPart();
            return;
        }
        //게임 시작
        PlayGame();
    }

    #region SetGame - 데이터 세팅 부분

    #region 데이터 세팅 진행 변수
    private bool doneDataManager = false;
    private bool doneLoadData = false;
    private bool doneInitiGameSetting = false;
    #endregion

    private void SetDataPart()
    {
        if(doneDataManager == false)
        {
            //1. 매니저 준비
            InitiDataManager();
            return;
        }
        if (doneLoadData == false)
        {
            //2. 데이터 로드
            LoadMasterData();
            return;
        }
        if (doneInitiGameSetting == false)
        {
            //3. 게임 플레이 세팅
            InitiGameSetting(); //여기선 GO영역도 들어감.
            return;
        }
        doneSetDataPart = true;
        SetGame();
    }

    #region 1. 매니저 세팅
    Queue<Action> initiManagerStack; 

    private void InitiDataManager()
    {
        //스택 만들고
        initiManagerStack = new();
        //순서대로 initi. 
        //초기화 작업이 비동기로 진행되는 경우 PlayMgInitiWorkStep을 해당 InitiSet에서 별도 호출

        InputDataMGWorkStep(delegate { m_tempSpriteBox.ManageInitiSet(); PlayMgInitiWorkStep("mgSprite로드끝"); });

        //1. 데이터 파싱
        InputDataMGWorkStep(delegate { m_parseManager.ManageInitiSet(); });
        //2. 마스터 데이터 생성 
        InputDataMGWorkStep(delegate { m_MasterDataManager = new MgMasterData(); PlayMgInitiWorkStep("기본데이터 생성 끝"); });
        //3. 이후 순서 무관
        InputDataMGWorkStep(delegate { m_loadManager.ManageInitiSet(); PlayMgInitiWorkStep("mg로드끝"); });

        InputDataMGWorkStep(delegate { m_tokenManager.ManageInitiSet(); PlayMgInitiWorkStep("mg토큰끝"); });

        InputDataMGWorkStep(delegate { m_gamePlayMaster.ManageInitiSet(); PlayMgInitiWorkStep("mg엠씨끝"); });
        
        InputDataMGWorkStep(delegate { m_playerManager.ManageInitiSet(); PlayMgInitiWorkStep("mg플레이어셋끝"); });

        InputDataMGWorkStep(delegate { m_soundManager.ManageInitiSet(); PlayMgInitiWorkStep("mg사운드셋끝"); });

        InputDataMGWorkStep(delegate { m_naviManager.ManageInitiSet(); PlayMgInitiWorkStep("mg네비셋끝"); });

        InputDataMGWorkStep(delegate { m_workOrderManager.ManageInitiSet(); PlayMgInitiWorkStep("mg핀셋끝"); });

        InputDataMGWorkStep(delegate { contentManager = new MGContent(); PlayMgInitiWorkStep("컨텐츠 매니저 끝"); });

        InputDataMGWorkStep(delegate { m_inputManager.ManageInitiSet(); PlayMgInitiWorkStep("mg인풋 끝"); });

        PlayMgInitiWorkStep("데이터 매니저 초기화 진행");
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
        //데이터 매니저 초기화 끝
        doneDataManager = true;
        SetDataPart();
    }
    #endregion

    #region 2 데이터 로드
    private void LoadMasterData()
    {
        m_loadManager.MasterDataLoad(); //토큰에 관한 마스터 데이터 로드. 
        DoneLoad();
    }
    public void DoneLoad()
    {
        doneLoadData = true;
        SetDataPart();
    }
    #endregion 

    #region 3 InitiGameSetting
    //3 게임을 위한 세팅
    private void InitiGameSetting()
    {
        m_tokenManager.ReferenceSet(); //두 클래스의 인스턴스 참조가 필요해서 나중에 해야함.
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

    #region SetGame - UI 세팅 부분
    private void SetUIPart()
    {
        //1. 매니저
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
        //4. 게임플레이가 플레이 진행
        m_gamePlayMaster.GameInitialSetting();
        m_cutton.SetActive(false);
    }

    public GameObject m_cutton;
}
