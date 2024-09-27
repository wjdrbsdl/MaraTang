using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIBase
{
    private Nation m_curVisitNation;
    private Quest m_curQuest; //최신 만들어준 길드 퀘스트

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

        m_curQuest = MGContent.GetInstance().RequestGuildQuest(); //뼈대 받음
        ReviseQuestCondtion(); //세부내용 조정
        //플레이어에게 해당 퀘스트 할건지 제안 
        SuggestToPlayer();
        //수락 받았다고 가정
        ReactNewQuest(true);
    }

    private void ReviseQuestCondtion()
    {
        m_curQuest.SerialNum = Random.Range(0, 100);
        //성공 조건 값 조정
        CurrentStageData curCondition = m_curQuest.CurStageData;
        TOrderItem newCondition = curCondition.SuccesConList[0];
        newCondition.SubIdx = 2;
        newCondition.Value = Random.Range(0, 100);
        curCondition.SuccesConList[0] = newCondition;

        //현재 조건 값 갱신
        m_curQuest.CurStageData.ResetCurCondtion();
    }

    private void SuggestToPlayer()
    {
        //퀘스트 정보와 수락여부 확인할 창을 팝업 
    }

    public void ReactNewQuest(bool _like)
    {
        //제안한 퀘스트에 플레이어의 수락여부
        if (_like)
        {
            MGContent.GetInstance().RealizeQuest(m_curQuest);
        }
    }
}
