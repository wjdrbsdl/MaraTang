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
        List<(int, int)> _rewardList = _reward.RewardsList;
        RewardType rewardType = _reward.RewardType;
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
