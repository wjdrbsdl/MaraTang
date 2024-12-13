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
      //  Debug.Log(_serialNum + "대사 출력");
        UISwitch(true);
        string script = _scriptData.GetScript();
        m_scriptText.text = script;
        m_selectInfo = new SelectItemInfo(null, false, 0, 0, _serialNum); ;
        m_serialNum = _serialNum;
    }

    public void OnConfirmScript()
    {

        bool ableConfirm = m_selectInfo.AbleConfirm();
        //컨펌 반려 먹었으면 빠꾸
        if (ableConfirm == false)
            return;
        
        //취소로 다른 스크립이 호출 되는 경우 새로 SetScript()로 세팅된 스크립트 창이 종료되버림
        if (m_selectInfo != null)
            m_selectInfo.Confirm();

        //퀘스트 연계로 다음 대화가 호출될때 해당 UI가 켜져있기 때문에 예약에 걸림. 

        m_selectInfo = null; //초기화 진행

        //끄면서 mg대사에 다음 대화를 요청 
        UISwitch(false);
    }

    public override void OffWindow()
    {
        base.OffWindow();//스택에서 뺴면서
        Debug.Log("다음 대사 요청");
        MGConversation.GetInstance().RequestNextScrtip();
    }

}
