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

    public override void WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(1).BuildCostData;
        WorkOrder expandOrder = new WorkOrder(changeCost.GetItemList(), 100);
        m_workOrder = expandOrder;
    }

    public override void Excute()
    {
        Debug.Log("확장 집행");
        if (ExpandLand() == true)
            DoneExcute();
    }

    private void FindExpandLand()
    {
        int findExpandCount = 1; //3개씩 확장하는걸로 
        for (int i = 1; i <= m_nation.GetNationLevel(); i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_nation.GetCapital().GetXIndex(), m_nation.GetCapital().GetYIndex(), i);
            //2. 무소속이면 대상 토지를 편입. 
            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
                //무국적 땅이 아니면 넘김
                if (tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER) == false)
                    continue;

                //정책 대상이면 넘김
                if (tile.GetPolicy() != null)
                    continue;

                TokenBase planToken = tile; //확장가능한 땅이면 타겟 지정
                SetPlanToken(planToken); //정책 대상으로 넣고
                tile.SetPolicy(this);
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

    private bool ExpandLand()
    {
        TokenTile planTile = (TokenTile)m_planToken;
        if (AbleExpand(planTile) == false)
        {
            //  Debug.Log("확장 불가능 상태");
            return false;
        }

        //확장 가능한 상태라면
        //Debug.Log("영토 확장 정책 수행 완료");
        //비용 처리 필요
        Debug.LogWarning("확장 비용 정산 필요");
        m_nation.AddTerritory(planTile); //계획 토큰을 타일로 전환후 영토 집행
        m_nation.ShowTerritory();
        return true;
    }

    private bool AbleExpand(TokenTile _tile)
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

