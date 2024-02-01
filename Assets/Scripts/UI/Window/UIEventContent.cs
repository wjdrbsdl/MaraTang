using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEventContent : UIBase
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

    public override void OffWindow()
    {
        //창을 끈다면
        for (int i = 0; i < m_eventSlots.Length; i++)
        {
            m_eventSlots[i].gameObject.SetActive(false);
        }
    }
}
