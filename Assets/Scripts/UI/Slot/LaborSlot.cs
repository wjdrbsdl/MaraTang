using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaborSlot : SlotBase
{
    //�ƹ���ɾ��� �����ֱ⸸ �ϴ� ����
    public TMP_Text laborNumText; //���߿� ���������� ��ü�ɺκ�
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
