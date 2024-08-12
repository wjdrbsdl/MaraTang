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
  
        m_conditionText.text = conStr;
    }

    private void SetRewardInfo(Quest quest)
    {
        string conStr = "�����������ͷ� �ٽ� ���� ������ �ʿ� �ִ�.";
        m_rewardText.text = conStr;
    }
}
