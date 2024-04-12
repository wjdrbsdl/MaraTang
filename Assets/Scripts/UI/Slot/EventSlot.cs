using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSlot : SlotBase
{
    private TokenEvent _eventToken;
    private UIBase _openUI; //소속된곳
    public TTokenOrder TokenOrder;
    public TOrderItem OrderItem;
    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        _eventToken = (TokenEvent)_token;
    }

    public void SetSlot(TOrderItem _order, UIBase _ui)
    {
        OrderItem = _order;
        _openUI = _ui;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        _eventToken = null;
    }

    public override void OnLeftClick()
    {
        _openUI.Switch(false);
        OrderExcutor.ExcuteOrderItem(OrderItem);
    }
}
