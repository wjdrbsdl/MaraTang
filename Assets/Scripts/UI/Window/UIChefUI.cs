using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChefUI : UIBase
{
    [SerializeField]
    GameObject[] m_subUies;

    [SerializeField]
    InputSlot[] m_inputCapitals;

    public void SetChefUI(int subCode, TokenTile _tile, TokenAction _action)
    {
        m_window.SetActive(true);
       // m_subUies[subCode].SetActive(true);
    }

    public void MixCapital()
    {
        List<(Capital, int)> resources = new(); //���� ���� ����

        for (int i = 0; i < m_inputCapitals.Length; i++)
        {

        }
        //������ ��ᰡ 2�� �̻��ΰ�� 
        if(resources.Count>=2)
        GamePlayMaster.GetInstance().RuleBook.MixCapital(resources);
    }

    public void OnChangedRecipe()
    {
        //��ᰡ �ٲ𶧸��� 
    }
}
