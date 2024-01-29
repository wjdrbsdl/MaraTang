using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MgGeneric<PlayerManager>, PlayerRule
{
    [SerializeField]
    UIPlayGame m_playGameUI; //�÷��̾��� �׼ǿ� ���õ� UI
    [SerializeField]SoundManager m_soundMg;
    private GamePlayStep m_curStep = GamePlayStep.SelectAct; //���� �÷��� �ܰ� 
    private TokenChar m_curChar; //���� ���õ� ĳ����
    private TokenAction m_curAction;
    private TokenChar m_mainChar = null; //���� ĳ����
    private PlayerCapitalData m_playerCapitalData; //�÷��̾��� �ڿ� ����

    [Header("Efx ���� Ȯ��")]
    [SerializeField]
    AudioClip actionSelectEFx;
    [SerializeField]
    AudioClip eventSelectEFx;

    #region �ʱ�ȭ
    public override void InitiSet()
    {
        base.InitiSet();
        g_instance = this;
        m_playerCapitalData = new();
    }
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ClickCancle();

        if (Input.GetMouseButtonDown(1))
            ClickCancle();
    }

    #region �÷��̾� ��ǲ���� ȣ��Ǵ� �κ�
    public void ClickTokenObject(TokenBase _token)
    {
        //�÷��̿��� Clicker�� Ŭ���� ������ ��� �̳༮���� Ŭ���Ѱ� �ѱ��. 
        //���� ��ū�� ������
        m_soundMg.PlayEfx(actionSelectEFx);
        TokenType tokenType = _token.GetTokenType();
        // Debug.Log(m_step + "�� " + tokenType + "����");
        switch (m_curStep)
        {
            case GamePlayStep.ChooseChar:
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
                if (GamePlayMaster.g_instance.RuleBook.IsMatchTargetType(m_curAction, _token)) //������ �׼ǿ� �°� ������� Ȥ�� �巡�׷� ������ ä���. 
                {
                    if (GamePlayMaster.g_instance.RuleBook.IsInRangeTarget(m_curChar, m_curAction, _token) == false)
                    {
                        Debug.Log("�ش� Ÿ���� ��Ÿ����̶� ��û �ݷ�");
                        return;
                    }

                    m_curAction.ClearTarget();
                    m_curAction.AddTarget(_token);
                    m_playGameUI.AddContent(_token);

                    ConfirmAction();
                }

                break;
        }
    }

    public void SelectActionToken(TokenBase _token)
    {
        //Debug.Log("�׼� ��");
        m_soundMg.PlayEfx(actionSelectEFx);
        TokenAction actionToken = (TokenAction)_token;
        //0. �׼� ��ū ��� ���� Ȯ��
        if (GamePlayMaster.g_instance.RuleBook.CheckUsableToken(m_curChar, actionToken) == false)
        {
            GamePlayMaster.g_instance.AnnounceState("ĳ���� �ൿ ��ġ ����");

            if (GamePlayMaster.g_instance.AdaptActionCount)
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

    public void ClickCancle()
    {
        //� �����ε� ��Ұ� ���Ӵٸ�
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

    public void ConfirmAction()
    {
        //���� �׼��� �����Ұ��� Ȯ�� ���� 
        if (GamePlayMaster.g_instance.RuleBook.CheckActionContent(m_curChar, m_curAction) == false)
            return;

        //�÷��̾� ���� ���� �ܰ踦 �ٲ������,
        //�׼��� ���� �����ϸ�, ��� �����°�� ChooseChar�ܰ�� �Ѿ���µ�, �� ���¿��� �ڿ� �ٽ� playaction�ܰ�� �����ؼ� ����
        ChangedPlayerStep(GamePlayStep.PlayAction);//������ ä���� �׼Ǽ��� ��û�� �Ҷ�
        GamePlayMaster.g_instance.PlayCharAction(m_curChar);
        
    }
    #endregion
 
    #region �÷��̾� �� ���� �������̽�
    public void PlayTurn()
    {
        if (GamePlayMaster.g_instance.m_testAuto)
        {
            Debug.Log("���� �÷��̾� �ڵ� �� ���� ����");
            EndTurn();
        }

        ChangedPlayerStep(GamePlayStep.ChooseChar);
    }

    public void DoneCharAction(TokenChar _char)
    {
        ChangedPlayerStep(GamePlayStep.ChooseChar);//�׼� ������ �Ǿ����� �ٽ� ĳ�����û��·�
    }

    public void EndTurn()
    {
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
    public void OnAdaptEvent()
    {
        //�̺�Ʈ ���� ��
        //1. ȹ���� �ڿ��� ����
        ChangedPlayerStep(m_preStep); //���� �������� ���ư���
    }


    #endregion

    //�÷��̾� �ں� ������ ���� 
    public void AdaptCapital(PlayerCapitalData.Capital _resource, int _value, bool isCal)
    {
        string reward = string.Format("{0} �ڿ� {1} Ȯ��", _resource, _value);
        GamePlayMaster.GetInstance().AnnounceState(reward);
        //���������Ϳ� ��������, ���°��� 
        if (isCal)
        {
            m_playerCapitalData.CalData(_resource, _value);
        }
        else
        {
            m_playerCapitalData.SetData(_resource, _value);
        }

        m_playGameUI.ResetCapitalInfo(m_playerCapitalData);
    }

    public void SetMainChar(TokenChar _mainChar)
    {
        _mainChar.isMainChar = true;
        m_mainChar = _mainChar;
    }

    //�÷��̾� ���� �ܰ谡 �ٲ���, �� ���¿� �ʿ��� �ʱ� ����(����, ui��)
    private void ChangedPlayerStep(GamePlayStep _step)
    {
        //������ �ٲ���� �� �⺻���� ������ �ϱ� 
        m_curStep = _step;
        if (m_curStep.Equals(GamePlayStep.ChooseChar))
        {
            m_curChar = null;
            m_playGameUI.OffPlayUI();
            GamePlayMaster.g_instance.ResetEmphasize();
            return;
        }
        if (m_curStep.Equals(GamePlayStep.SelectAct))
        {
            m_curAction = null;
            m_playGameUI.ShowActionToken(m_curChar); //UI������ �ʿ��� ó�� ����
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
            //3. UI �� ǥ��
            m_playGameUI.ShowFillContentUI(m_curChar, m_curAction);

            //4. ������ �׼��� Ÿ�� ������Ʈ ���� ǥ��
            GamePlayMaster.g_instance.EmphasizeTargetObject(m_curChar.GetXIndex(), m_curChar.GetYIndex(), m_curAction); //�⺻ �̵� �Ÿ� ���� 

            return;
        }
        if (m_curStep.Equals(GamePlayStep.PlayAction))
        {
            m_playGameUI.OffPlayUI();
            return;
        }
    }

}
