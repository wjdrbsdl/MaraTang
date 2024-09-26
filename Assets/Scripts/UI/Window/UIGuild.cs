using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIBase
{
    private Nation m_curVisitNation;

   public void SetGuildInfo()
    {
        UISwitch(true);
        m_curVisitNation = GameUtil.GetTileTokenFromMap(PlayerManager.GetInstance().GetMainChar().GetMapIndex()).GetNation();
        GuildCard card = PlayerManager.GetInstance().GetMainChar().GetGuildCard();
        Debug.Log("계급 "+card.GetGuildGrade());
    }

    public void MakeGuildQuest()
    {
        //길드 임무 생성
        //1. 나라 부족한 자원
        //2. 나라 주변 몬스터
        //3. 그냥 현재 진행상태에서 자원, 몬스터를 찾고 물리쳐달라는 퀘스트를 생성. 
        Debug.Log("길드퀘스트 생성");
        Quest guildQuest = MGContent.GetInstance().RequestGuildQuest();
        ConditionChecker curCondition = guildQuest.CurStageData;
        for(int i = 0; i < curCondition.SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = curCondition.SuccesConList[i];
            Debug.Log(conditionItem.Tokentype +" : "+ conditionItem.SubIdx + " "+ conditionItem.Value );
        }

    }
}
