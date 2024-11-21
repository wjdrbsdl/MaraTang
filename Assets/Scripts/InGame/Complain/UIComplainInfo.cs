using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIComplainInfo : UIBase
{
    Complain m_complain = null; //할당된 녀석

    public void SetCompalinInfo(Complain _complain)
    {
        UISwitch(true);
        m_complain = _complain;
    }

    //플레이어가 해결하기 누르면 
    public void OnClickRespond()
    {
        m_complain.Respond();
    }
}

