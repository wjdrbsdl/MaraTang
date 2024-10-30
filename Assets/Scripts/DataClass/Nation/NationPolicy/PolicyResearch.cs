using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyResearch : NationPolicy
{
    
    public override void MakePlan()
    {
        Debug.Log("연구 계획");

        SelectTechTree();
    }

    public override WorkOrder WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(1).BuildCostData;
        WorkOrder researchOrder = new WorkOrder(changeCost.GetItemList(), 100, m_planIndex, WorkType.Research);
        m_workOrder = researchOrder;
        return researchOrder;
    }

    public override void Excute()
    {
        Debug.Log("연구 집행");
        if (Research() == true)
            DoneExcute();
    }


    private void SelectTechTree()
    {
        //다음 연구할 기술을 선택. 
        TechTreeSelector treeManager = new(); //매니저 생성하고 
        int planIndex = treeManager.GetTechPidByNotDone(m_nation.TechPart.GetTechList());
        SetPlanIndex(planIndex);
        SetDonePlan(true);
        // Debug.Log("다음 연구 테크pid는" + m_planIndex + "로 결정");
    }

    private bool Research()
    {
        if (AbleResearch(m_planIndex) == false)
        {
            return false;
        }


        m_nation.TechPart.CompleteTech(m_planIndex);

        return true;
    }

    private bool AbleResearch(int _techPid)
    {
        //마스터데이터에 없는 테크번호면 실패
        if (MgMasterData.GetInstance().GetTechData(_techPid) == null)
        {
            return false;
        }

        return true;
    }


}

