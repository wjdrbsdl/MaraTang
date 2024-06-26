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
            Debug.Log("�ش� Ÿ�Ͽ� �־�� ����");
            return;
        }
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        int subValue = _action.GetStat(TileActionStat.SubValue); //�ش� Ÿ�Կ��� �������� ����
        //tileActionType���� ���� ���� 

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
                //��� ��ȯ 
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
            //  Debug.Log(mineResult[i].Item1 + " �ڿ� ä��" + mineResult[i].Item2);
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
        Debug.Log(townFuction + "�۾� ���� ��û");
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
        //�÷��̾ ������ �ڿ��� ���� ������ �ڽ�Ʈ�����͸� ����
        List<(Capital, int)> _capitalList = new();
        _capitalList.Add((Capital.Food, 50));
        _capitalList.Add((Capital.Mineral, 50));
        _capitalList.Add((Capital.Person, 50));
        OrderCostData _costData = _playerCapital.GetTradeOrder(_capitalList);

        Nation targetNation = _tile.GetNation();
        _playerCapital.PayCostData(_costData);
        targetNation.PayCostData(_costData, false); //��°ɷ� ���� 
    }

    public enum TownFuction
    {
        GiveMoney = 1
    }

}
