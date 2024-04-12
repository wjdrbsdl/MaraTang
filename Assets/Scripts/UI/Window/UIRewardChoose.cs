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

    public void ShowRewardList(TTokenOrder _tokenOrder)
    {
        Switch(true);
      
        //주문서 방식이 선택 표기인 경우 대상 아이템을 표기해줌. 
        
        //MakeSamplePool<EventSlot>(ref m_eventSlots, m_sampleEventSlot.gameObject, rewardList.Count, m_box);
        //SetRewardSlots(rewardList);
    }

    private void SetRewardSlots(List<TTokenOrder> _rewardList)
    {
        for (int i = 0; i < _rewardList.Count; i++)
        {
            m_eventSlots[i].gameObject.SetActive(true);
            m_eventSlots[i].SetSlot(_rewardList[i], this);
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
