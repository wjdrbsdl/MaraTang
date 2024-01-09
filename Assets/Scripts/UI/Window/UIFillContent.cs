using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFillContent : UIBase
{
    public GameObject m_contentForm;
    public ShowSlot m_charSlot;
    public ShowSlot m_actionSlot;
    public TargetSlot m_targetSlot;
    public void SetContentForm(TokenChar _char, TokenAction _action)
    {
        m_contentForm.SetActive(true);
        m_charSlot.SetSlot(_char);
        m_actionSlot.SetSlot(_action);
        m_targetSlot.ClearSlot();
    }

    public void AddContent(TokenBase _contentTarget)
    {
        m_targetSlot.gameObject.SetActive(true);
        m_targetSlot.SetSlot(_contentTarget);
    }

    public void ActionConfirmButton()
    {
        PlayerManager.g_instance.ConfirmAction();
    }
}
