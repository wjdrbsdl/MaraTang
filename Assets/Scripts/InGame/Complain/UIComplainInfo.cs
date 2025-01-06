using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
public class UIComplainInfo : UIBase
{
    Complain m_complain = null; //할당된 녀석
    public TMP_Text InfoText;
    public void SetCompalinInfo(Complain _complain)
    {
        UISwitch(true);
        m_complain = _complain;
        InfoText.text = "요청 타입 " + _complain.RequestType + "남은 턴 "+_complain.RestTurn;
    }

    //플레이어가 해결하기 누르면 
    public void OnClickRespond()
    {
        m_complain.Respond();
        UISwitch(false);
    }
}

