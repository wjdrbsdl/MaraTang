using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaborRequestBtn : MonoBehaviour
{
    //아무기능없이 보여주기만 하는 슬랏
    public TMP_Text requestText; //나중에 아이콘으로 대체될부분
    [SerializeField]
    private int m_index;
    [SerializeField]
    private UITileInfo m_tileInfo;

    public void SetSlot(int _index, bool _have)
    {
        requestText.text = "요청";
        if(_have)
            requestText.text = "반환";

        m_index = _index;
    }


    public void ClickRequestBtn()
    {
        m_tileInfo.OnClickLaborCoin(m_index);
    }
}
