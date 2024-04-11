using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSlot : SlotBase
{
    private TokenEvent _eventToken;
    private UIBase _openUI; //�ҼӵȰ�
    public TTokenOrder RewardInfo;
    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        _eventToken = (TokenEvent)_token;
    }

    public void SetSlot(TTokenOrder _rewardInfo, UIBase _ui)
    {
        RewardInfo = _rewardInfo;
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
        MGContent.GetInstance().SelectReward(RewardInfo);
    }
}
