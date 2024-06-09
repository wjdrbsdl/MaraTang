using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardData : IOrderCustomer
{
    public ERewardType RewardType; //����迭�� ���� �ֹ��� orderType�� ����� �޶�������? 
    public TTokenOrder RewardOrder;

    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = _rewardType;
        if (RewardType.Equals(ERewardType.None))
            return;
        //� ��������� ���� ������ �ϴ� ��������� ����� ȣ��Ǵºκб��� �׽�Ʈ 
        TOrderItem charItem = new TOrderItem().WriteCharItem(CharStat.Strenth, 50);
        TOrderItem capitalItem = new TOrderItem().WriteCapitalItem(Capital.Mineral, 150);
        TOrderItem actionItem = new TOrderItem().WriteActionItem(2, 50);
        List<TOrderItem> orderList = new List<TOrderItem>() { charItem, capitalItem, actionItem};

        RewardOrder = new TTokenOrder().Select(EOrderType.ItemSelect, orderList, _chunkNum);
    }

    public RewardData(ContentData _contentData, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        //1. ����Ʈ �������� ������ Ÿ���� ��������
        RewardType = _contentData.RewardType;
        if (RewardType.Equals(ERewardType.None))
            return;

        //2. ������ Ÿ�Կ� ���� Ÿ���� ����
        EOrderType orderType = EOrderType.None;
        switch (RewardType)
        {
            //������ Ÿ�Կ� ���� OrderType ���缭 ���� 
            
            case ERewardType.CharStat:
            case ERewardType.Capital:
            case ERewardType.ActionToken:
                orderType = EOrderType.ItemSelect;
                break;
            case ERewardType.EventToken:
                orderType = EOrderType.SpawnEvent;
                break;
            case ERewardType.CharToken:
                orderType = EOrderType.SpawnMonster;
                break;
        }

        OrderMaker orderMaker = new();
        RewardOrder = orderMaker.MakeOrder(orderType, _contentData.RewardMainItemList, this, _chunkNum);
        
    }


    public void OnOrderCallBack(OrderReceipt _orderReceipt) //������ ��
    {
        Debug.Log("������ ��");
    }
}
