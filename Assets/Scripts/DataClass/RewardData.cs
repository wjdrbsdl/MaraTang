using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardData 
{
    public ERewardType RewardType; //보상계열에 따라 주문서 orderType의 방식이 달라지도록? 
    public TTokenOrder RewardOrder;

    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //어떠한 데이트가 들어오면 거기에 맞게 보상 설정. 
    {
        RewardType = _rewardType;

        //어떤 보상식으로 줄진 모르지만 일단 보상아이템 만들고 호출되는부분까지 테스트 
        TOrderItem charItem = new TOrderItem().WriteCharItem(CharStat.Strenth, 50);
        TOrderItem capitalItem = new TOrderItem().WriteCapitalItem(Capital.Green, 150);
        TOrderItem actionItem = new TOrderItem().WriteActionItem(2, 50);
        List<TOrderItem> orderList = new List<TOrderItem>() { charItem, capitalItem, actionItem};

        RewardOrder = new TTokenOrder().Select(EOrderType.ItemSelect, orderList, _chunkNum);
    }
}
