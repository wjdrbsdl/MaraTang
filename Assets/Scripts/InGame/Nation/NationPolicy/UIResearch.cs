using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIResearch : UIBase
{
    //해당 플레이어가 지닌 자원을 종류 수량을 선택하는 부분. 
    [SerializeField]
    private ResearchSlot m_reserachSlotSample;
    [SerializeField]
    private Transform m_grid;
    [SerializeField]
    private ResearchSlot[] m_researchSlots;

    [SerializeField]
    private ResearchSlot m_selectSlot;

    public void SetNationResearch(int _nationNum)
    {
        UISwitch(true);
        Nation nation = MgNation.GetInstance().GetNation(_nationNum);
        List<NationTechData> techList = MgMasterData.GetInstance().GetTechDic().Select(dic => dic.Value).ToList();
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<ResearchSlot>(ref m_researchSlots, m_reserachSlotSample.gameObject, techList.Count, m_grid);
        //2. 리스트대로 슬랏에 표기
        SetSlots(techList, nation.TechPart);
    }

    private void SetSlots(List<NationTechData> _techList, NationTechPart _nationTech)
    {
        int itemCount = _techList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            m_researchSlots[i].gameObject.SetActive(true);
            m_researchSlots[i].SetResearchSlot(_techList[i], _nationTech.IsDoneTech(_techList[i].GetPid()));
        }
        for (int i = itemCount; i < m_researchSlots.Length; i++)
        {
            m_researchSlots[i].gameObject.SetActive(false);
        }
    }

    public void SelectReserach(NationTechData _techData)
    {

    }
}

