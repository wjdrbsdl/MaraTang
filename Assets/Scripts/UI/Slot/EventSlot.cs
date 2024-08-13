using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSlot : SlotBase
{
    private TokenEvent _eventToken;
    private UIBase _openUI; //소속된곳
    public TTokenOrder TokenOrder;
    public int OrderIdx;
    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        _eventToken = (TokenEvent)_token;
    }

    public void SetSlot(TTokenOrder _order, int _index, UIBase _ui)
    {
        TokenOrder = _order;
        OrderIdx = _index;
        _openUI = _ui;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        _eventToken = null;
    }

    public override void OnLeftClick()
    {
        _openUI.UISwitch(false);
        OrderExcutor orderExcutor = new();
        orderExcutor.ExcuteOrderItem(TokenOrder, OrderIdx);
        Debug.LogWarning("임시로 슬롯 선택시에 응답 0번 호출. 선택 수에 따른 선택 미비, 확인 전달 구현 필요");
        MGContent.GetInstance().SendActionCode(new TOrderItem(TokenType.Conversation, (int)ConversationEnum.Response, 0));
    }
}
