using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRewardChoose : UIBase
{
    [SerializeField]
    private EventSlot m_sampleEventSlot;
    [SerializeField]
    private Transform m_box;
    [SerializeField]
    private EventSlot[] m_eventSlots = new EventSlot[] { };

    public void ShowItemList(TTokenOrder _tokenOrder)
    {
        UISwitch(true);

        //주문서 방식이 선택 표기인 경우 대상 아이템을 표기해줌. 
        int itemCount = _tokenOrder.orderItemList.Count;
        MakeSamplePool<EventSlot>(ref m_eventSlots, m_sampleEventSlot.gameObject, itemCount, m_box);
        SetRewardSlots(_tokenOrder);
    }

    private void SetRewardSlots(TTokenOrder _tokenOrder)
    {
        List<TOrderItem> _ItemList = _tokenOrder.orderItemList;
        for (int i = 0; i < _ItemList.Count; i++)
        {
            int itemIdx = i;
            m_eventSlots[i].gameObject.SetActive(true);
            m_eventSlots[i].SetSlot(_tokenOrder, itemIdx, this);
        }
    }

    public override void OffWindow()
    {
        //창을 끈다면
        for (int i = 0; i < m_eventSlots.Length; i++)
        {
            m_eventSlots[i].gameObject.SetActive(false);
        }
    }

    public void Confirm()
    {

    }
}
