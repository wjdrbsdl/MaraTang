using System;
using System.Collections;
using UnityEngine;


public class ShowcaseSlot : SlotBase
{
    private UIShowcase m_shocaseUI; //콜백받을 곳

    public void ShowCaseSet(TokenBase _capitalToken, UIShowcase _shocaseUI)
    {
        SetSlot(_capitalToken);
        m_shocaseUI = _shocaseUI;
        
    }

    public override void OnLeftClick()
    {
        m_shocaseUI.SelectedSlot(this);
    }
}
