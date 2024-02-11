using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MgGeneric<PlayerManager>, PlayerRule
{
    [SerializeField]
    UIPlayGame m_playGameUI; //플레이어의 액션에 관련된 UI
    [SerializeField]
    DisplayActionToken m_displayAction;
    [SerializeField]SoundManager m_soundMg;
    private GamePlayStep m_curStep = GamePlayStep.EndTurn; //현재 플레이 단계 
    private TokenChar m_curChar; //현재 선택된 캐릭터
    private TokenAction m_curAction;
    private TokenChar m_mainChar = null; //메인 캐릭터
    private PlayerCapitalData m_playerCapitalData = new(); //플레이어의 자원 정보

    [Header("Efx 별도 확보")]
    [SerializeField]
    AudioClip actionSelectEFx;
    [SerializeField]
    AudioClip eventSelectEFx;

    public override void InitiSet()
    {
        base.InitiSet();
        m_mainChar = MgToken.GetInstance().GetMainChar();
        m_mainChar.isMainChar = true;
    }

    #region 플레이어 인풋 - 클릭, 선택 등
    #region 클릭- 한번, 더블, 취소
    public void ClickTokenObject(TokenBase _token)
    {
        //플레이에서 Clicker로 클릭을 감지한 경우 이녀석으로 클릭한걸 넘긴다. 
        //무슨 토큰을 눌렀나
        m_soundMg.PlayEfx(actionSelectEFx);
        m_playGameUI.ResetSnapInfo(_token);
        TokenType tokenType = _token.GetTokenType();
        // Debug.Log(m_step + "에 " + tokenType + "눌림");
        switch (m_curStep)
        {
            case GamePlayStep.ChooseChar:
            case GamePlayStep.SelectAct:
                //액션을 골라서 타겟을 고르는 상태가 아니라면 
                ChangedPlayerStep(GamePlayStep.ChooseChar); //일단 기본 아무것도 안고른 상태로 돌리고
                if (tokenType == TokenType.Char) //만약 _token 타입이 캐릭이라면 해당 캐릭고른걸로
                {
                    //게임마스터에게 얘 골랐다고 전달 - 액션 지니고 있어야하네 
                   
                    TokenChar charToken = (TokenChar)_token;
                    if (charToken.IsPlayerChar())
                    {
                        m_curChar = charToken;
                        GamePlayMaster.g_instance.EmphasizeTarget(m_curChar);
                        ChangedPlayerStep(GamePlayStep.SelectAct);
                    }
                }
                break;
            case GamePlayStep.FillContent:
                //누른게 타일이던 그 위의 오브젝트던 일단 위치로 변환해서
                
                if (GamePlayMaster.g_instance.RuleBook.IsInRangeTarget(m_curChar, m_curAction, _token) == false)
                {
                    Debug.Log("해당 타겟은 사거리밖이라 요청 반려");
                    return;
                }

                m_curAction.ClearTarget();
                m_curAction.SetTargetCoordi(_token.GetMapIndex());
                m_playGameUI.AddContent(_token);
                ConfirmAction();
                break;
        }
    }

    public void DoubleClickTokenObject(TokenBase _token)
    {
        //더블클릭한 토큰 타입에 따라 UI 세팅
        TokenType tokenType = _token.GetTokenType();

        if (tokenType.Equals(TokenType.Tile))
            m_playGameUI.ShowTileWorkShopUI((TokenTile)_token);
    }

    public void ClickCancle()
    {
        //어떤 도구로든 취소가 들어왓다면
        //1. 켜져있는 UI가 있으면 걔를 우선적으로 끌것
        if (m_playGameUI.CheckLastUI())
        {
            //만약 펼쳐져있던 UI가 있으면 UI를 끄기 수행.
            m_playGameUI.CancleLastUI();
            return;
        }

        //2. 없다면 조작중인 상태의 롤벡으로 진행
        if (m_curStep.Equals(GamePlayStep.SelectAct))
        {
            ChangedPlayerStep(GamePlayStep.ChooseChar);
            return;
        }
        if (m_curStep.Equals(GamePlayStep.FillContent))
        {
            ChangedPlayerStep(GamePlayStep.SelectAct);
            return;
        }
    }
    #endregion

    #region 선택 - 캐릭 액션, 이벤트, 타일 액션
    public void SelectActionToken(TokenBase _token)
    {
        //Debug.Log("액션 고름");
        m_soundMg.PlayEfx(actionSelectEFx);
        TokenAction actionToken = (TokenAction)_token;
        //0. 액션 토큰 사용 조건 확인
        if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(m_curChar, actionToken) == false)
        {
            Announcer.Instance.AnnounceState("캐릭터 행동 수치 부족");

            if (GamePlayMaster.g_instance.TempAdaptActionCount)
                return;
        }
        //1. 현재 액션으로 할당하고, 단계 변화
        m_curAction = actionToken;
        ChangedPlayerStep(GamePlayStep.FillContent);//액션토큰을 골랐으면 내용채우기 단계로

    }

    public void SelectEventToken(TokenBase _eventToken)
    {
        m_soundMg.PlayEfx(eventSelectEFx);
        TokenEvent eventToken = (TokenEvent)_eventToken;
        //이벤트 선택 가능여부는 제쳐두고 
        m_playGameUI.OffPlayUI();
        GamePlayMaster.g_instance.SelectEvent(eventToken);
    }

    public void ConfirmAction()
    {
        //현재 액션을 수행할것을 확인 누름 
        if (GamePlayMaster.g_instance.RuleBook.CheckActionContent(m_curChar, m_curAction) == false)
            return;

        //플레이어 쪽이 먼저 단계를 바꿔놔야함,
        ChangedPlayerStep(GamePlayStep.PlayAction);//내용을 채워서 액션수행 요청을 할때

        //액션의 실제 수행여부는 상관없이 액션 횟수는 차감
        int needCount = m_curAction.GetStat(ActionStat.NeedActionCount);
        m_curChar.CalStat(CharStat.CurActionCount, -needCount);

        GamePlayMaster.g_instance.PlayCharAction(m_curChar);
        
    }

    public void SelectTileAction(TokenTile _selectedToken, TokenAction _tileAction)
    {
        GamePlayMaster.GetInstance().PlayTileAction(_selectedToken, _tileAction);
    }
    #endregion
    #endregion

    #region 플레이어 턴 수행 인터페이스
    public void PlayTurn()
    {
        if (GamePlayMaster.g_instance.m_testAuto)
        {
            Debug.Log("리얼 플레이어 자동 턴 종료 선언");
            EndTurn();
        }
        GamePlayMaster.GetInstance().FogContorl(m_mainChar);
        ChangedPlayerStep(GamePlayStep.ChooseChar);
    }

    public void DoneCharAction(TokenChar _char)
    {
        ChangedPlayerStep(GamePlayStep.ChooseChar);//액션 수행이 되었으면 다시 캐릭선택상태로
    }

    public void EndTurn()
    {
        if (m_curStep.Equals(GamePlayStep.EndTurn))
            return;

        ChangedPlayerStep(GamePlayStep.EndTurn);
        GamePlayMaster.g_instance.EndPlayerTurn();
    }

    public GamePlayStep GetCurPlayStep()
    {
        return m_curStep;
    }
    #endregion //플레이어 인터페이스

    #region 이벤트 토큰 관련
    private GamePlayStep m_preStep = GamePlayStep.SelectAct;
    public void OnTriggerEvent(List<TokenEvent> _events)
    {
        m_preStep = m_curStep; //현재 단계 저장해놓고 
        ChangedPlayerStep(GamePlayStep.TriggerEvent);
        m_playGameUI.ShowEventList(_events);
    }
    public void DoneAdaptEvent()
    {
        //이벤트 적용 후
        //1. 획득한 자원값 적용
        ChangedPlayerStep(m_preStep); //이전 스텝으로 돌아가고
    }


    #endregion

    //플레이어 자본 데이터 조정 
    public void AdaptCapitalStat(Capital _resource, int _value, bool isCal)
    {
        string reward = string.Format("{0} 자원 {1} 확보", _resource, _value);
        Announcer.Instance.AnnounceState(reward);
        //기존데이터에 적용인지, 덮는건지 
        if (isCal)
        {
            m_playerCapitalData.CalValue(_resource, _value);
        }
        else
        {
            m_playerCapitalData.SetValue(_resource, _value);
        }

        m_playGameUI.ResetCapitalInfo(m_playerCapitalData);
    }

    public void SetMainChar(TokenChar _mainChar)
    {
        _mainChar.isMainChar = true;
        m_mainChar = _mainChar;
    }
    public TokenChar GetMainChar()
    {
        return m_mainChar;
    }

    public void OnChangePlayData()
    {
        m_playGameUI.ResetPlayData();
    }

    //플레이어 스텝 단계가 바뀐경우, 그 상태에 필요한 초기 세팅(값들, ui들)
    private void ChangedPlayerStep(GamePlayStep _step)
    {
        //스텝이 바뀌었을 때 기본적인 세팅을 하기 
        m_curStep = _step;
        if (m_curStep.Equals(GamePlayStep.ChooseChar))
        {
            m_curChar = null;
            m_playGameUI.OffPlayUI();
            m_displayAction.OffActionDisplay();
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.SelectAct))
        {
            m_curAction = null;
            m_playGameUI.ShowActionToken(m_curChar); //UI적으로 필요한 처리 진행
            //m_displayAction.ShowActionTokens(m_curChar);
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.FillContent))
        {
            //내용채우기 단계가 되면

            //1. 선택된 캐릭터에 선택되었던 액션을 수행할 액션으로 세팅하고
            m_curChar.SetNextAction(m_curAction);
            //2. 액션 타겟 초기화
            m_curAction.ClearTarget();
            //3. UI 표기
            //m_playGameUI.ShowFillContentUI(m_curChar, m_curAction);
            //m_displayAction.OffActionDisplay();

            //4. 선택한 액션의 타겟 오브젝트 강조 표시
            GamePlayMaster.g_instance.EmphasizeTargetObject(m_curChar.GetXIndex(), m_curChar.GetYIndex(), m_curAction); //기본 이동 거리 세팅 

            return;
        }
        if (m_curStep.Equals(GamePlayStep.PlayAction))
        {
            m_playGameUI.OffPlayUI();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.EndTurn))
        {
            m_curChar = null;
            m_curAction = null;
            GamePlayMaster.g_instance.ResetEmphasize();
            m_playGameUI.OffPlayUI();
            return;
        }
    }

}
