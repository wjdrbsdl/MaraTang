using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventStat
{
    ETokenType, ItemPID, ItemValue, GiveMethod
}

public class TokenEvent : TokenBase, IOrderCustomer
{
    public int m_selectCount = 0; //������ ��
    public TTokenOrder TokenOrder;

    #region �̺�Ʈ ��ū ����
    public TokenEvent()
    {
        m_tokenType = TokenType.Event;
    }

    //�����͵����� ����
    public TokenEvent(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]); //��Ʈ �����ͻ� 0��°�� pid
        m_itemName = valueCode[1]; //1�� �̸�
        m_tokenType = TokenType.Event;
        m_tokenIValues = new int[System.Enum.GetValues(typeof(EventStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
    }

    //���纻 ����

    public TokenEvent(TokenEvent _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = _masterToken.m_tokenType;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //������ ������ ���� ����� ��ü ���� �迭 �� ����. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //���ݰ� ����
    }

    public static TokenEvent CopyToken(TokenEvent _origin)
    {
        return new TokenEvent(_origin);
    }
    #endregion

    public void ActiveEvent()
    {
        Debug.Log(m_tokenPid + "�Ǿ��̵� �ߵ�");
        OrderExcutor orderExcutor = new();
        orderExcutor.ExcuteOrder(this);
        SendQuestCallBack();
    }

    public void RemoveEvent()
    {
        //0. ������Ʈ ����
        if (m_object != null)
            m_object.DestroyObject();

        //1. ����� ó��
        SendQuestCallBack();

        //2. ������ ���� ����
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.DeleteEnterEvent();
        MgToken.GetInstance().RemoveCharToken(this);
    }

    public override void CleanToken()
    {
        base.CleanToken();
        //0. ������Ʈ ����
        if (m_object != null)
            m_object.DestroyObject();
        //2. ������ ���� ����
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.DeleteEnterEvent();
        MgToken.GetInstance().RemoveCharToken(this);
    }

    public void MakeEventContent(EOrderType _orderType, List<TOrderItem> _itemList)
    {
        //���� �̺�Ʈ�� PID�� ������ ���� ���� TTokenOrder ����
       
        if (_orderType.Equals(EOrderType.SpawnMonster))
        {
            //����ϴ� �׷��� ĳ�����ΰ�� - ĳ���� ���� �ֹ����� ����
            TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, _itemList, ESpawnPosType.Random, GameUtil.GetMainCharChunkNum());
        }
        
        TokenOrder.SetOrderCustomer(this);
    }

    public void OnOrderCallBack(OrderReceipt _orderReceipt) //�̺�Ʈ ��ū ��
    {
        Debug.Log("�̺� ��ū�� �ݹ� ���� Ƚ�� "+_orderReceipt.Order.OrderExcuteCount);
        EOrderType orderType = _orderReceipt.Order.OrderType; //�����ߴ� ���� Ÿ��
        //1. ���� ���� �ֹ��� ����� ���� ���Ϳ� �ڽ��� ���� ����Ʈ�� �� ������ ��������. 
    }
}
