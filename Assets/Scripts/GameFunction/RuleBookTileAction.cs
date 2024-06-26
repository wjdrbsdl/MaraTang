using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBookTileAction
{
    public void ConductTileAction(TokenTile _tile, TokenAction _action)
    {
        TokenChar player = MgToken.GetInstance().GetMainChar();
        if (GameUtil.GetMinRange(player, _tile) > 0 && GamePlayMaster.GetInstance().AdaptInTileForAct == true)
        {
            Debug.Log("해당 타일에 있어야 가능");
            return;
        }
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        int subValue = _action.GetStat(TileActionStat.SubValue); //해당 타입에서 부차적인 벨류
        //tileActionType으로 행태 구별 

        switch (tileActionType)
        {
            case TileActionType.Harvest:
                HarvestTile(_tile);
                MgUI.GetInstance().CancleLastUI();
                break;
            case TileActionType.Build:
                BuildTile(_tile, (TileType)subValue);
                MgUI.GetInstance().CancleLastUI();
                break;

            case TileActionType.CapitalChef:
                CapitalAction doCode = (CapitalAction)subValue;
                //재료 변환 
                MgUI.GetInstance().ShowCapitalWorkShop(doCode, _tile, _action);
                break;

            case TileActionType.LandUsage:
                UseTownFunction(_tile, subValue);
                break;

            case TileActionType.Destroy:
                BuildTile(_tile, TileType.Nomal);
                MgUI.GetInstance().CancleLastUI();
                break;
            default:
                MgUI.GetInstance().CancleLastUI();
                break;
        }

    }

    private void HarvestTile(TokenTile _tile)
    {
        List<(Capital, int)> mineResult = GamePlayMaster.GetInstance().RuleBook.MineResource(_tile).GetResourceAmount();
        for (int i = 0; i < mineResult.Count; i++)
        {
            //  Debug.Log(mineResult[i].Item1 + " 자원 채취" + mineResult[i].Item2);
            PlayerCapitalData.g_instance.CalCapital(mineResult[i].Item1, mineResult[i].Item2);
        }
    }

    public OrderCostData GetTileChangeCost(TileType _tileType)
    {
        OrderCostData costData = MgMasterData.GetInstance().GetTileData((int)_tileType).BuildCostData;
        return costData;
    }

    private void BuildTile(TokenTile _tile, TileType _tileType)
    {
        if (PlayerCapitalData.g_instance.CheckInventory(GetTileChangeCost(_tileType)) == true)
        {
            _tile.ChangeTileType(_tileType);
        }
    }

    private void UseTownFunction(TokenTile _tile, int _subValue)
    {
        TownFuction townFuction = (TownFuction)_subValue;
        Debug.Log(townFuction + "작업 수행 요청");
        switch (townFuction)
        {
            case TownFuction.GiveMoney:
                GiveMoney(_tile);
                break;
        }
    }

    private void GiveMoney(TokenTile _tile)
    {

        return;

        PlayerCapitalData _playerCapital = PlayerCapitalData.g_instance;
        //플레이어가 지불할 자원과 수를 가지고 코스트데이터를 생성
        List<(Capital, int)> _capitalList = new();
        _capitalList.Add((Capital.Food, 50));
        _capitalList.Add((Capital.Mineral, 50));
        _capitalList.Add((Capital.Person, 50));
        OrderCostData _costData = _playerCapital.GetTradeOrder(_capitalList);

        Nation targetNation = _tile.GetNation();
        _playerCapital.PayCostData(_costData);
        targetNation.PayCostData(_costData, false); //얻는걸로 진행 
    }

    public enum TownFuction
    {
        GiveMoney = 1
    }

}
