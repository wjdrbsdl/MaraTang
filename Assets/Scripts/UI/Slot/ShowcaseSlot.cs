using System;
using System.Collections;
using UnityEngine;


public class ShowcaseSlot : SlotBase
{
    private UIShowcase m_shocaseUI; //콜백받을 곳
    public string m_testText;

    public void ShowCaseSet(string text, UIShowcase _shocaseUI)
    {
        m_testText = text;
        m_shocaseUI = _shocaseUI;
    }

    public override void OnLeftClick()
    {
        m_shocaseUI.SelectedSlot(this);
    }
}
