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
        ConditionChecker curCondition = guildQuest.CurStageData;
        for(int i = 0; i < curCondition.SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = curCondition.SuccesConList[i];
            Debug.Log(conditionItem.Tokentype +" : "+ conditionItem.SubIdx + " "+ conditionItem.Value );
        }

    }
}
