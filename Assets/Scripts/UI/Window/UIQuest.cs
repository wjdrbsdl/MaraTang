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
        m_conditionText.text = m_currentQuest.SerialNum+"시리얼"+ condition.Tokentype + "의 " + ((Capital)condition.SubIdx) + " 를" + condition.Value + "만큼\n"+
            cur.Tokentype+" :"+cur.SubIdx+":"+cur.Value;
    }

    private void SetRewardInfo()
    {
        string conStr = "컨텐츠데이터로 다시 정보 세팅할 필요 있다.";
        m_rewardText.text = conStr;
    }

    private void SetClearBtn()
    {
        m_clearBtn.SetActive(m_currentQuest.CurStageData.AbleClear);
    }

    public void ReqeustClearQuest()
    {
        if(m_currentQuest.CurStageData.CheckSuccess()) //한번더 조건충족 체크 후
        m_currentQuest.ClearStage(); //진행
    }
}
