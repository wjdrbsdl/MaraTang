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
        string conStr = "컨텐츠데이터로 다시 정보 세팅할 필요 있다.";
  
        m_conditionText.text = conStr;
    }

    private void SetRewardInfo(Quest quest)
    {
        string conStr = "컨텐츠데이터로 다시 정보 세팅할 필요 있다.";
        m_rewardText.text = conStr;
    }
}
