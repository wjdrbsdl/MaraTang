using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINationPolicy : UIBase
{
    public PolicySlot m_slotSample;
    public PolicySlot[] m_policySlots;
    public Transform m_gridBox;

    public void SetNationPolicy(Nation _nation)
    {
        UISwitch(!m_window.activeSelf);
        List<NationPolicy> policyList = _nation.GetNationPolicyList();
        MakeSamplePool<PolicySlot>(ref m_policySlots, m_slotSample.gameObject, policyList.Count, m_gridBox);
        SetInfo(policyList);
    }

    private void SetInfo(List<NationPolicy> _policyList)
    {
        int policyCount = _policyList.Count;
        for (int i = 0; i < policyCount; i++)
        {
            m_policySlots[i].gameObject.SetActive(true);
            m_policySlots[i].SetSlot(_policyList[i]);
        }
        //정보 남는 슬롯은 off
        for (int over = policyCount; over < m_policySlots.Length; over++)
        {
            m_policySlots[over].gameObject.SetActive(false);
        }
    }
}
