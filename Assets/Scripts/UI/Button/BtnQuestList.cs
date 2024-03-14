using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnQuestList : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_statText;
    private Quest m_quest;

    public void SetButton(Quest _quest)
    {
        m_quest = _quest;
        m_statText.text = _quest.QuestPid + "¹ø ³²Àº È½¼ö "+_quest.RestWoldTurn;
    }

    public void OnButtonClick()
    {
        MgUI.GetInstance().ShowQuest(m_quest);
    }
}
