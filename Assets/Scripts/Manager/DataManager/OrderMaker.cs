using System.Collections.Generic;
using UnityEngine;

public class OrderMaker
{
    //T 주문서내용을 받으면 내용을 해석해서 집행시키는 부분. 

    public TTokenOrder MakeOrder(EOrderType _orderType, List<TOrderItem> _orderItemList, IOrderCustomer _customer = null, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder madeOrder = new();

        ESpawnPosType spawnPos = ESpawnPosType.Random;
        switch (_orderType)
        {
            //이벤트토큰 생성하는 경우
            case EOrderType.SpawnEvent:
                spawnPos = ESpawnPosType.CharRound;
                madeOrder = new TTokenOrder().Spawn(EOrderType.SpawnEvent, _orderItemList, spawnPos, _chunkNum);
                break;
            case EOrderType.SpawnMonster:
                madeOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, _orderItemList, spawnPos, _chunkNum);
                break;
        }

        madeOrder.SetOrderCustomer(_customer);
        return madeOrder;
    }

}