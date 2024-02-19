using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MgGeneric<PlayerManager>, PlayerRule
{
    [SerializeField]
    MgUI m_playGameUI; //�÷��̾��� �׼ǿ� ���õ� UI
    [SerializeField]
    DisplayActionToken m_displayAction;
    [SerializeField]SoundManager m_soundMg;
    private GamePlayStep m_curStep = GamePlayStep.EndTurn; //���� �÷��� �ܰ� 
    private TokenChar m_curChar; //���� ���õ� ĳ����
    private TokenTile m_curTile; //���� ������ ��
    private TokenAction m_curAction;
    private TokenChar m_mainChar = null; //���� ĳ����
    private PlayerCapitalData m_playerCapitalData = new(); //�÷��̾��� �ڿ� ����

    [Header("Efx ���� Ȯ��")]
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

    #region �÷��̾� ��ǲ - Ŭ��, ���� ��
    #region Ŭ��- �ѹ�, ����, ���
    public void ClickTokenObject(TokenBase _token)
    {
        //1. ���� ����
        m_soundMg.PlayEfx(actionSelectEFx);
        //2. �������� ����
        m_playGameUI.ResetSnapInfo(_token);
        TokenType tokenType = _token.GetTokenType();
        // Debug.Log(m_step + "�� " + tokenType + "����");
        switch (m_curStep)
        {
            case GamePlayStep.ChooseChar:
            case GamePlayStep.SelectAct:
                //�׼��� ��� Ÿ���� ���� ���°� �ƴ϶�� 
                ChangedPlayerStep(GamePlayStep.ChooseChar); //�ϴ� �⺻ �ƹ��͵� �Ȱ� ���·� ������
                if (tokenType == TokenType.Char) //���� _token Ÿ���� ĳ���̶�� �ش� ĳ�����ɷ�
                {
                    //���Ӹ����Ϳ��� �� ����ٰ� ���� - �׼� ���ϰ� �־���ϳ� 
                   
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
                //������ Ÿ���̴� �� ���� ������Ʈ�� �ϴ� ��ġ�� ��ȯ�ؼ�
                
                if (GamePlayMaster.g_instance.RuleBook.IsInRangeTarget(m_curChar, m_curAction, _token) == false)
                {
                    Debug.Log("�ش� Ÿ���� ��Ÿ����̶� ��û �ݷ�");
                    return;
                }

                m_curAction.ClearTarget();
                m_curAction.SetTargetCoordi(_token.GetMapIndex());
                ConfirmAction();
                break;
        }
    }

    public void DoubleClickTokenObject(TokenBase _token)
    {
        //����Ŭ���� ��ū Ÿ�Կ� ���� UI ����
      //  TokenType tokenType = _token.GetTokenType();
        m_selectedToken = _token;
        //if (tokenType.Equals(TokenType.Tile))
        //{
        //    m_curTile = (TokenTile)_token;
        //    m_playGameUI.ShowTileWorkShopUI();
        //}
        m_playGameUI.ShowTokenObjectUI(m_selectedToken);
            
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

    #region ���� - ĳ�� �׼�, �̺�Ʈ, Ÿ�� �׼�
    public void SelectActionToken(TokenBase _token)
    {
        //Debug.Log("�׼� ��");
        m_soundMg.PlayEfx(actionSelectEFx);
        if(!(m_curStep.Equals(GamePlayStep.ChooseChar) || m_curStep.Equals(GamePlayStep.SelectAct)))
        {
            //���� ���°� ĳ���� �����̳� �׼� ���� �ܰ谡 �ƴϸ� �ۿ� �ȵ�. 
            return;
        }
        TokenAction actionToken = (TokenAction)_token;
        //0. �׼� ��ū ��� ���� Ȯ��
        if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(m_curChar, actionToken) == false)
        {
            Announcer.Instance.AnnounceState("ĳ���� �ൿ ��ġ ����");

            if (GamePlayMaster.g_instance.TempAdaptActionCount)
                return;
        }
        //1. ���� �׼����� �Ҵ��ϰ�, �ܰ� ��ȭ
        m_curAction = actionToken;
        ChangedPlayerStep(GamePlayStep.FillContent);//�׼���ū�� ������� ����ä��� �ܰ��

    }

    public void SelectEventToken(TokenBase _eventToken)
    {
        m_soundMg.PlayEfx(eventSelectEFx);
        TokenEvent eventToken = (TokenEvent)_eventToken;
        //�̺�Ʈ ���� ���ɿ��δ� ���ĵΰ� 
        m_playGameUI.OffPlayUI();
        GamePlayMaster.g_instance.SelectEvent(eventToken);
    }

    public void ConfirmAction()
    {
        //���� �׼��� �����Ұ��� Ȯ�� ���� 
        if (GamePlayMaster.g_instance.RuleBook.CheckActionContent(m_curChar, m_curAction) == false)
            return;

        //�÷��̾� ���� ���� �ܰ踦 �ٲ������,
        ChangedPlayerStep(GamePlayStep.PlayAction);//������ ä���� �׼Ǽ��� ��û�� �Ҷ�

        //�׼��� ���� ���࿩�δ� ������� �׼� Ƚ���� ����
        int needCount = m_curAction.GetStat(CharActionStat.NeedActionCount);
        m_curChar.UseActionCount(needCount); 
        m_curChar.UseActionEnergy(m_curAction.GetStat(CharActionStat.NeedActionEnergy));
        GamePlayMaster.g_instance.PlayCharAction(m_curChar);
        
    }

    public void SelectTileAction(TokenTile _selectedToken, TokenAction _tileAction)
    {
        GamePlayMaster.GetInstance().PlayTileAction(_selectedToken, _tileAction);
    }
    #endregion
    #endregion

    #region �÷��̾� �� ���� �������̽�
    public void PlayTurn()
    {
        PopupDamage.GetInstance().DamagePop(m_mainChar.GetObject().gameObject, 10);
        if (GamePlayMaster.g_instance.m_testAuto)
        {
            Debug.Log("���� �÷��̾� �ڵ� �� ���� ����");
            EndTurn();
        }
        GamePlayMaster.GetInstance().FogContorl(m_mainChar);
        ChangedPlayerStep(GamePlayStep.ChooseChar);
    }

    public void DoneCharAction(TokenChar _char)
    {
        ChangedPlayerStep(GamePlayStep.ChooseChar);//�׼� ������ �Ǿ����� �ٽ� ĳ�����û��·�
        AutoEnd();
    }

    private void AutoEnd()
    {
        if (m_mainChar.GetStat(CharStat.CurActionCount) == 0)
            EndTurn();
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
    #endregion //�÷��̾� �������̽�

    #region �̺�Ʈ ��ū ����
    private GamePlayStep m_preStep = GamePlayStep.SelectAct;
    public void OnTriggerEvent(List<TokenEvent> _events)
    {
        m_preStep = m_curStep; //���� �ܰ� �����س��� 
        ChangedPlayerStep(GamePlayStep.TriggerEvent);
        m_playGameUI.ShowEventList(_events);
    }
    public void DoneAdaptEvent()
    {
        //�̺�Ʈ ���� ��
        //1. ȹ���� �ڿ��� ����
        ChangedPlayerStep(m_preStep); //���� �������� ���ư���
    }


    #endregion

    public void OnChangePlayData()
    {
        m_playGameUI.ResetPlayData();
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
    private void ChangedPlayerStep(GamePlayStep _step)
    {
        //������ �ٲ���� �� �⺻���� ������ �ϱ� 
        m_curStep = _step;
        if (m_curStep.Equals(GamePlayStep.ChooseChar))
        {
           // m_curChar = null;
            m_playGameUI.OffPlayUI();
            m_displayAction.OffActionDisplay();
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.SelectAct))
        {
            m_curAction = null;
            m_playGameUI.ShowActionToken(); //UI������ �ʿ��� ó�� ����
            //m_displayAction.ShowActionTokens(m_curChar);
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.FillContent))
        {
            //����ä��� �ܰ谡 �Ǹ�

            //1. ���õ� ĳ���Ϳ� ���õǾ��� �׼��� ������ �׼����� �����ϰ�
            m_curChar.SetNextAction(m_curAction);
            //2. �׼� Ÿ�� �ʱ�ȭ
            m_curAction.ClearTarget();
            //3. UI ǥ��
            //m_playGameUI.ShowFillContentUI(m_curChar, m_curAction);
            //m_displayAction.OffActionDisplay();

            //4. ������ �׼��� Ÿ�� ������Ʈ ���� ǥ��
            GamePlayMaster.g_instance.EmphasizeTargetObject(m_curChar, m_curAction); //�⺻ �̵� �Ÿ� ���� 

            return;
        }
        if (m_curStep.Equals(GamePlayStep.PlayAction))
        {
            m_playGameUI.OffPlayUI();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.EndTurn))
        {
           // m_curChar = null;
            m_curAction = null;
            GamePlayMaster.g_instance.ResetEmphasize();
            m_playGameUI.OffPlayUI();
            return;
        }
    }

}
