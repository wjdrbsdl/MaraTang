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
        //1. ������ ��� ������ �κ�
        m_window.SetActive(true);
        //2. ��� ������ �����̽� ȣ��
       
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
         DoChangeCapital();
     
    }

    private void DoChangeCapital()
    {
        //2ĭ �� �����ߴ��� üũ
        bool isOk = true;
        for (int i = 0; i < 2; i++)
        {
            //1. ������� Ȱ��ȭ �Ǳ� ������ ���������� ���� ��� ������
            if (m_inputCapitals[i].gameObject.activeSelf == false)
            {
                isOk = false;
                break;
            }
        }

        if (isOk == false)
        {
            Debug.Log("���� �� �ȵ�");
            return;
        }

        //1. ù��°�� ���
        (Capital, int) input = ((Capital)m_inputCapitals[0].GetTokenBase().GetPid(), m_inputCapitals[0].GetAmount());
        //2. �ι�°�� capital�� �ڿ�
        Capital ouput = (Capital)m_inputCapitals[1].GetTokenBase().GetPid();
        //������ ��ᰡ 2�� �̻��ΰ�� 
        GamePlayMaster.GetInstance().RuleBook.ChangeCapital(input, ouput);
    }
    #endregion
}
