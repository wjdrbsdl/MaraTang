using System;
using System.Collections;
using UnityEngine;


public class ShowcaseSlot : SlotBase
{
    private UIShowcase m_shocaseUI; //콜백받을 곳
    public bool m_isSelected = false;
    public void ShowCaseSet(TokenBase _capitalToken, UIShowcase _shocaseUI)
    {
        SetSlot(_capitalToken);
        m_shocaseUI = _shocaseUI;
        m_isSelected = false;
        
    }

    public override void OnLeftClick()
    {
        //선택이 안된 상태라면
        if(m_isSelected == false)
        {
            //선택 요청
            bool isSelected = m_shocaseUI.SelectSlot(this);
            if (isSelected)
                m_isSelected = true;
        }
        //선택이 된 상황이라면
        else
        {
            bool isCancled = m_shocaseUI.CancleSlot(this); //캔슬 요청
            m_isSelected = false;
        }
    }

}
