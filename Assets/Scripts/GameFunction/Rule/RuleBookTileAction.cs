using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBookTileAction
{
    public void ConductTileAction(TokenTile _tile, TokenTileAction _action)
    {
     
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        int subValue = _action.GetStat(TileActionStat.SubValue); //해당 타입에서 부차적인 벨류
        int Value = _action.GetStat(TileActionStat.Value);
        //tileActionType으로 행태 구별 

        switch (tileActionType)
        {
           
            case TileActionType.UIOpen:
                OpenUIByCode((UICodeEnum)subValue, _tile);
                    break;
            case TileActionType.WorkOrder:
                //현재 스폰만 일괄중 다른 작업 정의를 위해선 추가 정보 가 필요
                MakeWorkOrder(_tile, (WorkType)subValue, Value);
                break;
            default:
                MgUI.GetInstance().CancleLastUI();
                break;
        }

    }

    public void ConductTileAction(TokenTile _tile, TOrderItem _actionOrder)
    {

        TokenType tileActionType = (TokenType)_actionOrder.Tokentype;
        int subValue = _actionOrder.SubIdx;
        int Value = _actionOrder.Value;
        //tileActionType으로 행태 구별 

        switch (tileActionType)
        {

            case TokenType.UIOpen:
                OpenUIByCode((UICodeEnum)subValue, _tile);
                break;
            case TokenType.MonsterNationSpawn:
                //현재 스폰만 일괄중 다른 작업 정의를 위해선 추가 정보 가 필요
                MakeWorkOrder(_tile, (WorkType)subValue, Value);
                break;
            default:
                MgUI.GetInstance().CancleLastUI();
                break;
        }

    }

    private void OpenGiveMoneyUI(TokenTile _tile)
    {
        List<TOrderItem> itemList = PlayerCapitalData.g_instance.GetItemList();
        SelectItemInfo selectItemInfo = new SelectItemInfo(itemList, false, 1, itemList.Count, true); //자원기부는 최소1, 최대 자원수만큼
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
            case UICodeEnum.GiveMoney:
                OpenGiveMoneyUI(_tile);
                break;
            case UICodeEnum.StudyAction:
                MgUI.GetInstance().ShowStudyInfo(_tile);
                break;
            default:
                Debug.Log("없는 오픈");
                break;
        }
    }

    private void MakeWorkOrder(TokenTile _tile, WorkType _workType, int _value)
    {
        new WorkOrder(null, 100, _tile, _value, _workType);
    }

}
