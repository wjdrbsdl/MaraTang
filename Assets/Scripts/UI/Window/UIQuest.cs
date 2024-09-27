using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIQuest : UIBase
{
    public TMP_Text m_conditionText;
    public TMP_Text m_rewardText;

    public void SetQuestInfo(Quest quest)
    {
        UISwitch(true);
        SetConditionInfo(quest);
        SetRewardInfo(quest);
    }

    private void SetConditionInfo(Quest quest)
    {
        string conStr = "�����������ͷ� �ٽ� ���� ������ �ʿ� �ִ�.";
        CurrentStageData stageInfo = quest.CurStageData;
        TOrderItem condition = stageInfo.SuccesConList[0];
        TOrderItem cur = stageInfo.CurConList[0];
        m_conditionText.text = quest.SerialNum+"�ø���"+ condition.Tokentype + "�� " + ((Capital)condition.SubIdx) + " ��" + condition.Value + "��ŭ\n"+
            cur.Tokentype+" :"+cur.SubIdx+":"+cur.Value;
    }

    private void SetRewardInfo(Quest quest)
    {
        string conStr = "�����������ͷ� �ٽ� ���� ������ �ʿ� �ִ�.";
        m_rewardText.text = conStr;
    }
}
