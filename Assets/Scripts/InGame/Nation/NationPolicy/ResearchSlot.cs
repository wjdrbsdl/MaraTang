using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ReserchSlotFunction
{
    Select, Show
}

public class ResearchSlot : SlotBase
{
    public ReserchSlotFunction m_function = ReserchSlotFunction.Select; //해당 슬랏 기능 
    private NationTechData m_techData; //할당된 스킬 pid
    private bool m_isComplete = false; 
    [SerializeField]
    private UIResearch m_reserachUI;
    public TMP_Text reserchText; //나중에 아이콘으로 대체될부분

    public void SetResearchSlot(NationTechData _techData, bool _done)
    {
        gameObject.SetActive(true);
        m_techData = _techData;
        m_isComplete = _done;
        reserchText.text = _techData.GetTechName();
    }

    public void OnClick()
    {
        if (m_function != ReserchSlotFunction.Select)
            return;

        //안 배운경우에만 배우겠다고 의사전달 
        if (m_isComplete == false)
            m_reserachUI.SelectReserach(m_techData);

    }
}

