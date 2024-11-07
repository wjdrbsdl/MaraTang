﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class MgWorkOrderPin : MgGeneric<MgWorkOrderPin>
{
    public WorkOrderPin m_pinSample;
    private List<WorkOrderPin> m_pinLIst = new();
    
    public WorkOrderPin RequestWorkOrderPin(TokenTile _tile)
    {
        Debug.Log("작업핀 요청 받음");
        WorkOrderPin workPin = Instantiate(m_pinSample);
        workPin.SetPinInfo(_tile);
        m_pinLIst.Add(workPin);
        return workPin;
    }

    public void RemovePin(TokenTile _tile)
    {

    }

}
