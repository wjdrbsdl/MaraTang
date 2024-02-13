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
        //1. ������ ��� ������ �κ�
        m_window.SetActive(true);
        //2. ��� ������ �����̽� ȣ��
        int tempSelectMaxCount = 2;
        MgUI.GetInstance().ShowCaseOpen(m_rectTrans, OnChangeSelect, tempSelectMaxCount);
        //3. ���� ���� �ʱ�ȭ
        ResetRecord();
    }

    public void OnBtnMixCapital()
    {
        List<(Capital, int)> resources = new(); //���� ���� ����

        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            //1. ������� Ȱ��ȭ �Ǳ� ������ ���������� ���� ��� ������
            if (m_inputCapitals[i].gameObject.activeSelf == false)
                break;

            Capital inputCaptial = (Capital) m_inputCapitals[i].GetTokenBase().GetPid(); //�ڿ� ���� ������ - Pid�� �ڿ��ڵ� ����
            int amount = m_inputCapitals[i].GetAmount(); //�ش� ���Կ� �Ҵ�� ���� ����
            resources.Add((inputCaptial, amount)); //Ʃ�÷� �߰�
        }
        //������ ��ᰡ 2�� �̻��ΰ�� 
        if(resources.Count>=2)
        GamePlayMaster.GetInstance().RuleBook.MixCapital(resources);
    }

    public void OnChangeSelect(List<ShowcaseSlot> _selectedSlot)
    {
        //���ù��� ������ �ް�ʹ�. 
        string log = "";
        for (int i = 0; i < _selectedSlot.Count; i++)
        {
            log += ((Capital)(_selectedSlot[i].GetTokenBase().GetPid())).ToString() + "�� Ŭ����\n";
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
