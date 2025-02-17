﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerAction : UIBase, iInventoryUI
{
    //해당 플레이어가 지닌 자원을 종류 수량을 선택하는 부분. 
    [SerializeField]
    private InvenSlot m_invenSlotSample;
    [SerializeField]
    private Transform m_grid;
    [SerializeField]
    private InvenSlot[] m_invenSlots;

    [Header("정보표기")]
    [SerializeField] private TokenAction m_selectAction;
    [SerializeField] private TMP_Text m_itemName; //나중에 아이콘으로 대체될부분
    [SerializeField] private TMP_Text m_itemInfo; //나중에 아이콘으로 대체될부분

    public void SetPlayerAction()
    {
        UISwitch(true);
        List<TokenAction> actionList = PlayerManager.GetInstance().GetMainChar().GetActionList();
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<InvenSlot>(ref m_invenSlots, m_invenSlotSample.gameObject, actionList.Count, m_grid);
        //2. 리스트대로 슬랏에 표기
        SetSlots(actionList);
    }

    public void SelectAction(TokenAction _action)
    {
        //선택한 아이템 정보 세팅
        m_itemName.text = _action.GetItemName();
        m_itemInfo.text = string.Format(_action.GetItemInfo(),"그 테스트 ","1567",5);
    }

    private void SetSlots(List<TokenAction> _equiptList)
    {
        int itemCount = _equiptList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            int index = i;
            m_invenSlots[i].gameObject.SetActive(true);
            m_invenSlots[i].SetInvenSlot(_equiptList[i], this);
        }
        for (int i = itemCount; i < m_invenSlots.Length; i++)
        {
            m_invenSlots[i].gameObject.SetActive(false);
        }
    }

    public void OnClickInventorySlot(TokenBase _token)
    {
        SelectAction((TokenAction)_token);
    }
}

