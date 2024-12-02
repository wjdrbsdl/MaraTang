using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBookTileAction
{
    public void ConductTileAction(TokenTile _tile, TOrderItem _actionOrder, TileType _actionPlace)
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
            case TokenType.MonsterSpawn:
                MonsterSpawn(_tile, subValue, Value);
                break;
            case TokenType.NationStat:
                NationStatUp(_tile, subValue, Value);
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

    private void MonsterSpawn(TokenTile _spawnTile, int _monsterPid, int _monsterCount)
    {
        for (int i = 0; i < _monsterCount; i++)
        {
            MgToken.GetInstance().SpawnCharactor(_spawnTile.GetMapIndex(), _monsterPid);
        }
    }

    private void NationStatUp(TokenTile _statTile, int _nationStatEnum, int _value)
    {
        Nation nation = _statTile.GetNation();
        nation.CalStat((NationStatEnum)_nationStatEnum, _value);
        Debug.Log(nation.GetNationNum() + "국가 " + (NationStatEnum)_nationStatEnum + "스텟 " + _value + "적용 \n" + nation.GetStat((NationStatEnum)_nationStatEnum));

    }
 }
