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
    private CameraFollow m_camFollow;
    private PlayerMember m_playerMemeber = PlayerMember.LivePlayer; // 0번 플레이어, 1번 AI
    private PlayerRule[] m_players;
    private AIPlayer m_aiPlayer;
    public RuleBook RuleBook;
    public EmphasizeObject EmphasizeTool;

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
            if (m_playerMemeber.Equals(PlayerMember.LivePlayer))
            {
                if (m_players[(int)PlayerMember.LivePlayer].GetCurPlayStep() != GamePlayStep.ChooseChar)
                    return;
                //리얼 플레이어 차례일때 누르면 라이브 플레이어 턴 종료 선언
                PlayerManager.GetInstance().EndTurn();
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
        Announcer play = new();
        m_aiPlayer = new();
        m_players = new PlayerRule[2];
        EmphasizeTool = new();
    }

    public void FirstStart()
    {
        Debug.Log("처음 시작");
        RuleBook.ParseTileActions();
        m_aiPlayer.SetInitial(); //mgtoken이 토큰 다 만들고 나서 진행해야함.
        m_playerMemeber = PlayerMember.LivePlayer; //시작은 플레이어
        //플레이어 할당
        m_players[0] = PlayerManager.GetInstance();
        m_players[1] = m_aiPlayer;
        //턴 준비
        ReadyNextTurn();
        //처음 시작시 플레이어 메인캐릭터에 카메라 포커스.
        GamePlayMaster.GetInstance().CamFocus(PlayerManager.GetInstance().GetMainChar());
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
       //CamFocus(MgToken.GetInstance().GetNpcPlayerList()[turn]);

    }

    //2. 전달 받은 캐릭과 액션을 수행 해줌
    public void PlayCharAction(TokenChar _charToken)
    {
        //캐릭터 토큰이 준비된대로 룰북가지고 액션을 수행
        ResetEmphasize();
        _charToken.actionCount -= 1;
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
   
    #region 타일 액션 수행
    public void PlayTileAction(TokenTile _tile, TokenAction _action)
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

    #region 이벤트 발생
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
      
        //Debug.Log("입장 이벤트 없드");
       
        //없다면 도착에 따른 확률적 이벤트 수행
        int setCount = UnityEngine.Random.Range(1, 100);
        if(setCount >= 90)
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
            MgToken.GetInstance().GetNpcPlayerList()[i].RecoverActionTokenCount();
        }
    }
    private void SettleWorldTurn()
    {
        AnnounceState("세계턴 변화 따짐 내용 없음");
      
    }

    private void SetPlayDataUI()
    {
        AnnounceState("플레이 데이터 갱신");
        PlayerManager.GetInstance().OnChangePlayData();
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

    #region 오브젝트 강조
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
    #endregion

    #region 카메라 조정
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

    public void AnnounceState(string message)
    {
        //  Debug.Log(message);
        Announcer.Instance.AnnounceState(message);
    }

    public bool TempAdaptActionCount = false;
    public bool TempAdaptEvent = false;

    #endregion

    public GamePlayData GetPlayData()
    {
        return m_playData;
    }
 }

public enum GamePlayStep
{
    //게임마스터가 게임을 진행하면서 현재 스텝을 정의 
    ChooseChar, SelectAct, FillContent, CheckDecision, PlayAction, TriggerEvent, EndTurn
}

public class GamePlayData
{
    public int PlayTime = 0;
}