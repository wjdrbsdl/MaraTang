using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectBase : UIBase
{
    protected SelectItemInfo m_selectInfo; //ǥ���� ������ ����

    public void OnConfirm()
    {
        //UI ��ư���� ȣ�� 
        if (m_selectInfo != null)
        {
            bool ableConfirm = m_selectInfo.AbleConfirm();
            //���� �ݷ� �Ծ����� ����
            if (ableConfirm == false)
                return;
        }

        UISwitch(false);
        SelectItemInfo scriptInfo = m_selectInfo;
        m_selectInfo = null; //�ʱ�ȭ ����

        //������ ���� �ʱ�ȭ�� �س��� ������
        //��ҷ� �ٸ� ��ũ���� ȣ�� �Ǵ� ��� ���� SetScript()�� ���õ� ��ũ��Ʈ â�� ����ǹ���
        if (scriptInfo != null)
            scriptInfo.Confirm();
    }

    public override void ReqeustOff()
    {
        //��� ��û�� �޾��� �� ��� ��� �ȵǴµ� �ϴ��� ��ҵǴ°ɷ� �׽�Ʈ


        if (m_selectInfo.AbleCancle() == false)
        {
            //��� �Ұ����� �������� �ݷ�
            return;
        }
        UISwitch(false);
        SelectItemInfo scriptInfo = m_selectInfo;
        m_selectInfo = null; //�ʱ�ȭ ����

        //������ ���� �ʱ�ȭ�� �س��� ������
        //��ҷ� �ٸ� ��ũ���� ȣ�� �Ǵ� ��� ���� SetScript()�� ���õ� ��ũ��Ʈ â�� ����ǹ���
        if (scriptInfo != null)
            scriptInfo.Cancle();

    }
}
