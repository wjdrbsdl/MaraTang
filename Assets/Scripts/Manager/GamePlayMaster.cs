using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayMaster : MgGeneric<GamePlayMaster>
{
    public MagnetItem testMangetSample;
    public NaviPin testNaviPin;
    public int testBlessPid;
    #region  변수
    public enum PlayerMember
    {
        LivePlayer, AI
    }

    public float m_playSpeed = 1; //게임 애니메이션의 속도 기본 1
    public float m_moveSpeed = 0.5f;
    public const float c_movePrecision = 0.1f; //움직임 정밀도
    public bool m_autoReportCheck = true;
    public bool m_testCheckPlayerInventory = false; //플레이어 인벤 체크하느냐 
    public bool m_testAuto = true;
    public bool m_cheatWorkFree = true; //작업시 자원 필요여부 
    public bool m_actChunkContent = false;

    private GamePlayData m_playData = new();

    [SerializeField]
    private CameraFollow m_camFollow;
    private PlayerMember m_playerMemeber = PlayerMember.LivePlayer; // 0번 플레이어, 1번 AI
    private PlayerRule[] m_players;
    private AIPlayer m_aiPlayer;
    public RuleBook RuleBook;
    public EmphasizeObject EmphasizeTool;
    public List<WorkOrder> m_globalWorkList = new();
    public List<Complain> m_globalComplainList = new();
    public int tempDevilBirthrestTurm = 3; //발생주기 세트

    //보고 단계에서 저장불러오기를 위해 필요한 변수들
    public NationManageStepEnum curNationStep = NationManageStepEnum.FinishPreTurn;
    public int curNationNum = -1;

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            //m_playData.PlayTime = 0;
            //m_playerMemeber = 0;
            //NoticeTurnPlayer();

            //TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
            //EquiptItem equipt = new EquiptItem( MgMasterData.GetInstance().GetEquiptData(1));
            //mainChar.AquireEquipt(equipt);

            // ReadyNextTurn(); //강제 턴 리셋

            // UnityEngine.SceneManagement.SceneManager.LoadScene("0.IntroScene");
            //TOrderItem deathCode = new TOrderItem(TokenType.Content, (int)ContentEnum.GameState, (int)GameStateEnum.PlayerDead);
            //MGContent.GetInstance().SendActionCode(deathCode);

            GodBless bless = new GodBless(MgMasterData.GetInstance().GetGodBless(testBlessPid));
            PlayerManager.GetInstance().GetMainChar().AquireBless(bless);

            //TokenBuff armorBreakbuff = new TokenBuff( MgMasterData.GetInstance().GetBuffData((int)BuffEnum.ArmorBreak));
            //PlayerManager.GetInstance().GetMainChar().CastBuff(armorBreakbuff);
            //TokenBuff secondBuff = new TokenBuff(MgMasterData.GetInstance().GetBuffData((int)BuffEnum.Fracture));
            //PlayerManager.GetInstance().GetMainChar().CastBuff(secondBuff);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            //MgToken.GetInstance().MakeMap(); // 맵 다시만들기
            //FirstStart();
            //  MGContent.GetInstance().m_devilIncubator.SetRestBrithTurn(tempDevilBirthrestTurm);
            GodBless bless = new GodBless(MgMasterData.GetInstance().GetGodBless(testBlessPid));
            PlayerManager.GetInstance().GetMainChar().RemoveBless(bless);
            //kmp테스트
            //KMP kmpTest = new KMP();
            //kmpTest.Search();

            //근처 동굴 생성
            //OrderExcutor excutor = new();
            //TOrderItem item = new TOrderItem(TokenType.Tile, 1, 1);
            //excutor.BuildPlace(1, 1);

            //GameSaver saver = new GameSaver();
            //saver.SaveGame();

            MgUI.GetInstance().ShowPlayerEquipt();

        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            GameLoader loader = new GameLoader();
            loader.LoadGame();
            //  MgUI.GetInstance().ShowQuestList();
            //int randomSkill = UnityEngine.Random.Range(1, 3);
            //Debug.Log(randomSkill + "스킬 습득");
            //PlayerManager.GetInstance().StudyPlayerAction(randomSkill);

        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            //Debug.Log("플레이어 턴 종료 선언");
            PlayerManager.GetInstance().EndTurn();
        }
    }

    #region 초기화 및 시작
    public override void ManageInitiSet()
    {
        base.ManageInitiSet();
        RuleBook = new();
        RuleBook.m_PlayMaster = this;
        Announcer play = new(); // 아나운서 싱글톤생성
        m_aiPlayer = new();
        m_players = new PlayerRule[2];
        EmphasizeTool = new();
    }

    public void GameInitialSetting()
    {
      //  Debug.Log("처음 시작");
        m_aiPlayer.SetInitial(); //mgtoken이 토큰 다 만들고 나서 진행해야함.
        m_playerMemeber = PlayerMember.LivePlayer; //시작은 플레이어
        //플레이어 할당
        m_players[0] = PlayerManager.GetInstance();
        m_players[1] = m_aiPlayer;
        //세계의 시작만 세팅 
        ReadyNextWorldTurn(); //월드 턴 변화 진행
        //처음 시작시 플레이어 메인캐릭터에 카메라 포커스.
        CamFocusMainChar();
        PlayerManager.GetInstance().FirstStart();
    
    }
    #endregion
    
    #region 국가 턴 수행
    public void ReportNationStep(Nation _nation, NationManageStepEnum _step)
    {
        curNationStep = _step;
        curNationNum = _nation.GetNationNum();
        if (m_autoReportCheck)
        {
            //  Debug.LogWarning("보고 자동확인으로 플레이어 체크없이 진행");
            _nation.DoneJob(_step);
        }
        else
        {
            MgUI.GetInstance().ShowNationReport(_nation, _step);
        }
    }

    #endregion

    #region 캐릭터 턴 수행
    //게임마스터는 플레이어들이 턴종료때까지 대기, 수행할 캐릭터 액션토큰을 정해서 요구하면 해당 액션을 수행해주는 역할 

    //1. 해당 차례 플레이어에게 액션 수행하라고 전달
    public bool TracePlayChar = false;
    public void NoticeTurnPlayer()
    {
        //플레이어 여러명이 있다고 가정하고 순서대로 진행 - 현재는 1.나 2.AI 로 진행 
        int turn = (int)m_playerMemeber;
        m_players[turn].PlayTurn();

        //해당 플레이턴이 될때 카메라 고정을 풀거나, 하도록
        if(TracePlayChar == true)
        CamFocus(MgToken.GetInstance().GetCharList()[turn]);

    }

    //2. 전달 받은 캐릭과 액션을 수행 해줌
    public void PlayCharAction(TokenChar _charToken)
    {
        //캐릭터 토큰이 준비된대로 룰북가지고 액션을 수행
        ResetEmphasize();
        _charToken.ShowAction(true);
        RuleBook.ReadCharAction(_charToken); //룰북에 액션 수행파트 읽기
    }

    //3. TokenObject 애니메이션 수행
    private IEnumerator m_tokenActionCoroutine;
    private Action m_actionEffect;
    [SerializeField] private bool m_isPlayAnimate = true;
    public void AnimateTokenObject(IEnumerator _aniCoroutine, Action _actionEffect, TokenChar _playChar)
    {
        //진행중이던 코루틴이 있다면 중지
        StopAnimateTokenObject();

        m_actionEffect = _actionEffect;
        m_tokenActionCoroutine = _aniCoroutine;
        //코루틴이 있으면 수행하고
        if (m_tokenActionCoroutine != null && m_isPlayAnimate)
        {
            StartCoroutine(m_tokenActionCoroutine);
            return;
        }
        //없으면 효과라도 있으면 수행
        if (m_actionEffect != null)
            m_actionEffect();

        //코루틴이 아닌 여기서 액션 끝났음을 수행
        DoneCharAction(_playChar);
    }

    //4. 액션 수행 끝났으면 해당 플레이어게 해당 캐릭터 액션수행 끝났음을 알림. 
    public void DoneCharAction(TokenChar _charToken)
    {
        //   Debug.Log("<<color=red>마스터 :  " + _charToken.num + "번 더 수행할 행동력이 있나?</color>");
        ResetAnimateValue();
        CamTraceOff();
        _charToken.SetState(CharState.Idle);
        _charToken.ShowAction(false);
        m_players[(int)m_playerMemeber].DoneCharAction(_charToken); //현재 플레이어에게 해당 캐릭터의 액션이 완료되었음을 알린다.
        OccurMoveEvent(_charToken);
    }

    //5. 해당 플레이어가 턴을 종료하면, 다음 차례 플레이어를 뽑거나 턴정산을 진행
    public void EndPlayerTurn()
    {
        CharTurnEnd(m_playerMemeber);
        int cur = (int)m_playerMemeber;
        cur += 1; //다음차례

        if ((int)PlayerMember.AI < cur)
        {
            //만약 ai 턴이 끝난것이라면
            DoneStep(GamePlayStep.CharTurn); //EndPlayTurn에서 모든 캐릭 턴 끝나면 호출
            return;
        }

        //그게 아니라면
        m_playerMemeber = (PlayerMember)cur; //현 플레이어 멤버 바꾸고
        NoticeTurnPlayer();//다음 차례 플레이어를 호출 
    }

    private void CharTurnEnd(PlayerMember _member)
    {
        List<TokenChar> charlist = MgToken.GetInstance().GetCharList();
        bool isPlayer = _member == PlayerMember.LivePlayer;
        for (int i = 0; i < charlist.Count; i++)
        {
            if (charlist[i].IsPlayerChar() == isPlayer)
                charlist[i].TurnReset();
        }
    }

    public void StopAnimateTokenObject()
    {
        //코루틴 진행중인거 멈추기
        if (m_tokenActionCoroutine != null)
        {
            //코루틴 중단
            StopCoroutine(m_tokenActionCoroutine);
            //효과 바로 적용
            if (m_actionEffect != null)
                m_actionEffect();
        }
        ResetAnimateValue();
    }

    private void ResetAnimateValue()
    {
        m_tokenActionCoroutine = null;
        m_actionEffect = null;
    }

    #endregion
 
    #region 이동 후 처리

    private void OccurMoveEvent(TokenChar _charToken)
    {
        if (IsPlayerMoveDone(_charToken) == false)
            return;
        //만약 플레이어 캐릭터가 이동한거라면 해당 위치에서 다시 안개 설정
        FogContorl(_charToken);

        SearchTile findTile = new();
        //1. 채집 진행
        findTile.FindSomething(GameUtil.GetTileTokenFromMap(_charToken.GetMapIndex()), _charToken);

    }

    private bool IsPlayerMoveDone(TokenChar _charToken)
    {
        if (_charToken.isMainChar == false)
            return false;
        if (_charToken.GetNextActionToken().GetActionType().Equals(ActionType.Move) == false)
            return false;

        return true;
    }

    #endregion

    #region 턴 정산
  
    private void ReadyNextTurn()
    {
        PopupMessage("턴 종료"); 
        EffectEndTurn(); //턴이 종료할때 발휘되는 효과나 계산할것들 정산
        ReadyNextWorldTurn(); //월드 턴 변화 진행
        ResetNationTurn(); //국가 집행순서 세팅
        ResetPlayerTurn(); //플레이어 턴으로 세팅
        ResetPlayDataUI(); //플레이 데이터 갱신
        EffectStartTurn(); //턴 시작시 발휘되는 효과 적용
        DoneStep(GamePlayStep.ReadyNextTurn); //ReadyNextTurn()에서 턴 정산 끝나면 호출 
     
    }

   

    private void EffectEndTurn()
    {
        m_playerMemeber = 0;
        DoWorkList();
        CountComplainTurn();
      //  Debug.Log("겜마스터 완료 제거 후 남은 작업수 " + m_globalWorkList.Count);
    }

    private void ReadyNextWorldTurn()
    {
        AnnounceState("세계턴 변화 따짐 내용 없음");
        m_playData.PlayTime += 1; //여태 진행한 턴
        //컨텐츠 에서 무언가 발생 
        MGContent.g_instance.WriteContentWhenNextTurn();
    }

    private void ResetNationTurn()
    {
        MgNation.GetInstance().ResetNationNum();
    }

    private void ResetPlayerTurn()
    {
        //AI 턴까지 끝나면 다시 리얼플레이어 턴으로 초기화 해서 시작 
        m_playerMemeber = PlayerMember.LivePlayer; //처음 유저로 플레이어 코드 변경. 
    }

    private void ResetPlayDataUI()
    {
        AnnounceState("플레이 데이터 갱신");
        PlayerManager.GetInstance().OnChangePlayData();
    }

    private void EffectStartTurn()
    {
        MGContent.g_instance.m_devilIncubator.BirthDevil();
    }

    #endregion

    #region 턴 시작
    public float startTermTime = 0.3f;
    
     public void DoneStep(GamePlayStep _step)
    {
        switch(_step)
        {
            case GamePlayStep.GameInitialSetting:
                StartCharTurn(); //플레이 턴 부터 시작. 
                break;
            case GamePlayStep.ReadyNextTurn:
                //다음차례 준비 끝나면 국가 부터 시작
                StartNationTurn();
                break;
            case GamePlayStep.NationTurn:
                StartCharTurn();
                break;
            case GamePlayStep.CharTurn:
                SettleNationPolicy();
                break;
            case GamePlayStep.NationSettle:
                ReadyNextTurn();
                break;
        }
    }

    private void StartNationTurn()
    {
        //국가 매니저를 통해 국가 행동을 진행
        MgNation.GetInstance().ManageNationTurn();
        //국가 매니저에서 국가 행동이 끝나면 겜플레이 마스터에게 국가 행동이 끝났음을 콜백?
    }

    private void StartCharTurn()
    {
        Invoke(nameof(NoticeTurnPlayer), startTermTime); //캐릭터 액션 턴 시작
    }

    private void SettleNationPolicy()
    {
        //국가 정책 정산 
        MgNation.GetInstance().SettleNationEndTurn();
    }

    #endregion

    #region 부가 편의 기능
    public void FogContorl(TokenBase _targetPos, int _sight = 2)
    {
        //일단 안개 걷는 기능만
        List<TokenTile> tiles = GameUtil.GetTileTokenListInRange(_sight, _targetPos.GetMapIndex());
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].ChangeViewState(TileViewState.Sight);
        }
    }

    #region 오브젝트 강조
    public void EmphasizeTarget(TokenBase _token)
    {
        EmphasizeTool.Emphasize(_token);
    }

    public void EmphasizeTargetTileObject(TokenChar _char, TokenAction _actionToken)
    {
        //토큰오브젝트를 강조하는 부분 (UI 부분은 UIPlayGame 에서 관리)
        List<ObjectTokenBase> tileObjList = GameUtil.GetTileObjectInRange(_actionToken.GetFinalRange(_char), _char.GetXIndex(), _char.GetYIndex(), _actionToken.GetStat(CharActionStat.MinLich));
        EmphasizeTool.Emphasize(tileObjList);
    }

    public void ResetEmphasize()
    {
        EmphasizeTool.ResetEmphasize(); //선택시 필요했던 강조부분을 초기화 
    }
    #endregion

    #region 카메라 조정
    public void CamFocusMainChar()
    {
        CamFocus(PlayerManager.GetInstance().GetMainChar());
    }

    public void CamFocus(TokenChar _char)
    {
        //해당 으로 포커스
        m_camFollow.FocusTarget(_char.GetObject());
    }

    public void CamTraceOn(TokenChar _char)
    {
        m_camFollow.SetTarget(_char.GetObject());
        m_camFollow.TraceOnOff(true);
    }

    public void CamTraceOff()
    {
        m_camFollow.TraceOnOff(false);
    }
    #endregion

    private void PopupMessage(string message)
    {
        AlarmPopup.GetInstance().PopUpMessage(message);
    }

    private void AnnounceState(string message)
    {
        //  Debug.Log(message);
        Announcer.Instance.AnnounceState(message);
    }

    public bool AdaptActionCountRestrict = false;
    public bool AdaptInTileForAct = false; //해당 타일에 있어야 작업이 가능하게 
    public bool TempAdaptEvent = false;

    #endregion

    public GamePlayData GetPlayData()
    {
        return m_playData;
    }

    public void CharMoveToCapital(int _nationNum)
    {
        Nation nation = MgNation.GetInstance().GetNation(_nationNum);
        //그 나라의 수도 
        TokenTile nationCapitalTile = nation.GetCapital();
        //메인 캐릭터
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        //수도위치로 캐릭터 이동
        RuleBook.Migrate(mainChar, nationCapitalTile);
        //카메라 포커싱
        CamFocusMainChar();
        //초기 나라 선택하는 컨텐츠 부분이후 턴이 시작되어야함. 이부분을 
        //CharMoveToCapital()을 호출해서 병행중인데 해당 함수에 던Step기능이 있으면 안됨. 수정 필요. 
        Debug.LogWarning("CharMoveToCapital()을 호출해서 병행중인데 해당 함수에 던Step기능이 있으면 안됨. 수정 필요. ");
        DoneStep(GamePlayStep.GameInitialSetting); //게임 초기 세팅 이후 국가 선택까지 마치면 초기 세팅 끝. 
    }

    #region 작업서 관리
    public void RegistorWork(WorkOrder _order)
    {
        if(m_globalWorkList.IndexOf(_order) == -1)
        {
            m_globalWorkList.Add(_order);
        }
    }

    public void RemoveWork(WorkOrder _work)
    {
        m_globalWorkList.Remove(_work);
    }

    private void DoWorkList()
    {
        WorkOrder[] workArray = m_globalWorkList.ToArray();
        for (int i = 0; i < workArray.Length; i++)
        {
            workArray[i].DoWork();
        }
    }
    #endregion

    #region 불만관리
    public void RegistorComplain(Complain _complain)
    {
        if(m_globalComplainList.IndexOf(_complain) == -1)
        {
            m_globalComplainList.Add(_complain);
        }
    }

    public void RemoveComplain(Complain _complain)
    {
        m_globalComplainList.Remove(_complain);
    }

    public void CountComplainTurn()
    {
        Complain[] complainArray = m_globalComplainList.ToArray();
        for (int i = 0; i < complainArray.Length; i++)
        {
            complainArray[i].TurnCount();
        }
    }
    #endregion

    public void PlayerDead()
    {
        TOrderItem deathCode = new TOrderItem(TokenType.Content, (int)ContentEnum.GameState, (int)GameStateEnum.PlayerDead);
        MGContent.GetInstance().SendActionCode(deathCode);
    }
}

public enum GameStateEnum
{
    Play, PlayerDead, Lose, Victory
}

//게임 전체 관점에서 단계
public enum GamePlayStep
{
    GameInitialSetting, NationTurn, CharTurn, ReadyNextTurn, NationSettle
}

//캐릭터 차례 진행 단계
public enum CharTurnStep
{
    //게임마스터가 게임을 진행하면서 현재 스텝을 정의 
    ChooseNation, SelectNationPolicy,
    ChooseChar, SelectCharAct, FillCharActiContent, PlayCharAction, EndCharTurn
}

public class GamePlayData
{
    public int PlayTime = 0;
}