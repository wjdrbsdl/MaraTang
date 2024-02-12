using System;
using System.Collections;
using UnityEngine;


public class ShowcaseSlot : SlotBase
{
    public InputSlot _callBackAction; //콜백받을 곳
    private string m_testText;

    public void ShowCaseSet(string text, InputSlot _backClass)
    {
        m_testText = text;
        _callBackAction = _backClass;
    }

    public override void OnLeftClick()
    {
        MgUI.GetInstance().CancleLastUI();
        _callBackAction.SelectItem(m_testText);
    }
}
