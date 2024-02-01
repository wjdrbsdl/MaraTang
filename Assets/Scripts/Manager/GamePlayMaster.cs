using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GamePlayMaster : MgGeneric<GamePlayMaster>
{
    #region  변수
    public enum PlayerMember
    {
        LivePlayer, AI
    }
    public float m_moveSpeed = 0.5f;
    public const float c_movePrecision = 0.1f; //움직임 정밀도
    public Sprite[] m_testActionIcon;
    public bool m_testAuto = true;

    
    private GamePlayData m_playData = new();
    [SerializeField]
    private UIPlayData m_playDataUI;

    [SerializeField]
    private CameraFollow m_camFollow;
    private PlayerMember m_playerMemeber = PlayerMember.LivePlayer; // 0번 플레이어, 1번 AI
    private PlayerRule[] m_players;
    private AIPlayer m_aiPlayer;
    public RuleBook RuleBook;
    public EmphasizeObject EmphasizeTool;

    
    [Header("[배틀씬]")]
    [SerializeField] private UIBattle m_battleUI;
    #region 행동 예약 변수
    Queue<IEnumerator> actionReserVationQueue = new(); //수행할 코루틴
    private bool isPlayingCorutine = false; //선행중인 코루틴이 있는지 체크
    private int m_FinalActionStep = 0;
    public float waitTime = 2f;
    TokenChar m_actionChar;
    #endregion

    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
           m_playData.PlayTime = 0;
            m_playerMemeber = 0;
            NoticeTurnPlayer();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            for (int i = 0; i < MgToken.GetInstance().GetNpcPlayerList().Count; i++)
            {
                TokenChar charToken = MgToken.GetInstance().GetNpcPlayerList()[i];
                MgToken.GetInstance().TempPosRandomPlayer(charToken);
            }
           
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            MgToken.GetInstance().MakeMap(); // 맵 다시만들기
        }
        if (Input.GetKeyDown(KeyCode.F8))
        {
            if(m_playerMemeber.Equals(PlayerMember.LivePlayer))
            {
                if (m_players[(int)PlayerMember.LivePlayer].GetCurPlayStep() != GamePlayStep.ChooseChar)
                    return;
                //리얼 플레이어 차례일때 누르면 라이브 플레이어 턴 종료 선언
                EndPlayerTurn();
                Debug.Log("플레이어 턴 종료 선언");
            }
        }
    }

    #region 초기화 및 시작
    public override void InitiSet()
    {
        base.InitiSet();
        RuleBook = new();
        RuleBook.m_PlayMaster = this;
        m_aiPlayer = new();
        m_players = new PlayerRule[2];
        EmphasizeTool = new();
    }

    public void FirstStart()
    {
        m_aiPlayer.SetInitial(); //mgtoken이 토큰 다 만들고 나서 진행해야함.
        m_playerMemeber = PlayerMember.LivePlayer; //시작은 플레이어
        m_players[0] = PlayerManager.GetInstance();
        m_players[1] = m_aiPlayer;
        ReadyNextTurn();
    }
    #endregion

    #region 액션 수행
    //게임마스터는 플레이어들이 턴종료때까지 대기, 수행할 캐릭터 액션토큰을 정해서 요구하면 해당 액션을 수행해주는 역할 

    //1. 해당 차례 플레이어에게 액션 수행하라고 전달
    public void NoticeTurnPlayer()
    {
        //플레이어 여러명이 있다고 가정하고 순서대로 진행 - 현재는 1.나 2.AI 로 진행 
        int turn = (int)m_playerMemeber;
        m_players[turn].PlayTurn();

        //해당 플레이턴이 될때 카메라 고정을 풀거나, 하도록
        CamTraceOn(MgToken.GetInstance().GetNpcPlayerList()[turn]);
        CamTraceOff();
    }
 
    //2. 전달 받은 캐릭과 액션을 수행 해줌
    public void PlayCharAction(TokenChar _charToken)
    {
        //캐릭터 토큰이 준비된대로 룰북가지고 액션을 수행
        ResetEmphasize();
        _charToken.actionCount -= 1;
        _charToken.ShowAction(true);
        CamTraceOn(_charToken);
        RuleBook.PlayAction(_charToken);
    }

    //3. 액션 수행 끝났으면 해당 플레이어게 해당 캐릭터 액션수행 끝났음을 알림. 
    public void DoneCharAction(TokenChar _charToken)
    {
        //   Debug.Log("<<color=red>마스터 :  " + _charToken.num + "번 더 수행할 행동력이 있나?</color>");
        CamTraceOff();
        _charToken.ShowAction(false);
        RuleBook.OffRouteNumber();
        m_players[(int)m_playerMemeber].DoneCharAction(_charToken); //현재 플레이어에게 해당 캐릭터의 액션이 완료되었음을 알린다.
        OccurMoveEvent(_charToken);
    }

    //4. 해당 플레이어가 턴을 종료하면, 다음 차례 플레이어를 뽑거나 턴정산을 진행
    public void EndPlayerTurn()
    {
        int cur = (int)m_playerMemeber;
        cur += 1; //다음차례

        if ((int)PlayerMember.AI < cur)
        {
            //만약 ai 턴이 끝난것이라면
            ReadyNextTurn(); //해당 턴 종료로 다음턴 준비
            return;
        }

        //그게 아니라면
        m_playerMemeber = (PlayerMember)cur; //현 플레이어 멤버 바꾸고
        NoticeTurnPlayer();//다음 차례 플레이어를 호출 
    }

    #endregion

    #region 액션 예약 및 수행 
    #region 1. 룰북에서 액션 예약을 거는 부분 - 애니메이션같이 시간이 필요한 액션이 있으므로, 하나씩 예약을 걸어두고 진행하는 방식. 
    public void RservereInfo(TokenChar _actionChar, int maxStep)
    {
        m_actionChar = _actionChar;
        m_FinalActionStep = maxStep;
    }

    public void Reservateinstance(int curStep, Action effectAction)
    {
        //즉시 발휘되는 액션들 예약하는 부분
        PlayReservation(curStep); //액션 플레이를 한다. 
    }

    public void ReservateMove(TokenChar _interViewChar, TokenTile _tile, int effectTiming, int curstep, Action<TokenChar, TokenTile> effectAction = null)
    {
        //룰북에서 해당 pid 액션타입에 맞게 별개의 동작을 예약하는 부분
        //ex 이부분은 움직임을 위한 액션을 예약하는 부분 
        IEnumerator moveCoroutin = co_MoveAction(_interViewChar, _tile, effectTiming, curstep, effectAction);
        actionReserVationQueue.Enqueue(moveCoroutin);
        PlayReservation(curstep);
    }

    //어택이 아니더라도 대상에게 어떠한 효과를 주는 경우에는 다해도 될것 같은데
    public void ReservateAttack()
    {
        m_battleUI.Switch(true);
        //어택 토큰을 상대에게 사용시 
        //------------ 데이터
        //1. 어택토큰의 기능 + 사용자 버프로 공격 산출 
        //2. 방어자의 버프 적용 최종 데미지 적용
        //3. 방어자의 반격 모드 적용 - 
        //4. 사후 적용 - 데이터 부분
        //------------ UI
        //1. 캐릭터 토큰들 버프, 스텟 표시 
        //2. 사용된 공방 토큰 버프 적용 상태 표시 
        //3. 최종 적용 스텟 표시 
        //-> 확인누르면 반격 진행 - 위과정 반복 
        //4. 위 루트 가운데 센터로 표기 - A가 B로 공격 - C가 D만큼 피해 등. 
        //5. 최종확인시 창 종료 


    }

    public void DoneReserve(int _curStep)
    {
        PlayReservation(_curStep); //룰북으로부터 예약이 끝나면 받는 부분 - 
        //오로지 즉발로 이뤄진 액션토큰의 경우 - 종료가 2번 호출될 수 있겠다. 
    }
    #endregion

    #region 2. 행동 수행 코루틴
    //액션마다 수행 애니메이션이 다르므로 따로 정의 
    IEnumerator co_MoveAction(TokenChar _char, TokenTile _goalTile, int effectTiming, int curStep, Action<TokenChar, TokenTile> effectAction)
    {
        //   Debug.Log("이동 코루틴 수행 단계" + m_MaxStep+"/ " + curStep);

        Vector3 goal = _goalTile.GetObject().transform.position;
        if (effectTiming == 0 && effectAction != null)
            effectAction(_char, _goalTile);

        if (effectTiming == 1 && effectAction != null)
            effectAction(_char, _goalTile);

        _char.SetState(CharState.Move);

        Vector3 dir = goal - _char.GetObject().transform.position;
        while (true)
        {
            if (Vector2.Distance(_char.GetObject().transform.position, goal) < c_movePrecision)
            {
                //Debug.Log("거리 가까워서 중단");
                break;
            }


            _char.GetObject().transform.position += (dir.normalized * m_moveSpeed * Time.deltaTime);
            yield return null;
        }

        _char.SetState(CharState.Idle);

        if (effectTiming == 2 && effectAction != null)
            effectAction(_char, _goalTile);

        yield return new WaitForSeconds(waitTime);
        DoneCoroutineAction(curStep);
    }

    IEnumerator co_AttackAction(TokenChar _char, int v, int d, int curStep)
    {
        DoneCoroutineAction(curStep);
        yield return null;

    }
    #endregion

    private void PlayReservation(int _curStep)
    {
        if (isPlayingCorutine == true)
            return; //선행중인 코루틴 있으면 할거 없음. - 선행중 추가 예약하는 경우에 발생 

        if (isPlayingCorutine == false && actionReserVationQueue.Count >= 1) //CheckMaxStep과 Reservate() 두 경로에서 호출되므로 선행중인 코루틴 체크 필요. 
        {
            isPlayingCorutine = true;
            StartCoroutine(actionReserVationQueue.Dequeue()); //뽑은걸로 수행
            return;
        }

        //액션 단계중 코루틴이 없는 step인 경우 도달할 수 있으므로, 그 스텝이 최종 스텝인지 따져서 액션 종료.
        if (m_FinalActionStep == _curStep)
        {
            //해당 액션은 코루틴이 없던 녀석인듯. 
            DoneCharAction(m_actionChar);
        }
    }

    private void DoneCoroutineAction(int curStep)
    {
        isPlayingCorutine = false; //진행중이던 코루틴 끝났다고 체크. -> 즉발 함수로 이녀석을 호출해서 진행중이던 코루틴 중단시키면 안됨. 
        PlayReservation(curStep); //계속 수행 
    }
    #endregion // 액션 예약 및 수행 

    #region 타일 액션 수행
    public void PlayTileAction(TokenTile _tile, string _action)
    {

        if (_action.Equals(TileAction.Grass.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Grass, 50, true);
            return;
        }
        if (_action.Equals(TileAction.Mineral.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Stone, 50, true);
            return;
        }
        if (_action.Equals(TileAction.RemoveMineral.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Stone, -50, true);
            return;
        }
        if (_action.Equals(TileAction.RemoveGrass.ToString()))
        {
            PlayerManager.GetInstance().AdaptCapitalStat(Capital.Grass, -50, true);
            return;
        }

    }
    #endregion

    #region 사건 발생
    public void SelectEvent(TokenEvent _eventToken)
    {
        AnnounceState("플레이어가 선택한 이벤트는 " + _eventToken.GetPid());
        RuleBook.AdaptEvent(_eventToken);
    }

    private void OccurMoveEvent(TokenChar _charToken)
    {
        if (IsPlayerMoveDone(_charToken) == false)
            return;
        //만약 플레이어 캐릭터가 이동한거라면 해당 위치에서 다시 안개 설정
        FogContorl(_charToken);

        //입장 동시에 발발하는 이벤트가 있으면 수행
        TokenEvent enterEvent = RuleBook.CheckEnteranceEvent(_charToken.GetMapIndex());
        if (enterEvent != null)
        {
            RuleBook.PlayEntranceEvent(enterEvent);
            return;
        }
        else
        {
            Debug.Log("입장 이벤트 없드");
        }

        //없다면 도착에 따른 확률적 이벤트 수행
        RuleBook.OnTileArrive(_charToken);
    }

    private bool IsPlayerMoveDone(TokenChar _charToken)
    {
        //플레이어의 메인 캐릭터면서 최근한 행동이 Move인 경우 이벤트 발생
        if (_charToken.isMainChar == false)
            return false;
        if (_charToken.GetNextActionToken().GetActionType().Equals(ActionType.Move) == false)
            return false;

        return true;
    }

    #endregion

    #region 턴 정산 부분
    /*
     *  턴 넘김 순서 
        플레이어 턴 변화에 대한 수행
        오브젝트들 턴 변화에 대한 액션 변화 수행
     */

    private void ReadyNextTurn()
    {
        SettleActionTurn(); //액션 턴 정산
        SettleWorldTurn(); //월드 턴 정산
        SettingPlayerTurn(); //플레이어 턴으로 세팅
        SetPlayDataUI(); //플레이 데이터 갱신
        StartActionTurn(); //액션 턴 시작
    }

    private void SettleActionTurn()
    {
        m_playData.PlayTime += 1; //여태 진행한 턴
       // Debug.Log("행동 횟수 초기화");
        RecoverActionCount();
      //  Debug.Log("진행한 플레이어 넘버 초기화");
        m_playerMemeber = 0;
    }
    private void RecoverActionCount()
    {
       AnnounceState("소비된 액션 카운트 회복");
        for (int i = 0; i < MgToken.GetInstance().GetNpcPlayerList().Count; i++)
        {
            MgToken.GetInstance().GetNpcPlayerList()[i].actionCount = i + 1; //
        }
    }
    private void SettleWorldTurn()
    {
        AnnounceState("세계턴 변화 따짐 내용 없음");
      
    }

    private void SetPlayDataUI()
    {
        AnnounceState("플레이 데이터 갱신");
        m_playDataUI.ShowPlayData(m_playData);
    }

    private void SettingPlayerTurn()
    {
        //AI 턴까지 끝나면 다시 리얼플레이어 턴으로 초기화 해서 시작 
        m_playerMemeber = PlayerMember.LivePlayer; //처음 유저로 플레이어 코드 변경. 
        
    }

    private void StartActionTurn()
    {
        Invoke(nameof(NoticeTurnPlayer), 0.2f); //다시 진행
    }
    #endregion

    #region 부가 편의 기능
    public void FogContorl(TokenChar _char)
    {
        //일단 안개 걷는 기능만
        int tempSight = 2;
        List<TokenTile> tiles = GameUtil.GetTileTokenListInRange(tempSight, _char.GetXIndex(), _char.GetYIndex());
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].ChangeViewState(TileViewState.Sight);
        }
    }

    public void EmphasizeTarget(TokenBase _token)
    {
        EmphasizeTool.Emphasize(_token);
    }

    public void EmphasizeTargetObject(int _centerX, int _centerY, TokenAction _actionToken)
    {
        //토큰오브젝트를 강조하는 부분 (UI 부분은 UIPlayGame 에서 관리)
        List<ObjectTokenBase> objList = GameUtil.GetTokenObjectInRange(_actionToken.GetStat(ActionStat.Range), _centerX, _centerY);
        EmphasizeTool.Emphasize(objList);
    }

    public void ResetEmphasize()
    {
        EmphasizeTool.ResetEmphasize(); //선택시 필요했던 강조부분을 초기화 
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

    public void AnnounceState(string message)
    {
      //  Debug.Log(message);
      // 체크 깃허브
    }

    public bool AdaptActionCount = false;
    public bool AdaptEvent = false;

    #endregion

 }

public enum GamePlayStep
{
    //게임마스터가 게임을 진행하면서 현재 스텝을 정의 
    ChooseChar, SelectAct, FillContent, CheckDecision, PlayAction, TriggerEvent
}

public class GamePlayData
{
    public int PlayTime = 0;
}