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
