using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIBase
{
    private Nation m_curVisitNation;
    private Quest m_curQuest;

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

        m_curQuest = MGContent.GetInstance().RequestGuildQuest();
        m_curQuest.SerialNum = Random.Range(0, 100);
        CurrentStageData curCondition = m_curQuest.CurStageData;
        TOrderItem newCondition = curCondition.SuccesConList[0];
        newCondition.SubIdx = 2;
        newCondition.Value = Random.Range(0, 100);
        curCondition.SuccesConList[0] = newCondition;

        MGContent.GetInstance().RealizeQuest(m_curQuest);
    }
}
