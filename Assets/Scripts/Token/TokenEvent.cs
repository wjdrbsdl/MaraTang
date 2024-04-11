using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenEvent : TokenBase
{
    public int m_selectCount = 0; //������ ��
    public TTokenOrder[] m_selectEvent;
    #region �̺�Ʈ ��ū ����
    public TokenEvent()
    {

    }

    //�����͵����� ����
    public TokenEvent(int pid, int value2)
    {
        m_tokenPid = pid;
    }
    
    //���纻 ����

    public TokenEvent(TokenEvent _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
    }

    public static TokenEvent CopyToken(TokenEvent _origin)
    {
        return new TokenEvent(_origin);
    }
    #endregion

    /*
     * �̺�Ʈ ��ū - �����ϸ� �ڵ� �߻��ϴ� �༮ 
     * �������� �־����� �����ϴ� ��� 
     * -> �ڵ� ������ - �־��� ��������, �ϳ��� �ڵ������� ������ ȿ�� 
     * -> �׿� - �־��� �������� �����ϰų�, ��� -> ��ҽ� �ش� �̺�Ʈ�� ��� ���� ��ȯ 
     * �޴� ����� - ����, ����, ����, ��� 
     * 
     * �������� ���� ���� - ���� ��ȭ, ���� ȹ��, ���� ��ȯ �� 
     */

    public void ActiveEvent()
    {
        Debug.Log(m_tokenPid + "�Ǿ��̵� �ߵ�");
    }

    public void SelectEvent()
    {

    }
}
