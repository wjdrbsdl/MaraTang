using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNaviPin : MgGeneric<MgNaviPin>
{
    public NaviPin m_pinSample;
    public int m_pinMaxCount = 3;
    private List<NaviPin> m_pinLIst = new();
    private Dictionary<NationPolicy, List<NaviPin>> m_policyPinDic = new();
    public NaviPin RequestNaviPin()
    {
        if (m_pinMaxCount <= m_pinLIst.Count)
        {
            Debug.Log("�� �ִ� ����");
            return null;
        }

        NaviPin naviPin = Instantiate(m_pinSample);
        m_pinLIst.Add(naviPin);
        return naviPin;
    }

    public void ShowPolicyPin(NationPolicy _policy)
    {
        //��å�� ���õ� �� ǥ�� 
        MainPolicy mainPolicy = _policy.GetMainPolicy();
        switch (mainPolicy)
        {
            case MainPolicy.ExpandLand:
            case MainPolicy.ManageLand:
                MakePolicyPin(_policy);
                break;
        }
    }

    public void MakePolicyPin(NationPolicy _policy)
    {
        TokenTile targetTile = (TokenTile)_policy.GetPlanToken();
        int planIndex = _policy.GetPlanIndex();
        NaviPin naviPin = Instantiate(m_pinSample);
        naviPin.SetTileWorkPin(targetTile.GetObject().gameObject.transform.position, _policy.GetMainPolicy());
        AddPolicyPinList(_policy, naviPin);
    }

    public void RemovePin(NaviPin _pin)
    {
        //���� �䱸 ���� ���� null�̸� ����
        if (_pin == null)
            return;

        m_pinLIst.Remove(_pin);
        Destroy(_pin.gameObject);
    }

    public void RemovePolicyPinList(NationPolicy _policy)
    {
        //��å ���õ� �� ����
        if (m_policyPinDic.ContainsKey(_policy) == false)
            return;

        List<NaviPin> pinLIst = m_policyPinDic[_policy];
        //�ش� �ɵ� �����ϴ� �� �����ϰ�
        for (int i = 0; i < pinLIst.Count; i++)
        {
            NaviPin pin = pinLIst[i];
            pin.DestroyPin();
        }

        m_policyPinDic.Remove(_policy); //��ǿ��� ����
    }

    private void AddPolicyPinList(NationPolicy _policy, NaviPin _pin)
    {
        //��å�� ���õ� �� ����Ʈ �߰� 
        if (m_policyPinDic.ContainsKey(_policy))
        {
            m_policyPinDic[_policy].Add(_pin);
            return;
        }
        List<NaviPin> pinList = new();
        m_policyPinDic.Add(_policy, pinList);
        m_policyPinDic[_policy].Add(_pin);
    }
}
