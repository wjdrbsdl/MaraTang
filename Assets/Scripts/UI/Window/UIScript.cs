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
        //��� ��û�� �޾��� �� ��� ��� �ȵǴµ� �ϴ��� ��ҵǴ°ɷ� �׽�Ʈ
        Switch(false);
        SelectItemInfo scriptInfo = m_select; //�̸� �޾Ƴ���
        m_select = null; //�ʱ�ȭ ����

        //1. ���� ��Ҹ� �����ع����� ��� �������� �ٽ� �� ��ȭâ�� SetScript�� �����̵�. �׸��� ���� ����ġ ������ null�� ����Ǿ� ���׹߻�
        //2. �׷��� ���� �ʱ�ȭ�� ��� ���� 
        if (scriptInfo != null)
            scriptInfo.Cancle();

    }
}
