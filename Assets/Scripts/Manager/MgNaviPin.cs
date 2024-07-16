using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNaviPin : MgGeneric<MgNaviPin>
{
    public NaviPin m_pinSample;
    public int m_pinMaxCount = 3;
    private List<NaviPin> m_pinLIst = new();
    private List<NaviPin> m_tileWorkPinList = new();
    public NaviPin RequestNaviPin()
    {
        if (m_pinMaxCount <= m_pinLIst.Count)
        {
            Debug.Log("핀 최대 수량");
            return null;
        }

        NaviPin naviPin = Instantiate(m_pinSample);
        m_pinLIst.Add(naviPin);
        return naviPin;
    }

    public void RequestTileWorkPin(TokenTile _pinPosTile, int _workType)
    {
        NaviPin naviPin = Instantiate(m_pinSample);
        m_tileWorkPinList.Add(naviPin);
        naviPin.SetPinInfo(_pinPosTile.GetObject().gameObject.transform.position, _workType);
    }

    public void RemovePin(NaviPin _pin)
    {
        //제거 요구 받은 핀이 null이면 종료
        if (_pin == null)
            return;

        m_pinLIst.Remove(_pin);
        Destroy(_pin.gameObject);
    }
}
