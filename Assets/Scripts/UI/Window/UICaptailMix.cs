using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICaptailMix : UIBase
{
   
    [SerializeField] private InputSlot[] m_inputCapitals;
    [SerializeField] private GameObject m_sampleSlot;
    [SerializeField] private Transform m_box;
    [SerializeField] private RectTransform m_rectTrans;

    private InputSlot m_selectInputSlot = null;
    public void SetChefUI(TokenTile _tile, TokenTileAction _actionToken)
    {
        m_window.SetActive(true);
       int tempSelectMaxCount = 2;
        //1. ��ǲ ���� ����
        MakeSamplePool<InputSlot>(ref m_inputCapitals, m_sampleSlot.gameObject, tempSelectMaxCount, m_box);
        //2. ��ǲ ������ �̺�Ʈ �Ҵ�
        for (int i = 0; i < tempSelectMaxCount; i++)
        {
            m_inputCapitals[i].gameObject.SetActive(true);
            m_inputCapitals[i].SetEventOnInput(OnClickInputSlot, OnInputSlot);
        }
        OnClickInputSlot(m_inputCapitals[0]);

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
        Debug.Log("�ش� ��ǲ������ ���� ����");
        _inputSlot.selectedText.text = _inputSlot.ShowCase.GetTokenBase().GetItemName();
    }

    private void ResetRecord()
    {
        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            m_inputCapitals[i].gameObject.SetActive(false);
        }
    }

    #region ��ư ȣ��
    public void OnBtnDoJob()
    {
        DoMixCapital();
    }

    private void DoMixCapital()
    {
        List<(Capital, int)> resources = new(); //���� ���� ����

        for (int i = 0; i < m_inputCapitals.Length; i++)
        {
            //1. ������� Ȱ��ȭ �Ǳ� ������ ���������� ���� ��� ������
            if (m_inputCapitals[i].gameObject.activeSelf == false)
                break;

            Capital inputCaptial = (Capital)m_inputCapitals[i].GetTokenBase().GetPid(); //�ڿ� ���� ������ - Pid�� �ڿ��ڵ� ����
            int amount = m_inputCapitals[i].GetAmount(); //�ش� ���Կ� �Ҵ�� ���� ����
            resources.Add((inputCaptial, amount)); //Ʃ�÷� �߰�
        }
        //������ ��ᰡ 2�� �̻��ΰ�� 
        if (resources.Count >= 2)
            GamePlayMaster.GetInstance().RuleBook.MixCapital(resources);
    }

    #endregion
}
