using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class SelectItemInfo
{
    public bool IsFixedValue = false;
    public ITradeCustomer Giver;
    public ITradeCustomer Taker;
    public List<TOrderItem> ItemList; //보여주려는 아이템리스트
    public List<int> SelectedIndex; //선택한 리스트
    public List<int> SelectedValue; //선택한 수량
    private Action ConfirmAction;
    public SelectItemInfo(List<TOrderItem> _showList, bool _isFixedValue)
    {
        //만들려는 아이템 리스트를가지고 클래스 생성
        ItemList = _showList;
        
        IsFixedValue = _isFixedValue; //고정 벨류면 선택한 아이템의 최종 수량으로 아니면 선택한 값으로 
        SelectedIndex = new List<int>(); //선택한 인덱스
        SelectedValue = new List<int>(); //선택한 수량 - FixedValue가 false일때만 입력받음. 
    }

    public void AddChooseItem(int _itemIndex)
    {
        int inListIndex = SelectedIndex.IndexOf(_itemIndex);
        if (0 <= inListIndex)
        {
            //존재하던 거라면 빼기
            SelectedIndex.RemoveAt(inListIndex);
            SelectedValue.RemoveAt(inListIndex);
            return;
        }
        //없던거라면 인덱스 추가하고
        SelectedIndex.Add(_itemIndex);
        SelectedValue.Add(1); //최솟값으로 넣음
    }

    public void SetSelectValue(int _selectindex, int _value)
    {
        int inListIndex = SelectedIndex.IndexOf(_selectindex);
        SelectedValue[inListIndex] = _value;
    }

    public List<TOrderItem> GetSelectList()
    {
        List<TOrderItem> selectList = new List<TOrderItem>();
        for (int i = 0; i < SelectedIndex.Count; i++)
        {
            TOrderItem item = ItemList[SelectedIndex[i]]; //변경되는가 체크
            if(IsFixedValue == false)
            {
                //만약 변동 값기준이라면 변동된값으로 세팅해서 진행
                item.SetValue(SelectedValue[i]);
            }
            
            selectList.Add(item);
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
