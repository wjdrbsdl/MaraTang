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
        //1. ĳ���Ͱ� ������ �ڿ� ����Ʈ�� �����´�. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapital();
     
        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_box);
        m_selectAction = _selecttAction;
       int setCount = 0; //���� ������ ��

        foreach (KeyValuePair<Capital, TokenBase> item in haveCapitals)
        {
            m_showcaseSlots[setCount].gameObject.SetActive(true);
            m_showcaseSlots[setCount].ShowCaseSet(item.Value, this); 
            setCount += 1; //������ ���� �ø���
        }

        //���� �� ���ں��� �� �ڱ��� ��Ȱ��
        for (int i = setCount; i < haveCapitals.Count; i++)
        {
            m_showcaseSlots[i].gameObject.SetActive(false);
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
