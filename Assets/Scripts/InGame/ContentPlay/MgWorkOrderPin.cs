using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class MgWorkOrderPin : MgGeneric<MgWorkOrderPin>
{
    public WorkOrderPin m_pinSample;
    private Dictionary<WorkOrder, WorkOrderPin> m_pinDic = new();
    
    public WorkOrderPin RequestWorkOrderPin(TokenTile _tile)
    {
     //   Debug.Log("작업핀 요청 받음");
        WorkOrderPin workPin = Instantiate(m_pinSample);
        workPin.SetPinInfo(_tile);
        m_pinDic.Add(_tile.GetWorkOrder(), workPin);
        return workPin;
    }

    bool isWarn = false;
    public void RequestComplainPin(TokenTile _tile)
    {
        if(isWarn == false)
        {
            Debug.LogWarning("딱히 하는건 없는 핀 요구");
            isWarn = true;
        }
        
    }

    public void RemovePin(TokenTile _tile)
    {
        //   Debug.Log("작업핀 제거 요청 받음");
        if (_tile.GetWorkOrder()== null || m_pinDic.ContainsKey(_tile.GetWorkOrder()) == false)
            return;

        WorkOrderPin pin = m_pinDic[_tile.GetWorkOrder()];
        pin.DestroyPin();
        m_pinDic.Remove(_tile.GetWorkOrder());
    }

    bool isWarn2 = false;
    public void RemoveComplainPin(TokenTile _tile)
    {
        if (isWarn2 == false)
        {
            Debug.LogWarning("딱히 하는건 없는 핀 요구");
            isWarn2 = true;
        }

    }
}
