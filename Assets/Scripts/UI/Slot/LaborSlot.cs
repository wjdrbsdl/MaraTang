using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaborSlot : SlotBase
{
    //아무기능없이 보여주기만 하는 슬랏
    public TMP_Text laborNumText; //나중에 아이콘으로 대체될부분
    [SerializeField]
    private LaborCoin m_laborCoin; 

    public void SetSlot(LaborCoin _laborCoin)
    {
        m_laborCoin = _laborCoin;
        laborNumText.text = _laborCoin.ListIndex.ToString();
    }


    public void ClickLaborCoin()
    {
        if (m_laborCoin.tileType != TileType.Capital)
            return;

        m_laborCoin.BackCapital();
    }
}
