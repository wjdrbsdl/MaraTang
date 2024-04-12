using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RewardData 
{
    public ERewardType RewardType; //����迭�� ���� �ֹ��� orderType�� ����� �޶�������? 
    public TTokenOrder RewardOrder;

    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = _rewardType;

        //� ��������� ���� ������ �ϴ� ��������� ����� ȣ��Ǵºκб��� �׽�Ʈ 
        TOrderItem charItem = new TOrderItem().WriteCharItem(CharStat.Strenth, 50);
        TOrderItem capitalItem = new TOrderItem().WriteCapitalItem(Capital.Green, 150);
        TOrderItem actionItem = new TOrderItem().WriteActionItem(2, 50);
        List<TOrderItem> orderList = new List<TOrderItem>() { charItem, capitalItem, actionItem};

        RewardOrder = new TTokenOrder().Select(EOrderType.ItemSelect, orderList, _chunkNum);
    }
}
