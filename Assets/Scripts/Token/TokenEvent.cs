using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenEvent : TokenBase, IOrderCustomer
{
    public int m_selectCount = 0; //������ ��
    public TTokenOrder TokenOrder;
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
        OrderExcutor.ExcuteOrder(this);
    }

    public void SelectEvent()
    {

    }

    public void MakeEventContent()
    {
        //���� �̺�Ʈ ��ū���� �̺�Ʈ �����? 
        int tempPid = 2;
        int tempCount = 3;
        ESpawnPosType tempSpawnPos = ESpawnPosType.Random;
        TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, tempPid, tempCount, tempSpawnPos, GameUtil.GetMainCharChunkNum());
        TokenOrder.SetOrderCustomer(this);
    }

    public void OrderCallBack(OrderReceipt _orderReceipt) //�̺�Ʈ ��ū ��
    {
        Debug.Log("�̺� ��ū���� �ֹ��Ϸ� �ݹ����");
        //1. ���� ���� �ֹ��� ����� ���� ���Ϳ� �ڽ��� ���� ����Ʈ�� �� ������ ��������. 
    }
}
