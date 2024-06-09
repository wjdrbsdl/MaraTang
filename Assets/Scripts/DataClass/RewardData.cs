using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardData : IOrderCustomer
{
    public ERewardType RewardType; //보상계열에 따라 주문서 orderType의 방식이 달라지도록? 
    public TTokenOrder RewardOrder;

    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //어떠한 데이트가 들어오면 거기에 맞게 보상 설정. 
    {
        RewardType = _rewardType;
        if (RewardType.Equals(ERewardType.None))
            return;
        //어떤 보상식으로 줄진 모르지만 일단 보상아이템 만들고 호출되는부분까지 테스트 
        TOrderItem charItem = new TOrderItem().WriteCharItem(CharStat.Strenth, 50);
        TOrderItem capitalItem = new TOrderItem().WriteCapitalItem(Capital.Mineral, 150);
        TOrderItem actionItem = new TOrderItem().WriteActionItem(2, 50);
        List<TOrderItem> orderList = new List<TOrderItem>() { charItem, capitalItem, actionItem};

        RewardOrder = new TTokenOrder().Select(EOrderType.ItemSelect, orderList, _chunkNum);
    }

    public RewardData(ContentData _contentData, int _chunkNum = MGContent.NO_CHUNK_NUM) //어떠한 데이트가 들어오면 거기에 맞게 보상 설정. 
    {
        //1. 컨텐트 정보에서 리워드 타입을 가져오고
        RewardType = _contentData.RewardType;
        if (RewardType.Equals(ERewardType.None))
            return;

        //2. 리워드 타입에 오더 타입을 변경
        EOrderType orderType = EOrderType.None;
        switch (RewardType)
        {
            //리워드 타입에 따라서 OrderType 맞춰서 세팅 
            
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


    public void OnOrderCallBack(OrderReceipt _orderReceipt) //리워드 고객
    {
        Debug.Log("리워드 고객");
    }
}
