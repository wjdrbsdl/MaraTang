using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIChefUI : UIBase
{
    [SerializeField]
    GameObject[] m_subUies;

    [SerializeField] private InputSlot[] m_inputCapitals;
    [SerializeField] private GameObject m_sampleSlot;
    [SerializeField] private Transform m_box;
    [SerializeField] private RectTransform m_rectTrans;


    public void SetChefUI(int subCode, TokenTile _tile, TokenAction _actionToken)
    {
        //1. 선택한 재료 세팅할 부분
        m_window.SetActive(true);
        //2. 재료 선택할 쇼케이스 호출
        int tempSelectMaxCount = 2;
        MgUI.GetInstance().ShowCaseOpen(m_rectTrans, OnChangeSelect, tempSelectMaxCount);
        //3. 이전 선택 초기화
        ResetRecord();
    }

    public void OnBtnMixCapital()
    {
        List<(Capital, int)> resources = new(); //넣은 재료와 수량

        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            //1. 순서대로 활성화 되기 때문에 꺼져있으면 넣을 재료 끝난거
            if (m_inputCapitals[i].gameObject.activeSelf == false)
                break;

            Capital inputCaptial = (Capital) m_inputCapitals[i].GetTokenBase().GetPid(); //자원 정보 빼오고 - Pid로 자원코드 관리
            int amount = m_inputCapitals[i].GetAmount(); //해당 슬롯에 할당된 수량 빼옴
            resources.Add((inputCaptial, amount)); //튜플로 추가
        }
        //투약한 재료가 2개 이상인경우 
        if(resources.Count>=2)
        GamePlayMaster.GetInstance().RuleBook.MixCapital(resources);
    }

    public void OnChangeSelect(List<ShowcaseSlot> _selectedSlot)
    {
        //선택받은 슬랏을 받고싶다. 
        string log = "";
        for (int i = 0; i < _selectedSlot.Count; i++)
        {
            log += ((Capital)(_selectedSlot[i].GetTokenBase().GetPid())).ToString() + "가 클릭됨\n";
        }
        Debug.Log(log);

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

    private void ResetRecord()
    {
        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            m_inputCapitals[i].gameObject.SetActive(false);
        }
    }
}
