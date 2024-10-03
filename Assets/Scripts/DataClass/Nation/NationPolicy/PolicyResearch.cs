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

    public override void Excute()
    {
        Debug.Log("연구 집행");
        Research();
    }


    private void SelectTechTree()
    {
        //다음 연구할 기술을 선택. 
        TechTreeSelector treeManager = new(); //매니저 생성하고 
        int planIndex = treeManager.GetTechPidByNotDone(m_nation.TechPart.GetTechList());
        SetPlanIndex(planIndex);
        // Debug.Log("다음 연구 테크pid는" + m_planIndex + "로 결정");
    }

    private bool Research()
    {
        if (AbleResearch(m_planIndex) == false)
        {
            return false;
        }

        //   Debug.Log(m_planIndex + "번 테크 연구 완료");
        //1.비용내고
        TItemListData costData = MgMasterData.GetInstance().GetTechData(m_planIndex).ResearchCostData;
        m_nation.PayCostData(costData);
        //2.완료 기록하고
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
        TItemListData costData = MgMasterData.GetInstance().GetTechData(_techPid).ResearchCostData;
        return m_nation.CheckInventory(costData);
    }


}

