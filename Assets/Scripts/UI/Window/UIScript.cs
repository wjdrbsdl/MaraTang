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
        Switch(false);
        SelectItemInfo scriptInfo = m_select; //미리 받아놓고
        m_select = null; //초기화 진행

        //1. 먼저 취소를 진행해버리면 취소 진행으로 다시 이 대화창이 SetScript로 진행이됨. 그리고 위의 스위치 오프나 null이 진행되어 버그발생
        //2. 그래서 먼저 초기화후 취소 진행 
        if (scriptInfo != null)
            scriptInfo.Cancle();

    }
}
