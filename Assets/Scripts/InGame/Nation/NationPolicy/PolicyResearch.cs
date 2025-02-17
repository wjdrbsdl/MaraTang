﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyResearch : NationPolicy
{
    
    public override void MakePlan()
    {
      //  Debug.Log("연구 계획");

        SelectTechTree();
    }

    public override WorkOrder WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        NationTechData techData = MgMasterData.GetInstance().GetTechData(m_planIndex);
        TItemListData changeCost = techData.ResearchCostData;
        TokenTile workTile = (TokenTile)m_planToken;
      //  Debug.Log(m_nation.GetNationNum()+"에서"+ (TechEnum)techData.GetPid() + "연구 시작");
        WorkOrder researchOrder = new WorkOrder(changeCost.GetItemList(), techData.NeedTurn, techData.NeedLabor, workTile, m_planIndex, WorkType.Research);
        return researchOrder;
    }

    private void SelectTechTree()
    {
        //다음 연구할 기술을 선택. 
        TechTreeSelector treeManager = new(); //매니저 생성하고 
        int reserachIndex = treeManager.GetTechPidByNotDone(m_nation.TechPart.GetDoneTechList());
        //연구할게 없으면 종료 
        if (reserachIndex == FixedValue.No_INDEX_NUMBER)
            return;

        List<TokenTile> territory = m_nation.GetTerritorry();
        TokenTile labTile = null;
        for (int i = 0; i < territory.Count; i++)
        {
            TokenTile tile = territory[i];
            //연구소찾기
            if (tile.GetTileType() != TileType.Lab)
                continue;

            //빈 작업 찾기
            if (tile.GetWorkOrder() != null)
                continue;

            labTile = tile;
        }
        if (labTile == null)
        {
            //Debug.Log("국가에 랩실이 없음");
            return;
        }

        SetPlanToken(labTile); //연구의 집행은 일단 본진 - 이후 연구소 건물에서? 
        SetPlanIndex(reserachIndex);
        SetDonePlan(true);
        // Debug.Log("다음 연구 테크pid는" + m_planIndex + "로 결정");
    }

    public void CompleteResearch(TokenTile _workTile, int _techPid)
    {
        Nation nation = _workTile.GetNation();
        if (nation == null)
            return;

        nation.TechPart.CompleteTech(_techPid);
    }

}

