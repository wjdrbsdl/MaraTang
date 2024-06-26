using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UISelectItem : UIBase, ISelectCustomer
{
    //해당 플레이어가 지닌 자원을 종류 수량을 선택하는 부분. 
    [SerializeField]
    private SelectSlot m_showSlotSample;
    [SerializeField]
    private Transform m_grid;
    [SerializeField]
    private SelectSlot[] m_selectSlots;
    private SelectItemInfo m_selectInfo;
   
    public void SetSelectedInfo(SelectItemInfo _selectInfo)
    {
        base.OpenWindow();
        m_selectInfo = _selectInfo;
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<SelectSlot>(ref m_selectSlots, m_showSlotSample.gameObject, m_selectInfo.ItemList.Count, m_grid);
        //2. 리스트대로 슬랏에 표기
        SetSlots();
        //3. 선택한 아이템들은 따로 추가 표기
        SetSelectSlots();
    }

    private void SetSlots()
    {
        int itemCount = m_selectInfo.ItemList.Count;
        for (int i = 0; i < itemCount; i++)
        {
            TOrderItem orderItem = m_selectInfo.ItemList[i]; //정보 표기할 아이템 
            ISelectCustomer customer = this;
            int index = i;
            m_selectSlots[i].gameObject.SetActive(true);
            m_selectSlots[i].SetSlot(orderItem, customer, index);
        }
        for (int i = itemCount; i < m_selectSlots.Length; i++)
        {
            m_selectSlots[i].gameObject.SetActive(false);
        }
    }

    private void SetSelectSlots()
    {
        for (int i = 0; i < m_selectInfo.SelectedIndex.Count; i++)
        {
            int slotIndex = m_selectInfo.SelectedIndex[i];
            m_selectSlots[slotIndex].SetSelectState();
        }
    }

    public void OnSelectCallBack(int _slotIndex)
    {
        m_selectInfo.AddChooseItem(_slotIndex);
        //다시 정보 리셋 
        SetSlots();
        SetSelectSlots();
    }

    public void OnConfirm()
    {
        if(m_selectInfo != null)
        m_selectInfo.Confirm();

        Switch(false);
        m_selectInfo = null;
    }
}

