using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class InvenSlot : SlotBase
{
    public TokenType m_tokenType; //할당된 아이템의 tokenType
    public int m_itemPid = 0;
    public TMP_Text m_itemName; //나중에 아이콘으로 대체될부분
    public int m_slotIndex = 0;
    public iInventoryUI m_ui;
    private void Start()
    {
        m_isDragSlot = true;
    }

    public void SetInvenSlot(TokenBase _base, iInventoryUI _invenUI = null)
    {
        m_tokenType = _base.GetTokenType();
        m_itemPid = _base.GetPid();
        m_token = _base;
        m_ui = _invenUI;
        Naming();
    }

    private void Naming()
    {
        m_itemName.text = m_tokenType + " : " + m_itemPid;
    }

    public override void OnLeftClick()
    {
      //  Debug.Log(m_token.GetItemName() + "눌림");
        if (m_ui != null && m_token != null)
            m_ui.OnClickInventorySlot(m_token);
    }
}
