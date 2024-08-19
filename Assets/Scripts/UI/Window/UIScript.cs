using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : UISelectBase
{
    public Sprite m_charSprite;
    public TMP_Text m_scriptText;

    public void SetScript(ConversationData _scriptData)
    {
        UISwitch(true);
        string script = _scriptData.GetScript();
        m_scriptText.text = script;
        m_selectInfo = null;
    }

    public void SetSelectInfo(SelectItemInfo _selectInfo)
    {
        m_selectInfo = _selectInfo;
    }

}
