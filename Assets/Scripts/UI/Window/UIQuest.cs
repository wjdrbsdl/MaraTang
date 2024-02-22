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
        SetConditionInfo(quest.condition);
        SetRewardInfo(quest.reward);
    }

    private void SetConditionInfo(QuestCondition _condition)
    {
        string conStr = "";
        if (_condition.monsterCount > 0)
        {
            conStr += _condition.monsterPID + " 몬스터 " + _condition.monsterCount + " 처치";
        }
        m_conditionText.text = conStr;
    }

    private void SetRewardInfo(RewardData _reward)
    {
        string conStr = _reward.RewardType.ToString();
        m_rewardText.text = conStr;
    }
}
