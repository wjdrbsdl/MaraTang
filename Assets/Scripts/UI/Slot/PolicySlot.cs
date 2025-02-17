using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PolicySlot : SlotBase
{
    public TMP_Text m_policyName;
    public TMP_Text m_policyState;
    public TMP_Text m_policyNum;
    private NationPolicy m_nationPolicy;

    public void SetSlot(NationPolicy _nationPolicy)
    {
        m_policyName.text = _nationPolicy.GetMainPolicy().ToString();
        m_policyState.text = _nationPolicy.GetHoldCount().ToString();
        m_policyNum.text = _nationPolicy.GetPlanIndex().ToString();
        m_nationPolicy = _nationPolicy;
    }

    public void OnClickCancleBtn()
    {
        MgNation.GetInstance().GetNation(m_nationPolicy.GetNaionNum()).SuggestPolicyCancle(m_nationPolicy);
    }
}
