using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyExpandLand : NationPolicy
{
    
    public override void MakePlan()
    {
   //     Debug.Log("확장 계획");
        FindExpandLand();
    }

    public override WorkOrder WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(1).BuildCostData;
        WorkOrder expandOrder = new WorkOrder(changeCost.GetItemList(), 100, m_planIndex, WorkType.ExpandLand);
        TokenTile workTile = (TokenTile)m_planToken;
        workTile.RegisterWork(expandOrder);
        return expandOrder;
    }

    private void FindExpandLand()
    {
        int findExpandCount = 1; //3개씩 확장하는걸로 
        for (int i = 1; i <= m_nation.GetNationLevel(); i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_nation.GetCapital().GetMapIndex(), i);
            //2. 무소속이면 대상 토지를 편입. 
            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
                //무국적 땅이 아니면 넘김
                if (tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER) == false)
                    continue;

                //이미 공사중인 곳은 패스
                if (tile.IsBuilding() == true)
                    continue;

                TokenBase planToken = tile; //확장가능한 땅이면 타겟 지정
                SetPlanToken(planToken); //정책 대상으로 넣고
                SetPlanIndex(m_nationNum);
                SetDonePlan(true);
                findExpandCount -= 1;
                if (findExpandCount.Equals(0))
                {
                    break;
                }

            }

            if (findExpandCount.Equals(0))
            {
                break;
            }
        }
    }

    public bool ExpandLand(TokenTile _targetTile, int _nationNum)
    {
        Debug.LogWarning("확장 비용 정산 필요");
        Nation targetNation = MgNation.GetInstance().GetNation(_nationNum);
        if(targetNation == null)
        {
            Debug.Log("사라진 국가");
            return false;
        }
        targetNation.AddTerritory(_targetTile); //계획 토큰을 타일로 전환후 영토 집행
        targetNation.ShowTerritory();
        return true;
    }

    public bool AbleExpand(TokenTile _tile)
    {
        //만약 현재 타일상태가 누군가의 점유로 바꼈으면 확장 불가 
        if (_tile.GetStat(TileStat.Nation) != FixedValue.NO_NATION_NUMBER)
        {
            //   Debug.Log("미점유상태 타일이 아님");
            return false;
        }
        
        //비용 코스트 check필요

        return true;
    }
}

