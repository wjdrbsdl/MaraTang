using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIOneByeSelect : UIBase
{
    //해당 플레이어가 지닌 자원을 종류 수량을 선택하는 부분. 
    [SerializeField]
    private OneSelectSlot m_showSlotSample;
    [SerializeField]
    private Transform m_grid;
    [SerializeField]
    private OneSelectSlot[] m_selectSlots;
    private OneBySelectInfo m_onebyselectInfo;

    //SelectItemInfo 정보값을 표기, SelectSlot으로부터 입력을 받으며 진행. 
    #region 슬랏으로 정보 표기
    public void SetSelectedInfo(OneBySelectInfo _selectInfo)
    {
        UISwitch(true);
        m_onebyselectInfo = _selectInfo;
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<OneSelectSlot>(ref m_selectSlots, m_showSlotSample.gameObject, m_onebyselectInfo.ItemList.Count, m_grid);
        //2. 리스트대로 슬랏에 표기
        SetSlots();
    }

    public void OnSelectItem(int _index)
    {
        //아이템리스트에 대응되는 슬롯이 클릭된 경우 index가 전달됨

    }

    private void SetSlots()
    {
        int itemCount = m_onebyselectInfo.ItemList.Count;
      
        for (int i = 0; i < itemCount; i++)
        {
            TOrderItem orderItem = m_onebyselectInfo.ItemList[i]; //정보 표기할 아이템 
            //ISelectCustomer customer = this;
            int index = i;
            
            m_selectSlots[i].gameObject.SetActive(true);
            m_selectSlots[i].SetSlot(orderItem, this, index);
        }
        for (int i = itemCount; i < m_selectSlots.Length; i++)
        {
            m_selectSlots[i].gameObject.SetActive(false);
        }
    }


    public void ResetSlot()
    {
        SetSlots();
    }


    #endregion
}

