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

    public void ShowRewardList(RewardData _reward)
    {
        Switch(true);
        List<TTokenOrder> rewardList = _reward.RewardsList;
        
        MakeSamplePool<EventSlot>(ref m_eventSlots, m_sampleEventSlot.gameObject, rewardList.Count, m_box);
        //장비의 1, 2,3, 
        //스킬의 2,3,4
        //스텟의 5,6,7 등 어떠한 타입에서의 보상을 번호로 넘겨받은 상태 그 데이터 그대로 일단 표기 

        //1. 타입으로 masterData 타입을 찾는다 
        //2. subValue로 해당 data에서 해당하는 pid를 찾는다 
        //3. 찾은 데이터의 정보로 보상 슬롯을 세팅한다. 
        SetRewardSlots(rewardList);
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
