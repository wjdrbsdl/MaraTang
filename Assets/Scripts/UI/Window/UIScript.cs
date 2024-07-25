using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : UIBase
{
    public Sprite m_charSprite;
    public BtnAnswer m_yesBtn;
    public BtnAnswer m_noBtn;
    public BtnAnswer m_cancleBtn;
    public TMP_Text m_scriptText;

    public void SetScript(ConversationData _scriptData, ISelectCustomer _customer)
    {
        Switch(true);
        string script = _scriptData.GetScript();
        m_scriptText.text = script;
        m_yesBtn.SetButton(_customer);
        m_noBtn.SetButton(_customer);
        m_cancleBtn.SetButton(_customer);

    }

}
