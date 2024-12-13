using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : UISelectBase
{
    public Sprite m_charSprite;
    public TMP_Text m_scriptText;
    public int m_serialNum = FixedValue.No_VALUE;

    public void SetScript(ConversationData _scriptData, int _serialNum)
    {
        UISwitch(true);
        string script = _scriptData.GetScript();
        m_scriptText.text = script;
        m_selectInfo = new SelectItemInfo(null, false, 0, 0, _serialNum); ;
        m_serialNum = _serialNum;
    }

}
