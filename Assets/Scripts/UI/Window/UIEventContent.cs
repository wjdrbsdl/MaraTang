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
        //�̺�Ʈ �����͵��� �����̷� ǥ��
        Switch(true);
        int needSlotCount = MakeCount<EventSlot>(m_eventSlots, _eventTokens.Count);
        if (needSlotCount > 0)
        {
            MakeSlots<EventSlot>(ref m_eventSlots, needSlotCount, m_sampleEventSlot.gameObject, m_box);
        }
            

        for (int i = 0; i < _eventTokens.Count; i++)
        {
            m_eventSlots[i].SetSlot(_eventTokens[i]);
            m_eventSlots[i].gameObject.SetActive(true);
        }
    }

    public override void OffWindow()
    {
        //â�� ���ٸ�
        for (int i = 0; i < m_eventSlots.Length; i++)
        {
            m_eventSlots[i].gameObject.SetActive(false);
        }
    }
}
