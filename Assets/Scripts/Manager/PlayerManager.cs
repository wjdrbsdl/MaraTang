using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MgGeneric<PlayerManager>, PlayerRule, KeyInterceptor
{
    [SerializeField]
    MgUI m_playGameUI; //�÷��̾��� �׼ǿ� ���õ� UI
    [SerializeField]SoundManager m_soundMg;
    private CharTurnStep m_curStep = CharTurnStep.EndCharTurn; //���� �÷��� �ܰ� 
    private TokenChar m_curChar; //���� ���õ� ĳ����
    private TokenTile m_curTile; //���� ������ ��
    private TokenAction m_curAction;
    private TokenChar m_mainChar = null; //���� ĳ����

    public bool m_autoEnd = true; //������ �׼��� ������ �ڵ� ���� �Ǵ� �κ� 
    [Header("Efx ���� Ȯ��")]
    [SerializeField]
    AudioClip actionSelectEFx;
    [SerializeField]
    AudioClip eventSelectEFx;

    #region ����
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
        //���� ĳ���� ���õ� ���·� ����
        m_curChar = m_mainChar;
        m_playGameUI.ShowCharActionList();
    }

    public void LoadPlayer()
    {
        m_mainChar = MgToken.GetInstance().GetMainChar();
        FirstStart();
    }

    #endregion 

    #region �÷��̾� ��ǲ - Ŭ��, ���� ��
    #region Ŭ��- �ѹ�, ����, ���
    public void ClickTokenBase(TokenBase _token)
    {
        //1. ���� ����
        m_soundMg.PlayEfx(actionSelectEFx);
        //2. �������� ����
        m_playGameUI.ResetSnapInfo(_token);
        TokenType tokenType = _token.GetTokenType();
        // Debug.Log(m_step + "�� " + tokenType + "����");
        switch (m_curStep)
        {
            case CharTurnStep.ChooseChar:
            case CharTurnStep.SelectCharAct:
                //�׼��� ��� Ÿ���� ���� ���°� �ƴ϶�� 
                ChangedPlayerStep(CharTurnStep.ChooseChar); //�ϴ� �⺻ �ƹ��͵� �Ȱ� ���·� ������
                if (tokenType == TokenType.Char) //���� _token Ÿ���� ĳ���̶�� �ش� ĳ�����ɷ�
                {
                    //���Ӹ����Ϳ��� �� ����ٰ� ���� - �׼� ���ϰ� �־���ϳ� 
                   
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
                //������ Ÿ���̴� �� ���� ������Ʈ�� �ϴ� ��ġ�� ��ȯ�ؼ�
                
                if (GamePlayMaster.g_instance.RuleBook.IsInRangeTarget(m_curChar, m_curAction, _token) == false)
                {
                   // Debug.Log("�ش� Ÿ���� ��Ÿ����̶� ��û �ݷ�");
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
        //����Ŭ���� ��ū Ÿ�Կ� ���� UI ����
        if (m_curStep.Equals(CharTurnStep.PlayCharAction))
            return;

         m_selectedToken = _token;
         m_playGameUI.ShowTokenObjectInfo(m_selectedToken);
            
    }

    public void ClickCancle()
    {
        //� �����ε� ��Ұ� ���Ӵٸ�
        //1. �����ִ� UI�� ������ �¸� �켱������ ����
        if (m_playGameUI.CheckOpenUI())
        {
            //���� �������ִ� UI�� ������ UI�� ���� ����.
            m_playGameUI.CancleLastUI();
            return;
        }

        //2. ���ٸ� �������� ������ �Ѻ����� ����
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

    #region ���� - ĳ�� �׼�, �̺�Ʈ, Ÿ�� �׼�
    public void SelectActionToken(TokenBase _token)
    {
        //Debug.Log("�׼� ��");
        m_soundMg.PlayEfx(actionSelectEFx);
        if(
          (m_curStep.Equals(CharTurnStep.ChooseChar) ||
          m_curStep.Equals(CharTurnStep.SelectCharAct)|| 
          m_curStep.Equals(CharTurnStep.FillCharActiContent)) 
           == false)
        {
            //���� ���°� ĳ���� �����̳� �׼� ���� �ܰ谡 �ƴϸ� �ۿ� �ȵ�. 
            return;
        }
        TokenAction actionToken = (TokenAction)_token;
        //0. �׼� ��ū ��� ���� Ȯ��
        string failMessage = "";
        if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(m_curChar, actionToken, ref failMessage) == false)
        {
            if (m_curChar.IsPlayerChar())
                Announcer.Instance.AnnounceState(failMessage, true);

            if (GamePlayMaster.g_instance.AdaptActionCountRestrict)
                return;
        }
        //1. ���� �׼����� �Ҵ��ϰ�, �ܰ� ��ȭ
        m_curAction = actionToken;
        ChangedPlayerStep(CharTurnStep.FillCharActiContent);//�׼���ū�� ������� ����ä��� �ܰ��

    }

    public void ConfirmAction()
    {
        //���� �׼��� �����Ұ��� Ȯ�� ���� 
        if (GamePlayMaster.g_instance.RuleBook.CheckActionContent(m_curChar, m_curAction) == false)
            return;

        //�÷��̾� ���� ���� �ܰ踦 �ٲ������,
        ChangedPlayerStep(CharTurnStep.PlayCharAction);//������ ä���� �׼Ǽ��� ��û�� �Ҷ�
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

    #region �÷��̾� �� ���� �������̽�
    public void PlayTurn()
    {
        MgUI.GetInstance().TurnEndButtonOnOff(true);
        PopupDamage.GetInstance().DamagePop(m_mainChar.GetObject().gameObject, 10);
        //1. �Ȱ� �����ϰ�
        GamePlayMaster.GetInstance().FogContorl(m_mainChar);
        //2. �÷��̾� ���� �������� ����
        ChangedPlayerStep(CharTurnStep.ChooseChar); 
        //3. ���� �ڵ� �����̶�� �ٷ� ���� 
        if (GamePlayMaster.g_instance.m_testAuto)
        {
            EndTurn(); //Debug.Log("���� �÷��̾� �ڵ� �� ���� ����");
        }
    }

    public void DoneCharAction(TokenChar _char)
    {
        ChangedPlayerStep(CharTurnStep.ChooseChar);//�׼� ������ �Ǿ����� �ٽ� ĳ�����û��·�
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
    #endregion //�÷��̾� �������̽�

    public void OnChangePlayData()
    {
        m_playGameUI.ResetPlayData();
    }

    public void SetHeroPlace(TileType _tile)
    {
        //�÷��̾ ������ �� 
        TOrderItem placeItem = new TOrderItem(TokenType.OnChange, (int)OnChangeEnum.OnPlaceChange, (int)_tile);
        MGContent.GetInstance().SendActionCode(placeItem);
    }

    #region �÷��̾� �Ҵ�� Token ��������
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

    //�÷��̾� ���� �ܰ谡 �ٲ���, �� ���¿� �ʿ��� �ʱ� ����(����, ui��)
    private void ChangedPlayerStep(CharTurnStep _step)
    {
        //������ �ٲ���� �� �⺻���� ������ �ϱ� 
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
            //1. ���õ� ĳ���Ϳ� ���õǾ��� �׼��� ������ �׼����� �����ϰ�
            m_curChar.SetNextAction(m_curAction);
            //2. �׼� Ÿ�� �ʱ�ȭ
            m_curAction.ClearTarget();
            //3. ������ �׼��� ���� Ÿ�� ǥ��
            GamePlayMaster.g_instance.EmphasizeTargetTileObject(m_curChar, m_curAction); //�⺻ �̵� �Ÿ� ���� 
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

    #region �׼� ����
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
        int level = FixedValue.No_VALUE; //�⺻ �뺧��
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
        Debug.LogWarning("�н��ҿ��� ĳ���ͷ� ���� ���� ���� ��ȯ ���");
        TokenAction masterAction = MgMasterData.GetInstance().GetMasterCharAction(_actionPid);
        //2. �����ϴ� ��ų���� üũ
        if (masterAction == null)
            return false;

        //4. ��ų �Ҵ�
        TokenAction charAction = new TokenAction(masterAction);
        m_mainChar.AquireAction(charAction);

        //5. �׼� ���� ui ����
        m_playGameUI.ShowCharActionList();

        return true;
    }
    #endregion

    public void SetPreIntecoptor(KeyInterceptor _ceptor)
    {
        //�÷��̾�Ŵ����� �������μ� ���� Ű���ͼ��� ���ʿ����.     
    }
}
