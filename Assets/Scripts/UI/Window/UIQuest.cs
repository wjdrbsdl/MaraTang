using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIQuest : UIBase
{
    public TMP_Text m_conditionText;
    public TMP_Text m_rewardText;
    public GameObject m_clearBtn;
    private Quest m_currentQuest;
    public void SetQuestInfo(Quest quest)
    {
        UISwitch(true);
        m_currentQuest = quest;
        SetConditionInfo();
        SetRewardInfo();
        SetClearBtn();
    }

    private void SetConditionInfo()
    {
        CurrentStageData stageInfo = m_currentQuest.CurStageData;
        TOrderItem condition = stageInfo.SuccesConList[0];
        TOrderItem cur = stageInfo.CurConList[0];
        m_conditionText.text = m_currentQuest.SerialNum+"�ø���"+ condition.Tokentype + "�� " + ((Capital)condition.SubIdx) + " ��" + condition.Value + "��ŭ\n"+
            cur.Tokentype+" :"+cur.SubIdx+":"+cur.Value;
    }

    private void SetRewardInfo()
    {
        string conStr = "�����������ͷ� �ٽ� ���� ������ �ʿ� �ִ�.";
        m_rewardText.text = conStr;
    }

    private void SetClearBtn()
    {
        m_clearBtn.SetActive(m_currentQuest.CurStageData.AbleClear);
    }

    public void ReqeustClearQuest()
    {
        if(m_currentQuest.CurStageData.CheckSuccess()) //�ѹ��� �������� üũ ��
        m_currentQuest.ClearStage(); //����
    }
}
