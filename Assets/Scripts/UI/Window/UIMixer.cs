using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMixer : UIBase
{
   
    [SerializeField] private InputSlot[] m_inputCapitals;
    [SerializeField] private GameObject m_sampleSlot;
    [SerializeField] private Transform m_box;
    [SerializeField] private RectTransform m_rectTrans;

    private CapitalAction curMode = CapitalAction.MixCapital; //현재 어떤 모드인지
    private InputSlot m_selectInputSlot = null;
    public void SetChefUI(CapitalAction subCode, TokenTile _tile, TokenAction _actionToken)
    {
        //1. 선택한 재료 세팅할 부분
        m_window.SetActive(true);
        //2. 재료 선택할 쇼케이스 호출
        curMode = subCode;
        if (curMode.Equals(CapitalAction.MixCapital))
        {
            int tempSelectMaxCount = 2;
            //1. 인풋 슬랏 세팅
            MakeSamplePool<InputSlot>(ref m_inputCapitals, m_sampleSlot.gameObject, tempSelectMaxCount, m_box);
            //2. 인풋 슬랏에 이벤트 할당
            for (int i = 0; i < tempSelectMaxCount; i++)
            {
                m_inputCapitals[i].gameObject.SetActive(true);
                m_inputCapitals[i].SetEventOnInput(OnClickInputSlot, OnInputSlot);
            }
            OnClickInputSlot(m_inputCapitals[0]);
           // MgUI.GetInstance().ShowCaseOpen(m_rectTrans, OnChangeSelect, tempSelectMaxCount);
        }
        else if (curMode.Equals(CapitalAction.ChageCapital))
        {
            int tempSelectMaxCount = 2;
            MgUI.GetInstance().ShowCaseOpen(m_rectTrans, OnChangeSelect, tempSelectMaxCount);
        }


        //3. 이전 선택 초기화
       // ResetRecord();
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
        if (curMode.Equals(CapitalAction.MixCapital))
        {
            DoMixCapital();
        }
        else if (curMode.Equals(CapitalAction.ChageCapital))
        {
            DoChangeCapital();
        }

    }

    private void DoMixCapital()
    {
        List<(Capital, int)> resources = new(); //넣은 재료와 수량

        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            //1. 순서대로 활성화 되기 때문에 꺼져있으면 넣을 재료 끝난거
            if (m_inputCapitals[i].gameObject.activeSelf == false)
                break;

            Capital inputCaptial = (Capital)m_inputCapitals[i].GetTokenBase().GetPid(); //자원 정보 빼오고 - Pid로 자원코드 관리
            int amount = m_inputCapitals[i].GetAmount(); //해당 슬롯에 할당된 수량 빼옴
            resources.Add((inputCaptial, amount)); //튜플로 추가
        }
        //투약한 재료가 2개 이상인경우 
        if (resources.Count >= 2)
            GamePlayMaster.GetInstance().RuleBook.MixCapital(resources);
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
