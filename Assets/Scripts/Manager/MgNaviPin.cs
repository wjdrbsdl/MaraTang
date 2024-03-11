using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNaviPin : MgGeneric<MgNaviPin>
{
    public NaviPin m_pinSample;
    public int m_pinMaxCount = 3;
    private List<NaviPin> m_pinLIst = new();

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

    public void RemovePin(NaviPin _pin)
    {
        //���� �䱸 ���� ���� null�̸� ����
        if (_pin == null)
            return;

        m_pinLIst.Remove(_pin);
        Destroy(_pin.gameObject);
    }
}
