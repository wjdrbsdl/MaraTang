using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PolicyBuildResrouceTile : NationPolicy
{
    
    public override void MakePlan()
    {
//        Debug.Log("매니지 계획");
        FindManageLand();
    }

    public override WorkOrder WriteWorkOrder()
    {
        //임시 벌목장 건설에 필요한 코스트로 진행
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(1).BuildCostData;

        List<TOrderItem> nothing = new();
        //해당 타일을 자본타일로 바꾸는 작업서
        WorkOrder manageOrder = new WorkOrder(nothing, 100, m_planIndex, WorkType.ChangeBuild);
        TokenTile workTile = (TokenTile)m_planToken;
        Action doneEffect = delegate
        {
            workTile.CompleteOutBuild((TileType)m_planIndex);
        };
        manageOrder.SetDoneEffect(doneEffect);
        workTile.RegisterWork(manageOrder);
        m_workOrder = manageOrder;
        return manageOrder;
    }

    private void FindManageLand()
    {
        int endRange = m_nation.GetNationLevel(); //최종 살펴볼 위치
        for (int i = 1; i <= endRange; i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_nation.GetCapital().GetXIndex(), m_nation.GetCapital().GetYIndex(), i);

            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
                //만약 내땅이 아니면 일단 패스 
                if (tile.GetStat(TileStat.Nation).Equals(m_nation.GetNationNum()) == false)
                    continue;

                //이미 정책 대상 토지면 패스
                if (tile.GetPolicy() != null)
                    continue;

                //용도가 노말이 아니면 패스 
                if (tile.GetTileType().Equals(TileType.Nomal) == false)
                    continue;

                //내땅중에 용도가 nomal인 땅을 찾았다면
                TokenBase planToken = tile;
                int planIndex = UnityEngine.Random.Range((int)TileType.WoodLand, (int)TileType.Capital); //벌목장부터 광산까지 중 뽑기 
                SetDonePlan(true);
                SetPlanToken(planToken);
                SetPlanIndex(planIndex);
                tile.SetPolicy(this);
                return;
            }

        }
    }

    private bool ManageTerritory()
    {
        TokenTile planTile = (TokenTile)m_planToken;
        if (AbleManageLand(planTile, m_planIndex, m_nation.GetNationNum()) == false)
        {
            //   Debug.Log("운영 불가능 상태");
            return false;
        }

         planTile.ChangePlace((TileType)m_planIndex); //플랜 idx 타입으로 토지변경
        return true;
    }

    private bool AbleManageLand(TokenTile _tile, int _planIndex, int _nationNumber)
    {
        //만약 현재 타일상태가 누군가의 점유로 바꼈으면 확장 불가 
        if (_tile.GetStat(TileStat.Nation) != _nationNumber)
        {
            Debug.Log(_tile.GetStat(TileStat.Nation)+"토지 번호 "+ _nationNumber+"국가 번호가 다름"+
                _tile.GetMapIndex()[0]+":"+_tile.GetMapIndex()[1]);
            return false;
        }
        if (_tile.GetTileType() != TileType.Nomal)
        {
            Debug.Log("토지 변경 불가능한 상태");
            return false;
        }
        return true;
    }
}

