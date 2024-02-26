using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNaviPin : MgGeneric<MgNaviPin>
{
    public NaviPin m_pinSample;
    private List<NaviPin> m_pinLIst = new();

    public NaviPin GetNaviPin()
    {
        NaviPin naviPin = Instantiate(m_pinSample);
        m_pinLIst.Add(naviPin);
        return naviPin;
    }

    public void RemovePin(NaviPin _pin)
    {
        m_pinLIst.Remove(_pin);
        Destroy(_pin.gameObject);
    }
}
