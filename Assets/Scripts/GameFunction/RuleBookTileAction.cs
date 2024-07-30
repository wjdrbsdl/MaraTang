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

    public TItemListData GetTileChangeCost(TileType _tileType)
    {
        TItemListData costData = MgMasterData.GetInstance().GetTileData((int)_tileType).BuildCostData;
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
        //Debug.Log(townFuction + "작업 수행 요청");
        switch (townFuction)
        {
            case TownFuction.GiveMoney:
                OpenSelectUI(_tile);
                break;
        }
    }

    private void OpenSelectUI(TokenTile _tile)
    {
        List<TOrderItem> itemList = PlayerCapitalData.g_instance.GetItemList();
        SelectItemInfo selectItemInfo = new SelectItemInfo(itemList, false);
        selectItemInfo.SetGiver(PlayerCapitalData.g_instance);
        selectItemInfo.SetTaker(_tile.GetNation());
        Action confirmAction = delegate
        {
            GiveMoney(selectItemInfo);
        };
        selectItemInfo.SetAction(confirmAction);
        MgUI.GetInstance().ShowIconSelectList(selectItemInfo);
        return;
    }

    private void GiveMoney(SelectItemInfo _selectInfo)
    {
        List<TOrderItem> selectItem = _selectInfo.GetSelectList(); ;
        TItemListData costData = new TItemListData(selectItem);
        _selectInfo.Giver.PayCostData(costData);
        _selectInfo.Taker.PayCostData(costData, false);
        
    }

    private void TempSaySentence()
    {
        MGConversation.GetInstance().ShowSelectScript(ConversationEnum.Choose, 1, 4);
    }

    public enum TownFuction
    {
        GiveMoney = 1
    }

}
