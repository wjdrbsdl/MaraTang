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
    private Transform m_grid;
    [SerializeField]
    private ShowcaseSlot[] m_showcaseSlots;

    private Action<List<ShowcaseSlot>> m_selectAction;
    private int m_selectMaxCount;
    private List<ShowcaseSlot> m_selectedSlotList;

    public void OpenWindow(Action<List<ShowcaseSlot>> _selecttAction, int _selectMaxCount)
    {
        UISwitch(true);
        //0. ��û�� ��Ÿ�� �Ҵ�
        m_selectAction = _selecttAction;
        m_selectMaxCount = _selectMaxCount;
        m_selectedSlotList = new();

        //1. ĳ���Ͱ� ������ �ڿ� ����Ʈ�� �����´�. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapitalDic();
     
        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_grid);
        
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

    private InputSlot m_targetSlot;
    public void OpenWindow(InputSlot _inputSlot)
    {
        UISwitch(true);
        m_targetSlot = _inputSlot;

        //1. ĳ���Ͱ� ������ �ڿ� ����Ʈ�� �����´�. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapitalDic();

        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_grid);

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
        RectTransform caseRectTrans = m_box.GetComponent<RectTransform>();
        caseRectTrans.pivot = _tran.pivot; //�Ǻ� �����ϰ� �ٲ۵�
        Vector3 movePos = _tran.position; //�̵��� ��ġ�� �̵���Ű�� 
        //���� ���� * �Ǻ� ��ŭ, �� �Ǻ� �ݴ븸ŭ - �տ��� �Ǻ��� ��ġ ���ѳ��� ������ �ؿ� �������ִ� 1- �������� ����
        float yMove = caseRectTrans.sizeDelta.y * (1 - caseRectTrans.pivot.y)+ _tran.sizeDelta.y * _tran.pivot.y;
        yMove += 5f; //�е�
        movePos.y -= yMove; //y�����̵� 
        caseRectTrans.position = movePos;
    }
   
    public void TempSelectSlot(ShowcaseSlot _caseSlot)
    {
        m_targetSlot.SetShowCase(_caseSlot);
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
