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
        Debug.Log("��� "+card.GetGuildGrade());
    }

    public void MakeGuildQuest()
    {
        //��� �ӹ� ����
        //1. ���� ������ �ڿ�
        //2. ���� �ֺ� ����
        //3. �׳� ���� ������¿��� �ڿ�, ���͸� ã�� �����Ĵ޶�� ����Ʈ�� ����. 
        Debug.Log("�������Ʈ ����");

        Quest guildQuest = MGContent.GetInstance().RequestGuildQuest();
        m_curQuest = guildQuest;
        StageMasterData stage = MgMasterData.GetInstance().GetStageData(guildQuest.ContentPid, guildQuest.CurStep);
        guildQuest.CurStageData = new ConditionChecker(stage);
        ConditionChecker curCondition = guildQuest.CurStageData;
        TOrderItem newCondition = curCondition.SuccesConList[0];
        newCondition.SubIdx = 2;
        curCondition.SuccesConList[0] = newCondition;
        for(int i = 0; i < curCondition.SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = curCondition.SuccesConList[i];
            Debug.Log(conditionItem.Tokentype +" : "+ conditionItem.SubIdx + " "+ conditionItem.Value );
        }

        MGContent.GetInstance().RealizeQuest(m_curQuest);
    }
}
