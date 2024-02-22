using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIQuestList : UIBase
{
    public TMP_Text m_questSlotSample;
    public TMP_Text[] m_questSlots;
    public Transform m_box;
    public void SetQuestList()
    {
       List<Quest> questList = MGContent.g_instance.GetQuestList();
        MakeSamplePool<TMP_Text>(ref m_questSlots, m_questSlotSample.gameObject, questList.Count, m_box);

        SetInfo(questList);
    }

    private void SetInfo(List<Quest> _questList)
    {

        for (int i = 0; i < _questList.Count; i++)
        {
            m_questSlots[i].gameObject.SetActive(true);
            m_questSlots[i].text = _questList[i].QuestPid + "¹ø ÁøÇà";
        }
        for (int i = _questList.Count; i < m_questSlots.Length; i++)
        {
            m_questSlots[i].gameObject.SetActive(false);
        }
    }
}
