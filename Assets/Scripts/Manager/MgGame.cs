using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGame : MgGeneric<MgGame>
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
    PlayerManager m_playerManager;
    [SerializeField]
    SoundManager m_soundManager;
    [SerializeField]
    ObjTokenManager m_capitalManager;

    [SerializeField]
    UIPlayGame m_playerGameUI; 
    #endregion
 
    void Start()
    {
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
        if(doneSetDataPart == false)
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

    #region 데이터 세팅 부분
    private bool doneDataManager = false;
    private bool doneLoadData = false;
    private bool doneInitiGameSetting = false;
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

    Queue<Action> initiManagerStack; 
    private void InitiDataManager()
    {
        //아직 매니저 초기셋팅에 순서는 중요치 않음.
        initiManagerStack = new();
        Action parse = delegate { m_parseManager.InitiSet(); };
        initiManagerStack.Enqueue(parse);

        Action mgToken = delegate { m_tokenManager.InitiSet(); DoneInitiDataManager("mg토큰끝"); };
        initiManagerStack.Enqueue(mgToken);

        Action mgMc = delegate { m_gamePlayMaster.InitiSet(); DoneInitiDataManager("mg엠씨끝"); } ;
        initiManagerStack.Enqueue(mgMc);

        Action mgPlayer = delegate { m_playerManager.InitiSet(); DoneInitiDataManager("mg플레이어셋끝"); };
        initiManagerStack.Enqueue(mgPlayer);

        Action mgSound = delegate { m_soundManager.InitiSet(); DoneInitiDataManager("mg사운드셋끝"); };
        initiManagerStack.Enqueue(mgSound);

        Action mgCapital = delegate { m_capitalManager.InitiSet(); DoneInitiDataManager("mg자원셋끝"); };
        initiManagerStack.Enqueue(mgCapital);

        DoneInitiDataManager("파싱 시작");
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
        //데이터 매니저 초기화 끝
        doneDataManager = true;
        SetDataPart();
    }
   

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


    private void InitiGameSetting()
    {
        m_tokenManager.ReferenceSet(); //두 클래스의 인스턴스 참조가 필요해서 나중에 해야함.
        DoneGameSetting();
    }
    public void DoneGameSetting()
    {
        doneInitiGameSetting = true;
        SetDataPart();
    }
    #endregion

    #region UI 세팅 부분 - 데이터의 표현 및 플레이어의 입력만을 담당.
    private void SetUIPart()
    {
        //1. 매니저
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
        //4. 게임플레이가 플레이 진행
        m_gamePlayMaster.FirstStart();
        m_cutton.SetActive(false);
    }

    public GameObject m_cutton;
}
