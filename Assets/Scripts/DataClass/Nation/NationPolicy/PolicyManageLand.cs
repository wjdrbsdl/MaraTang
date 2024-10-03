using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyManageLand : NationPolicy
{
    
    public override void MakePlan()
    {
        Debug.Log("매니지 계획");
        FindManageLand();
    }

    public override void Excute()
    {
        Debug.Log("매니지 집행");
        ManageTerritory();
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
                int planIndex = Random.Range((int)TileType.WoodLand, (int)TileType.Capital); //벌목장부터 광산까지 중 뽑기 
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

        //변경 가능한 상태라면
        //  Debug.Log("영토 운영 정책 수행 완료");
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(m_planIndex).BuildCostData;
        m_nation.PayCostData(changeCost);
        planTile.ChangeTileType((TileType)m_planIndex); //플랜 idx 타입으로 토지변경
        return true;
    }

    private bool AbleManageLand(TokenTile _tile, int _nationNumber, int _planIndex)
    {
        //만약 현재 타일상태가 누군가의 점유로 바꼈으면 확장 불가 
        if (_tile.GetStat(TileStat.Nation) != _nationNumber)
        {
            //  Debug.Log("국가 귀속 타일이 아님");
            return false;
        }
        if (_tile.GetTileType() != TileType.Nomal)
        {
            //  Debug.Log("토지 변경 불가능한 상태");
            return false;
        }
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(_planIndex).BuildCostData;
        //  Debug.Log((TileType)m_planIndex + "로 변경하려는 중");
        if (m_nation.CheckInventory(changeCost) == false)
        {
            // Debug.Log("국가 단위 변경 비용 부족");
            return false;
        }

        return true;
    }
}

