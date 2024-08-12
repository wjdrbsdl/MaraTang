using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class SelectItemInfo
{
    public bool IsFixedValue = false;
    public int SerialNum = FixedValue.No_INDEX_NUMBER; //사용안할때는 none 넘버로 
    public ITradeCustomer Giver;
    public ITradeCustomer Taker;
    public List<TOrderItem> ItemList; //보여주려는 아이템리스트
    public List<int> SelectedIndex; //선택한 리스트
    public List<int> SelectedValue; //선택한 수량
    public UISelectItem SelectUI; //선택 입력을 받을 UI
    private Action ConfirmAction;

    #region 클래스 생성부분
    public SelectItemInfo(List<TOrderItem> _showList, bool _isFixedValue)
    {
        //만들려는 아이템 리스트를가지고 클래스 생성
        ItemList = _showList;

        IsFixedValue = _isFixedValue; //고정 벨류면 선택한 아이템의 최종 수량으로 아니면 선택한 값으로 
        SelectedIndex = new List<int>(); //선택한 인덱스
        SelectedValue = new List<int>(); //선택한 수량 - FixedValue가 false일때만 입력받음. 
    }

    public void SetAction(Action _action)
    {
        ConfirmAction = _action;
    }

    public void SetSerial(int _serialNum)
    {
        SerialNum = _serialNum;
    }

    public void SetGiver(ITradeCustomer _giver)
    {
        Giver = _giver;
    }

    public void SetTaker(ITradeCustomer _taker)
    {
        Taker = _taker;
    }
    #endregion

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

    #region 정보 가져오기

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

    #endregion

    #region 확인 취소
    public void Confirm()
    {
        if (ConfirmAction != null)
            ConfirmAction();

        List<TOrderItem> selectedList = GetSelectList();

        TOrderItem confirmItem = new TOrderItem(TokenType.Conversation, (int)ConversationEnum.Response, (int)ResponseEnum.Check); //확인용 item 생성
        selectedList.Add(confirmItem);
        for (int i = 0; i < selectedList.Count; i++)
        {
            MGContent.GetInstance().SendActionCode(selectedList[i]);
        }
        
    }

    public void Cancle()
    {
        TOrderItem cancleResponse = new TOrderItem(TokenType.Conversation, (int)ConversationEnum.Response, (int)ResponseEnum.Cancle); //확인용 item 생성
        MGContent.GetInstance().SendActionCode(cancleResponse);
    }
    #endregion

    public void ShowScript()
    {
        //보여주기 방식
        //MgUI.GetInstance().등으로 자기가 보여주려는 방식으로 진행
         MgUI.GetInstance().ShowTextSelectList(this);
    }

    #region 선택 콜백
    public void OnSelectCallBack(int _slotIndex)
    {
        Debug.Log("셀렉인포 클래스를 통해서 선택 콜백");
        AddChooseItem(_slotIndex);
        //다시 정보 리셋 
        SelectUI.ResetSlot();
    }

    public void OnChangeValueCallBack(int _slotIndex, int _value)
    {
        Debug.Log("셀렉인포 클래스를 통해서 변화 콜백");
        int max = ItemList[_slotIndex].Value; //기존의 값이 최댓값
        int min = 1;
        int final = Mathf.Clamp(_value, min, max);
        if (final != _value)
        {
            //입력된 값이 다르면, 입력된값을 변경 시킴
            SelectUI.SetSelectValue(_slotIndex, final); //직접 text 변경한건 재 콜백이 일어나지 않음.
        }
        SetSelectValue(_slotIndex, final);
    }
    #endregion
}
