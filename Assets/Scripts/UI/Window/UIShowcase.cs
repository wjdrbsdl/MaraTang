using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowcase : UIBase
{
    [SerializeField]
    private ShowcaseSlot m_showcaseSample;
    [SerializeField]
    private Transform m_box;
    [SerializeField]
    private ShowcaseSlot[] m_showcaseSlots;

    private Action<ShowcaseSlot> m_selectAction;

    public void OpenWindow(Action<ShowcaseSlot> _selecttAction)
    {
        base.OpenWindow();
        //1. 캐릭터가 보유한 자원 리스트를 가져온다. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapital();

        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_box);
        m_selectAction = _selecttAction;
        for (int i = 0; i < m_showcaseSlots.Length; i++)
        {
            m_showcaseSlots[i].gameObject.SetActive(true);
            m_showcaseSlots[i].ShowCaseSet(((Capital)i).ToString(), this);
        }
    }

    public void SizeControl(RectTransform _tran)
    {
        RectTransform rectTrans = m_box.GetComponent<RectTransform>();
        rectTrans.sizeDelta = _tran.sizeDelta;
        Vector3 movePos = _tran.position;
        float yMove = rectTrans.sizeDelta.y * 0.5f + _tran.sizeDelta.y * 0.5f + 55f;
        movePos.y -= yMove;
        rectTrans.position = movePos;
    }
   
    public void SelectedSlot(ShowcaseSlot _caseSlot)
    {
        m_selectAction(_caseSlot);
    }
}
