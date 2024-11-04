using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBookTileAction
{
    public void ConductTileAction(TokenTile _tile, TokenTileAction _action)
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
            case TileActionType.UIOpen:
                OpenUIByCode((UICodeEnum)subValue, _tile);
                    break;

            case TileActionType.WorkOrder:
                MakeWorkOrder(_tile, _action, subValue);
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

    public TItemListData GetTileChangeCost(TileType _tileType)
    {
        TItemListData costData = MgMasterData.GetInstance().GetTileData((int)_tileType).BuildCostData;
        return costData;
    }

    private void BuildTile(TokenTile _tile, TileType _tileType)
    {
        if (PlayerCapitalData.g_instance.CheckInventory(GetTileChangeCost(_tileType)) == true)
        {
            _tile.ChangePlace(_tileType);
        }
    }

    private void UseTownFunction(TokenTile _tile, int _subValue)
    {
        TownFuction townFuction = (TownFuction)_subValue;
        //Debug.Log(townFuction + "�۾� ���� ��û");
        switch (townFuction)
        {
            case TownFuction.GiveMoney:
                OpenGiveMoneyUI(_tile);
                break;
            case TownFuction.StudyAction:
                MgUI.GetInstance().ShowStudyInfo(_tile);
                break;
        }
    }

    private void OpenGiveMoneyUI(TokenTile _tile)
    {
        List<TOrderItem> itemList = PlayerCapitalData.g_instance.GetItemList();
        SelectItemInfo selectItemInfo = new SelectItemInfo(itemList, false, 1, itemList.Count, true); //�ڿ���δ� �ּ�1, �ִ� �ڿ�����ŭ
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

    private void OpenUIByCode(UICodeEnum _uiCode, TokenTile _tile)
    {
        switch (_uiCode)
        {
            case UICodeEnum.Temple:
                MgUI.GetInstance().ShowTemple(_tile);
                break;

            case UICodeEnum.Guild:
                MgUI.GetInstance().ShowGuildInfo();
                break;
            default:
                Debug.Log("���� ����");
                break;
        }
    }

    public enum TownFuction
    {
        GiveMoney = 1, StudyAction
    }

    private void MakeWorkOrder(TokenTile _tile, TokenTileAction _tileAction, int _workCode)
    {
        WorkOrder spawnOrder = new WorkOrder(null, 100, _workCode, WorkType.Spawn);
        _tile.RegisterWork(spawnOrder);
    }

}
