using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBookTileAction
{
    public void ConductTileAction(TokenTile _tile, TokenTileAction _action)
    {
     
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        int subValue = _action.GetStat(TileActionStat.SubValue); //�ش� Ÿ�Կ��� �������� ����
        int Value = _action.GetStat(TileActionStat.Value);
        //tileActionType���� ���� ���� 

        switch (tileActionType)
        {
           
            case TileActionType.UIOpen:
                OpenUIByCode((UICodeEnum)subValue, _tile);
                    break;
            case TileActionType.WorkOrder:
                //���� ������ �ϰ��� �ٸ� �۾� ���Ǹ� ���ؼ� �߰� ���� �� �ʿ�
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
        //tileActionType���� ���� ���� 

        switch (tileActionType)
        {

            case TokenType.UIOpen:
                OpenUIByCode((UICodeEnum)subValue, _tile);
                break;
            case TokenType.MonsterNationSpawn:
                //���� ������ �ϰ��� �ٸ� �۾� ���Ǹ� ���ؼ� �߰� ���� �� �ʿ�
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
            case UICodeEnum.GiveMoney:
                OpenGiveMoneyUI(_tile);
                break;
            case UICodeEnum.StudyAction:
                MgUI.GetInstance().ShowStudyInfo(_tile);
                break;
            default:
                Debug.Log("���� ����");
                break;
        }
    }

    private void MakeWorkOrder(TokenTile _tile, WorkType _workType, int _value)
    {
        new WorkOrder(null, 100, _tile, _value, _workType);
    }

}
