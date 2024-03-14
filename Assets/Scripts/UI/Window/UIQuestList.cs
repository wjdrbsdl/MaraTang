using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIQuestList : UIBase
{
    public BtnQuestList m_questSlotSample;
    public BtnQuestList[] m_questSlots;
    public Transform m_box;
    public void SetQuestList()
    {
        Switch(!m_window.activeSelf);
        List<Quest> questList = MGContent.g_instance.GetQuestList();
        MakeSamplePool<BtnQuestList>(ref m_questSlots, m_questSlotSample.gameObject, questList.Count, m_box);

        SetInfo(questList);
    }

    public void RefreshList()
    {
        //퀘스트 상태가 변했을 때 호출해서 UI 새로 세팅 
        if (m_window.activeSelf == false)
            return;

        List<Quest> questList = MGContent.g_instance.GetQuestList();
        MakeSamplePool<BtnQuestList>(ref m_questSlots, m_questSlotSample.gameObject, questList.Count, m_box);

        SetInfo(questList);
    }

    private void SetInfo(List<Quest> _questList)
    {

        for (int i = 0; i < _questList.Count; i++)
        {
            m_questSlots[i].gameObject.SetActive(true);
            m_questSlots[i].SetButton(_questList[i]);
        }
        for (int i = _questList.Count; i < m_questSlots.Length; i++)
        {
            m_questSlots[i].gameObject.SetActive(false);
        }
    }
}
