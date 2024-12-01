using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaborRequestBtn : MonoBehaviour
{
    //�ƹ���ɾ��� �����ֱ⸸ �ϴ� ����
    public TMP_Text requestText; //���߿� ���������� ��ü�ɺκ�
    [SerializeField]
    private int m_index;
    [SerializeField]
    private UITileInfo m_tileInfo;

    public void SetSlot(int _index, bool _have)
    {
        requestText.text = "��û";
        if(_have)
            requestText.text = "��ȯ";

        m_index = _index;
    }


    public void ClickRequestBtn()
    {
        m_tileInfo.OnClickLaborCoin(m_index);
    }
}
