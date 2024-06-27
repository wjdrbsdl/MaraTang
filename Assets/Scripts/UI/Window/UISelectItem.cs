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

    //SelectItemInfo 정보값을 표기, SelectSlot으로부터 입력을 받으며 진행. 
    #region 슬랏으로 정보 표기
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
        bool isFixed = m_selectInfo.IsFixedValue;
        for (int i = 0; i < m_selectInfo.SelectedIndex.Count; i++)
        {
            int slotIndex = m_selectInfo.SelectedIndex[i];
            int selectValue = m_selectInfo.SelectedValue[i];
            m_selectSlots[slotIndex].SetSelectState(selectValue, isFixed);
        }
    }
    #endregion

    #region 선택클래스에 정보 입력 
    public void OnSelectCallBack(int _slotIndex)
    {
        m_selectInfo.AddChooseItem(_slotIndex);
        //다시 정보 리셋 
        SetSlots();
        SetSelectSlots();
    }

    public void OnChangeValueCallBack(int _slotIndex, int _value)
    {
        //해당 값이 바뀐 경우. 
        int max = m_selectInfo.ItemList[_slotIndex].Value; //기존의 값이 최댓값
        int min = 1;
        int final = Mathf.Clamp(_value, min, max);
        if(final != _value)
        {
            //입력된 값이 다르면, 입력된값을 변경 시킴
            m_selectSlots[_slotIndex].SetSelectValue(final); //직접 text 변경한건 재 콜백이 일어나지 않음.
        }
        m_selectInfo.SetSelectValue(_slotIndex, final);
    }

    public void OnConfirm()
    {
        if(m_selectInfo != null)
        m_selectInfo.Confirm();

        Switch(false);
        m_selectInfo = null;
    }
    #endregion
}

