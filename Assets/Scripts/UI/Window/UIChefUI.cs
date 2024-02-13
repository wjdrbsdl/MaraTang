using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChefUI : UIBase
{
    [SerializeField]
    GameObject[] m_subUies;

    [SerializeField]
    InputSlot[] m_inputCapitals;

    [SerializeField]
    Transform m_box;
    [SerializeField]
    RectTransform m_rectTrans;

    public void SetChefUI(int subCode, TokenTile _tile, TokenAction _action)
    {
        m_window.SetActive(true);
        MgUI.GetInstance().ShowCaseOpen(m_rectTrans, RequestSlot);
        // m_subUies[subCode].SetActive(true);
    }

    public void MixCapital()
    {
        List<(Capital, int)> resources = new(); //넣은 재료와 수량

        for (int i = 0; i < m_inputCapitals.Length; i++)
        {

        }
        //투약한 재료가 2개 이상인경우 
        if(resources.Count>=2)
        GamePlayMaster.GetInstance().RuleBook.MixCapital(resources);
    }

    public void OnChangedRecipe()
    {
        //재료가 바뀔때마다 
    }

    public void RequestSlot(ShowcaseSlot _selectedSlot)
    {
        //선택받은 슬랏을 받고싶다. 
        Debug.Log(_selectedSlot.m_testText + "가 클릭됨");
    }
}
