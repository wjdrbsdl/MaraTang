using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSlot : SlotBase
{
    private TokenEvent _eventToken;

    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        _eventToken = (TokenEvent)_token;
    }

    public override void ClearSlot()
    {
        base.ClearSlot();
        _eventToken = null;
    }

    public override void OnLeftClick()
    {
        PlayerManager.g_instance.SelectEventToken(_eventToken); //ÀÌº¥Æ® ½½¶ù ¿ÞÅ¬¸¯
    }
}
