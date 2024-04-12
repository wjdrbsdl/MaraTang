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

    public void ShowEventList(List<TokenEvent> _eventTokens)
    {
        //이벤트 받은것들을 유아이로 표현
        Switch(true);
        MakeSamplePool<EventSlot>(ref m_eventSlots, m_sampleEventSlot.gameObject, _eventTokens.Count, m_box);

        for (int i = 0; i < _eventTokens.Count; i++)
        {
            m_eventSlots[i].SetSlot(_eventTokens[i]);
            m_eventSlots[i].gameObject.SetActive(true);
        }
    }

    public void ShowItemList(TTokenOrder _tokenOrder)
    {
        Switch(true);

        //주문서 방식이 선택 표기인 경우 대상 아이템을 표기해줌. 
        int itemCount = _tokenOrder.orderItemList.Count;
        MakeSamplePool<EventSlot>(ref m_eventSlots, m_sampleEventSlot.gameObject, itemCount, m_box);
        SetRewardSlots(_tokenOrder.orderItemList);
    }

    private void SetRewardSlots(List<TOrderItem> _ItemList)
    {
        for (int i = 0; i < _ItemList.Count; i++)
        {
            m_eventSlots[i].gameObject.SetActive(true);
            m_eventSlots[i].SetSlot(_ItemList[i], this);
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
}
