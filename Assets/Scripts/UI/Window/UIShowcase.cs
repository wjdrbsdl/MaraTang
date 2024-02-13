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

    private Action<List<ShowcaseSlot>> m_selectAction;
    private int m_selectMaxCount;
    private List<ShowcaseSlot> m_selectedSlotList;

    public void OpenWindow(Action<List<ShowcaseSlot>> _selecttAction, int _selectMaxCount)
    {
        base.OpenWindow();
        //0. ��û�� ��Ÿ�� �Ҵ�
        m_selectAction = _selecttAction;
        m_selectMaxCount = _selectMaxCount;
        m_selectedSlotList = new();

        //1. ĳ���Ͱ� ������ �ڿ� ����Ʈ�� �����´�. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapital();
     
        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_box);
        
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
   
    public bool SelectSlot(ShowcaseSlot _caseSlot)
    {
        //������ ���������� ���� �������� ���θ� ��ȯ
        //���� ���õ� Ƚ���� ���ؼ� �ִ� Ƚ���� �Ұ��� ��ȯ
        if (m_selectedSlotList.Count == m_selectMaxCount)
                return false;

        m_selectedSlotList.Add(_caseSlot); //���������� �߰�
        OnChangedSelect();
        return true;
    }

    public bool CancleSlot(ShowcaseSlot _caseSlot)
    {
        //�����ߴ� ����Ʈ���� �����ϰ� Ʈ���ȯ
        m_selectedSlotList.Remove(_caseSlot);
        OnChangedSelect();
        return true;
    }

    private void OnChangedSelect()
    {
        m_selectAction(m_selectedSlotList);
    }
}
