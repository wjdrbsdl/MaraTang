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
        //UI ��ư���� ȣ�� 
        if (m_select != null)
        {
            bool ableConfirm = m_select.Confirm();
            //���� �ݷ� �Ծ����� ����
            if (ableConfirm == false)
                return;
        }

        UISwitch(false);
        m_select = null;
    }

    public override void ReqeustOff()
    {
        //��� ��û�� �޾��� �� ��� ��� �ȵǴµ� �ϴ��� ��ҵǴ°ɷ� �׽�Ʈ
        UISwitch(false);
        SelectItemInfo scriptInfo = m_select; //�̸� �޾Ƴ���
        m_select = null; //�ʱ�ȭ ����

        //������ ���� �ʱ�ȭ�� �س��� ������
        //��ҷ� �ٸ� ��ũ���� ȣ�� �Ǵ� ��� ���� SetScript()�� ���õ� ��ũ��Ʈ â�� ����ǹ���
        if (scriptInfo != null)
            scriptInfo.Cancle();

    }
}
