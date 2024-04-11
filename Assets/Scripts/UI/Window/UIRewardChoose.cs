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
        //�̺�Ʈ �����͵��� �����̷� ǥ��
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
        //����� 1, 2,3, 
        //��ų�� 2,3,4
        //������ 5,6,7 �� ��� Ÿ�Կ����� ������ ��ȣ�� �Ѱܹ��� ���� �� ������ �״�� �ϴ� ǥ�� 

        //1. Ÿ������ masterData Ÿ���� ã�´� 
        //2. subValue�� �ش� data���� �ش��ϴ� pid�� ã�´� 
        //3. ã�� �������� ������ ���� ������ �����Ѵ�. 
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
        //â�� ���ٸ�
        for (int i = 0; i < m_eventSlots.Length; i++)
        {
            m_eventSlots[i].gameObject.SetActive(false);
        }
    }
}
