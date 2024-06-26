using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SelectItemInfo
{
    public ITradeCustomer Giver;
    public ITradeCustomer Taker;
    public List<TOrderItem> ItemList; //보여주려는 아이템리스트
    public List<int> SelectedIndex; //선택한 리스트
    private Action ConfirmAction;
    public SelectItemInfo(List<TOrderItem> _showList)
    {
        //만들려는 아이템 리스트를가지고 클래스 생성
        ItemList = _showList;
        SelectedIndex = new List<int>();
    }

    public void AddChooseItem(int _itemIndex)
    {
        if (SelectedIndex.IndexOf(_itemIndex) >= 0)
        {
            SelectedIndex.Remove(_itemIndex);
            return;
        }
        SelectedIndex.Add(_itemIndex);
    }

    public List<TOrderItem> GetSelectList()
    {
        List<TOrderItem> selectList = new List<TOrderItem>();
        for (int i = 0; i < SelectedIndex.Count; i++)
        {
            selectList.Add(ItemList[SelectedIndex[i]]);
        }
        return selectList;
    }

    public void SetAction(Action _action)
    {
        ConfirmAction = _action;
    }

    public void SetGiver(ITradeCustomer _giver)
    {
        Giver = _giver;
    }

    public void SetTaker(ITradeCustomer _taker)
    {
        Taker = _taker;
    }

    public void Confirm()
    {
        if (ConfirmAction != null)
            ConfirmAction();
    }
}
