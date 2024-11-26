using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICapitalChange : UIBase
{
    [SerializeField] private InputSlot[] m_inputCapitals;
    [SerializeField] private GameObject m_sampleSlot;
    [SerializeField] private Transform m_box;
    [SerializeField] private RectTransform m_rectTrans;

    private InputSlot m_selectInputSlot = null;
    public void SetChangeUI(TokenTile _tile)
    {
        //1. 선택한 재료 세팅할 부분
        m_window.SetActive(true);
        //2. 재료 선택할 쇼케이스 호출
       
        int tempSelectMaxCount = 2;
        MgUI.GetInstance().ShowCaseOpen(m_rectTrans, OnChangeSelect, tempSelectMaxCount);
    }

    public void OnChangeSelect(List<ShowcaseSlot> _selectedSlot)
    {
        int selectCount = _selectedSlot.Count;
        MakeSamplePool<InputSlot>(ref m_inputCapitals, m_sampleSlot.gameObject, selectCount, m_box);
        int slotCount = m_inputCapitals.Length;

        for (int i = 0; i < selectCount; i++)
        {
            m_inputCapitals[i].gameObject.SetActive(true);
            m_inputCapitals[i].SetSlot(_selectedSlot[i].GetTokenBase());
        }
        for (int close = selectCount; close < slotCount; close++)
        {
            m_inputCapitals[close].gameObject.SetActive(false);
        }
    }

    private void OnClickInputSlot(InputSlot _inputSlot)
    {
        m_selectInputSlot = _inputSlot;
        MgUI.GetInstance().ShowCaseOpen(m_rectTrans, m_selectInputSlot);
    }

    private void OnInputSlot(InputSlot _inputSlot)
    {
        Debug.Log("해당 인풋슬랏에 세팅 들어옴");
        _inputSlot.selectedText.text = _inputSlot.ShowCase.GetTokenBase().GetItemName();
    }

    private void ResetRecord()
    {
        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            m_inputCapitals[i].gameObject.SetActive(false);
        }
    }

    #region 버튼 호출
    public void OnBtnDoJob()
    {
         DoChangeCapital();
     
    }

    private void DoChangeCapital()
    {
        //2칸 다 선택했는지 체크
        bool isOk = true;
        for (int i = 0; i < 2; i++)
        {
            //1. 순서대로 활성화 되기 때문에 꺼져있으면 넣을 재료 끝난거
            if (m_inputCapitals[i].gameObject.activeSelf == false)
            {
                isOk = false;
                break;
            }
        }

        if (isOk == false)
        {
            Debug.Log("선택 다 안됨");
            return;
        }

        //1. 첫번째가 재료
        (Capital, int) input = ((Capital)m_inputCapitals[0].GetTokenBase().GetPid(), m_inputCapitals[0].GetAmount());
        //2. 두번째의 capital이 자원
        Capital ouput = (Capital)m_inputCapitals[1].GetTokenBase().GetPid();
        //투약한 재료가 2개 이상인경우 
        GamePlayMaster.GetInstance().RuleBook.ChangeCapital(input, ouput);
    }
    #endregion
}
