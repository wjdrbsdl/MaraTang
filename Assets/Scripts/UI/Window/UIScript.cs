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
      //  Debug.Log(_serialNum + "��� ���");
        UISwitch(true);
        string script = _scriptData.GetScript();
        m_scriptText.text = script;
        m_selectInfo = new SelectItemInfo(null, false, 0, 0, _serialNum); ;
        m_serialNum = _serialNum;
    }

    public void OnConfirmScript()
    {

        bool ableConfirm = m_selectInfo.AbleConfirm();
        //���� �ݷ� �Ծ����� ����
        if (ableConfirm == false)
            return;
        
        //��ҷ� �ٸ� ��ũ���� ȣ�� �Ǵ� ��� ���� SetScript()�� ���õ� ��ũ��Ʈ â�� ����ǹ���
        if (m_selectInfo != null)
            m_selectInfo.Confirm();

        //����Ʈ ����� ���� ��ȭ�� ȣ��ɶ� �ش� UI�� �����ֱ� ������ ���࿡ �ɸ�. 

        m_selectInfo = null; //�ʱ�ȭ ����

        //���鼭 mg��翡 ���� ��ȭ�� ��û 
        UISwitch(false);
    }

    public override void OffWindow()
    {
        base.OffWindow();//���ÿ��� ���鼭
        Debug.Log("���� ��� ��û");
        MGConversation.GetInstance().RequestNextScrtip();
    }

}
