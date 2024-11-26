using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MgGeneric<PlayerManager>, PlayerRule, KeyInterceptor
{
    [SerializeField]
    MgUI m_playGameUI; //플레이어의 액션에 관련된 UI
    [SerializeField]SoundManager m_soundMg;
    private CharTurnStep m_curStep = CharTurnStep.EndCharTurn; //현재 플레이 단계 
    private TokenChar m_curChar; //현재 선택된 캐릭터
    private TokenTile m_curTile; //현재 선택한 땅
    private TokenAction m_curAction;
    private TokenChar m_mainChar = null; //메인 캐릭터

    public bool m_autoEnd = true; //가능한 액션이 없으면 자동 종료 되는 부분 
    [Header("Efx 별도 확보")]
    [SerializeField]
    AudioClip actionSelectEFx;
    [SerializeField]
    AudioClip eventSelectEFx;

    #region 세팅
    public override void ManageInitiSet()
    {
        base.ManageInitiSet();
        new PlayerCapitalData();
        m_mainChar = MgToken.GetInstance().GetMainChar();
        m_mainChar.isMainChar = true;
    }

    public override void ReferenceSet()
    {
        MgInput.GetInstance().SetInterCeptor(this);
    }

    public void FirstStart()
    {
        //메인 캐릭터 선택된 상태로 시작
        m_curChar = m_mainChar;
        m_playGameUI.ShowCharActionList();
    }

    public void LoadPlayer()
    {
        m_mainChar = MgToken.GetInstance().GetMainChar();
        FirstStart();
    }

    #endregion 

    #region 플레이어 인풋 - 클릭, 선택 등
    #region 클릭- 한번, 더블, 취소
    public void ClickTokenBase(TokenBase _token)
    {
        //1. 선택 사운드
        m_soundMg.PlayEfx(actionSelectEFx);
        //2. 스냅인포 갱신
        m_playGameUI.ResetSnapInfo(_token);
        TokenType tokenType = _token.GetTokenType();
        // Debug.Log(m_step + "에 " + tokenType + "눌림");
        switch (m_curStep)
        {
            case CharTurnStep.ChooseChar:
            case CharTurnStep.SelectCharAct:
                //액션을 골라서 타겟을 고르는 상태가 아니라면 
                ChangedPlayerStep(CharTurnStep.ChooseChar); //일단 기본 아무것도 안고른 상태로 돌리고
                if (tokenType == TokenType.Char) //만약 _token 타입이 캐릭이라면 해당 캐릭고른걸로
                {
                    //게임마스터에게 얘 골랐다고 전달 - 액션 지니고 있어야하네 
                   
                    TokenChar charToken = (TokenChar)_token;
                    if (charToken.IsPlayerChar())
                    {
                        m_curChar = charToken;
                        GamePlayMaster.g_instance.EmphasizeTarget(m_curChar);
                        ChangedPlayerStep(CharTurnStep.SelectCharAct);
                    }
                }
                break;
            case CharTurnStep.FillCharActiContent:
                //누른게 타일이던 그 위의 오브젝트던 일단 위치로 변환해서
                
                if (GamePlayMaster.g_instance.RuleBook.IsInRangeTarget(m_curChar, m_curAction, _token) == false)
                {
                   // Debug.Log("해당 타겟은 사거리밖이라 요청 반려");
                    return;
                }

                m_curAction.ClearTarget();
                m_curAction.SetTargetCoordi(_token.GetMapIndex());
                ConfirmAction();
                break;
        }
    }

    public void DoubleClickTokenBase(TokenBase _token)
    {
        //더블클릭한 토큰 타입에 따라 UI 세팅
        if (m_curStep.Equals(CharTurnStep.PlayCharAction))
            return;

         m_selectedToken = _token;
         m_playGameUI.ShowTokenObjectInfo(m_selectedToken);
            
    }

    public void ClickCancle()
    {
        //어떤 도구로든 취소가 들어왓다면
        //1. 켜져있는 UI가 있으면 걔를 우선적으로 끌것
        if (m_playGameUI.CheckOpenUI())
        {
            //만약 펼쳐져있던 UI가 있으면 UI를 끄기 수행.
            m_playGameUI.CancleLastUI();
            return;
        }

        //2. 없다면 조작중인 상태의 롤벡으로 진행
        if (m_curStep.Equals(CharTurnStep.SelectCharAct))
        {
            ChangedPlayerStep(CharTurnStep.ChooseChar);
            return;
        }
        if (m_curStep.Equals(CharTurnStep.FillCharActiContent))
        {
            ChangedPlayerStep(CharTurnStep.SelectCharAct);
            return;
        }
    }
    #endregion

    #region 선택 - 캐릭 액션, 이벤트, 타일 액션
    public void SelectActionToken(TokenBase _token)
    {
        //Debug.Log("액션 고름");
        m_soundMg.PlayEfx(actionSelectEFx);
        if(
          (m_curStep.Equals(CharTurnStep.ChooseChar) ||
          m_curStep.Equals(CharTurnStep.SelectCharAct)|| 
          m_curStep.Equals(CharTurnStep.FillCharActiContent)) 
           == false)
        {
            //현재 상태가 캐릭터 선택이나 액션 고르는 단계가 아니면 작용 안됨. 
            return;
        }
        TokenAction actionToken = (TokenAction)_token;
        //0. 액션 토큰 사용 조건 확인
        string failMessage = "";
        if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(m_curChar, actionToken, ref failMessage) == false)
        {
            if (m_curChar.IsPlayerChar())
                Announcer.Instance.AnnounceState(failMessage, true);

            if (GamePlayMaster.g_instance.AdaptActionCountRestrict)
                return;
        }
        //1. 현재 액션으로 할당하고, 단계 변화
        m_curAction = actionToken;
        ChangedPlayerStep(CharTurnStep.FillCharActiContent);//액션토큰을 골랐으면 내용채우기 단계로

    }

    public void ConfirmAction()
    {
        //현재 액션을 수행할것을 확인 누름 
        if (GamePlayMaster.g_instance.RuleBook.CheckActionContent(m_curChar, m_curAction) == false)
            return;

        //플레이어 쪽이 먼저 단계를 바꿔놔야함,
        ChangedPlayerStep(CharTurnStep.PlayCharAction);//내용을 채워서 액션수행 요청을 할때
        GamePlayMaster.g_instance.PlayCharAction(m_curChar);
        
    }
    #endregion

    public void PushNumKey(int _index)
    {
        if (m_curChar == null)
            return;

        if (m_curChar.GetActionList().Count <= _index)
            return;

        SelectActionToken( m_curChar.GetActionList()[_index]);
    }
    #endregion

    #region 플레이어 턴 수행 인터페이스
    public void PlayTurn()
    {
        MgUI.GetInstance().TurnEndButtonOnOff(true);
        PopupDamage.GetInstance().DamagePop(m_mainChar.GetObject().gameObject, 10);
        //1. 안개 적용하고
        GamePlayMaster.GetInstance().FogContorl(m_mainChar);
        //2. 플레이어 상태 선택으로 변경
        ChangedPlayerStep(CharTurnStep.ChooseChar); 
        //3. 만약 자동 주행이라면 바로 종료 
        if (GamePlayMaster.g_instance.m_testAuto)
        {
            EndTurn(); //Debug.Log("리얼 플레이어 자동 턴 종료 선언");
        }
    }

    public void DoneCharAction(TokenChar _char)
    {
        ChangedPlayerStep(CharTurnStep.ChooseChar);//액션 수행이 되었으면 다시 캐릭선택상태로
        AutoEnd();
    }

    private void AutoEnd()
    {
        if (m_mainChar.GetStat(CharStat.CurActionCount) == 0 && m_autoEnd == true)
            EndTurn();
    }

    public void EndTurn()
    {
        if (m_curStep.Equals(CharTurnStep.EndCharTurn))
            return;

        ChangedPlayerStep(CharTurnStep.EndCharTurn);
        MgUI.GetInstance().TurnEndButtonOnOff(false);
        GamePlayMaster.g_instance.EndPlayerTurn();
    }

    public CharTurnStep GetCurPlayStep()
    {
        return m_curStep;
    }
    #endregion //플레이어 인터페이스

    public void OnChangePlayData()
    {
        m_playGameUI.ResetPlayData();
    }

    public void SetHeroPlace(TileType _tile)
    {
        //플레이어가 입장한 곳 
        TOrderItem placeItem = new TOrderItem(TokenType.OnChange, (int)OnChangeEnum.OnPlaceChange, (int)_tile);
        MGContent.GetInstance().SendActionCode(placeItem);
    }

    #region 플레이어 할당된 Token 가져오기
    public TokenChar GetMainChar()
    {
        return m_mainChar;
    }

    public TokenTile GetSelectedTile()
    {
        return m_curTile;
    }

    public TokenAction GetSelectedAction()
    {
        return m_curAction;
    }

    public TokenChar GetSelectedChar()
    {
        return m_curChar;
    }

    private TokenBase m_selectedToken;
    public TokenBase GetSelectedToken()
    {
        return m_selectedToken;
    }
    #endregion

    //플레이어 스텝 단계가 바뀐경우, 그 상태에 필요한 초기 세팅(값들, ui들)
    private void ChangedPlayerStep(CharTurnStep _step)
    {
        //스텝이 바뀌었을 때 기본적인 세팅을 하기 
        m_curStep = _step;
        if (m_curStep.Equals(CharTurnStep.ChooseChar))
        {
            m_playGameUI.ShowCharActionList();
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(CharTurnStep.SelectCharAct))
        {
            m_curAction = null;
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(CharTurnStep.FillCharActiContent))
        {
            //1. 선택된 캐릭터에 선택되었던 액션을 수행할 액션으로 세팅하고
            m_curChar.SetNextAction(m_curAction);
            //2. 액션 타겟 초기화
            m_curAction.ClearTarget();
            //3. 선택한 액션의 범위 타일 표기
            GamePlayMaster.g_instance.EmphasizeTargetTileObject(m_curChar, m_curAction); //기본 이동 거리 세팅 
            return;
        }
        if (m_curStep.Equals(CharTurnStep.PlayCharAction))
        {
            m_playGameUI.OffPlayUI();
            return;
        }
        if (m_curStep.Equals(CharTurnStep.EndCharTurn))
        {
            m_curAction = null;
            GamePlayMaster.g_instance.ResetEmphasize();
            m_playGameUI.OffPlayUI();
            return;
        }
    }

    #region 액션 관리
    public List<TokenAction> GetPlayerActionList()
    {
        return m_mainChar.GetActionList();
    }

    public bool IsStudyAction(int _actionPid)
    {
        int level = GetPlayerActionLevel(_actionPid);
        if (level != FixedValue.No_VALUE)
            return true;

        return false;
    }

    public int GetPlayerActionLevel(int _actionPid)
    {
        int level = FixedValue.No_VALUE; //기본 노벨류
        List<TokenAction> actionList = GetPlayerActionList();
        int findActionPid = _actionPid;
        for (int i = 0; i < actionList.Count; i++)
        {
            int haveActionPid = actionList[i].GetPid();
            if (findActionPid == haveActionPid)
            {
                return 1;
            }

        }
        return level;
    }

    public bool StudyPlayerAction(int _actionPid)
    {
        Debug.LogWarning("학습소에서 캐릭터로 직접 습득 구조 변환 요망");
        TokenAction masterAction = MgMasterData.GetInstance().GetMasterCharAction(_actionPid);
        //2. 존재하는 스킬인지 체크
        if (masterAction == null)
            return false;

        //4. 스킬 할당
        TokenAction charAction = new TokenAction(masterAction);
        m_mainChar.AquireAction(charAction);

        //5. 액션 슬롯 ui 갱신
        m_playGameUI.ShowCharActionList();

        return true;
    }
    #endregion

    public void SetPreIntecoptor(KeyInterceptor _ceptor)
    {
        //플레이어매니저는 최종으로서 이전 키인터셉터 할필요없음.     
    }
}
