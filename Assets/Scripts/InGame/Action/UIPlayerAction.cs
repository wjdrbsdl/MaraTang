using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIPlayerAction : UIBase
{
    //해당 플레이어가 지닌 자원을 종류 수량을 선택하는 부분. 
    [SerializeField]
    private InvenSlot m_invenSlotSample;
    [SerializeField]
    private Transform m_grid;
    [SerializeField]
    private InvenSlot[] m_invenSlots;

    public void SetPlayerAction()
    {
        UISwitch(true);
        List<TokenAction> actionList = PlayerManager.GetInstance().GetMainChar().GetActionList();
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<InvenSlot>(ref m_invenSlots, m_invenSlotSample.gameObject, actionList.Count, m_grid);
        //2. 리스트대로 슬랏에 표기
        SetSlots(actionList);
    }

    private void SetSlots(List<TokenAction> _equiptList)
    {
        int itemCount = _equiptList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            int index = i;
            m_invenSlots[i].gameObject.SetActive(true);
            m_invenSlots[i].SetInvenSlot(_equiptList[i], index);
        }
        for (int i = itemCount; i < m_invenSlots.Length; i++)
        {
            m_invenSlots[i].gameObject.SetActive(false);
        }
    }

}

