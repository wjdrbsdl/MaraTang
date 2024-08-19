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
        UISwitch(true);
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
        //UI 버튼으로 호출 
        if (m_select != null)
        {
            bool ableConfirm = m_select.AbleConfirm();
            //컨펌 반려 먹었으면 빠꾸
            if (ableConfirm == false)
                return;
        }

        UISwitch(false);
        SelectItemInfo scriptInfo = m_select;
        m_select = null; //초기화 진행

        //위에서 먼저 초기화를 해놓지 않으면
        //취소로 다른 스크립이 호출 되는 경우 새로 SetScript()로 세팅된 스크립트 창이 종료되버림
        if (scriptInfo != null)
            scriptInfo.Confirm();
    }

    public override void ReqeustOff()
    {
        //취소 요청을 받았을 때 얘는 취소 안되는데 일단은 취소되는걸로 테스트


        if(m_select.AbleCancle() == false)
        {
            //취소 불가능한 선택지면 반려
            return;
        }
        UISwitch(false);
        SelectItemInfo scriptInfo = m_select;
        m_select = null; //초기화 진행

        //위에서 먼저 초기화를 해놓지 않으면
        //취소로 다른 스크립이 호출 되는 경우 새로 SetScript()로 세팅된 스크립트 창이 종료되버림
        if (scriptInfo != null)
            scriptInfo.Cancle();

    }
}
