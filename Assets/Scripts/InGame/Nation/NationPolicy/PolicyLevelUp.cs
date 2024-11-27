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

    public override WorkOrder WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(1).BuildCostData;
        TokenTile workTile = (TokenTile)m_planToken;
        WorkOrder levelUpOrder = new WorkOrder(changeCost.GetItemList(),0, 100, workTile, m_planIndex, WorkType.NationLvUp);
        return levelUpOrder;
    }

    public bool LevelUp(TokenTile _tile)
    {
        Nation nation = _tile.GetNation();
        if(nation == null)
        {
            Debug.Log("사라진 국가");
            return false;
        }
            
        Announcer.Instance.AnnounceState("국가 레벨 상승 :" + nation.GetNationLevel() + "Lv");
    
        nation.LevelUp();
        return true;
    }


}

