using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIScript : UIBase
{
    public Sprite m_charSprite;
    public TMP_Text m_scriptText;
    public SelectItemInfo m_select;

    public void SetScript(ConversationData _scriptData)
    {
        Switch(true);
        string script = _scriptData.GetScript();
        m_scriptText.text = script;
        m_select = null;
    }

    public void SetSelectInfo(SelectItemInfo _selectInfo)
    {
        m_select = _selectInfo;
    }

    public void BtnConfirm()
    {
        if (m_select != null)
            m_select.Confirm();

        Switch(false);
        m_select = null;
    }

    public override void ReqeustOff()
    {
        //취소 요청을 받았을 때 얘는 취소 안되는데 일단은 취소되는걸로 테스트
        if (m_select != null)
            m_select.Cancle();

        Switch(false);
        m_select = null;

    }
}
