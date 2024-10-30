using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyLevelUp : NationPolicy
{
    
    public override void MakePlan()
    {
       // Debug.Log("레벨 계획");
        TokenBase planToken = m_nation.GetCapital();
        SetPlanToken(planToken);
        SetDonePlan(true);
    }

    public override void WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(1).BuildCostData;
        WorkOrder levelUpOrder = new WorkOrder(changeCost.GetItemList(), 100, m_planIndex, WorkType.NationLvUp);
        m_workOrder = levelUpOrder;
    }

    public override void Excute()
    {
        Debug.Log("레벨 집행");
        if (LevelUp() == true)
            DoneExcute();
    }

    private bool LevelUp()
    {
        Announcer.Instance.AnnounceState("국가 레벨 상승 :" + m_nation.GetNationLevel() + "Lv");
    
        m_nation.LevelUp();
        return true;
    }


}

