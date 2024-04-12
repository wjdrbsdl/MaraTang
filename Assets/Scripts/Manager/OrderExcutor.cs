using System.Collections.Generic;
using UnityEngine;

public static class OrderExcutor
{
    //T 주문서내용을 받으면 내용을 해석해서 집행시키는 부분. 

    public static void ExcuteOrder(QuestCondition _questCondition)
    {
        ExcuteOrder(_questCondition.TokenOrder);
    }

    public static void ExcuteOrder(TokenEvent _eventToken)
    {
        ExcuteOrder(_eventToken.TokenOrder);
    }

    public static void ExcuteOrder(TTokenOrder _order)
    {
        EOrderType orderType = _order.OrderType;
        //Debug.Log(orderType + "주문 들어옴");
        switch (orderType)
        {
            case EOrderType.Capital:
                break;
            case EOrderType.CharStat:
                break;
            case EOrderType.Content:
                break;
            case EOrderType.SpawnEvent:
                OrderEventSpawn(_order);
                break;
            case EOrderType.SpawnMonster:
                OrderSpawnMonster(_order);
                break;
        }
    }

    private static void OrderEventSpawn(TTokenOrder _order)
    {
        //1. 스폰할 이벤트 타입 
        int tempEventPid = 1;
        //2. 스폰할 갯수 
        int tempMakeCount = 3;
        //3. 스폰 장소 
        int[] b = MgToken.GetInstance().GetMainChar().GetMapIndex();
        int[] rightUp = GameUtil.GetPosFromDirect(b, TileDirection.RightUp);
        int[] rightDown = GameUtil.GetPosFromDirect(b, TileDirection.RightDown);
        int[] left = GameUtil.GetPosFromDirect(b, TileDirection.Left);
        List<int[]> tempSpawnPosList = new List<int[]>();
        tempSpawnPosList.Add(rightUp);
        tempSpawnPosList.Add(rightDown);
        tempSpawnPosList.Add(left);

        for (int i = 0; i < tempMakeCount; i++)
        {
          TokenEvent eventToken = MgToken.GetInstance().SpawnEvent(tempSpawnPosList[i], tempEventPid);
            CallBackOrder(eventToken, _order);
        }
        

    }

    private static void OrderSpawnMonster(TTokenOrder _order)
    {
        //1. 스폰할 몬스터
        int charPid = _order.SubIdx;
        //2. 스폰할 갯수 
        int spawnCount = _order.Value;
        //3. 스폰 장소 - 청크 최대 숫자중, 스폿 카운트 만큼 뽑기 진행
        List<int> randomPosList = GameUtil.GetRandomNum(25, spawnCount);
        //4. 구역 뽑기
        Chunk madeChunk = MGContent.GetInstance().GetChunk(_order.ChunkNum);
        //5. 스폰 진행
        for (int i = 0; i < spawnCount; i++)
        {
            int chunkTileNum = randomPosList[i]; //청크 내부에서 해당 타일의 idx
            int[] tilePos = GameUtil.GetXYPosFromIndex(madeChunk.tiles.GetLength(0), chunkTileNum);//청크 기준으로 좌표 도출 
            int[] spawnCoord = madeChunk.tiles[tilePos[0], tilePos[1]].GetMapIndex();//청크 좌표를 월드 좌표로 전환
            TokenChar questMonster = MgToken.GetInstance().SpawnCharactor(spawnCoord, charPid); //월드 좌표로 pid 캐릭 스폰 
            CallBackOrder(questMonster, _order); //스폰된 몬스터와 주문서로 고객에게 콜백
            //콜백받은 고객이 해당 부속물에 알아서 남은 작업 진행. 
        }

    }

    private static void CallBackOrder(TokenBase _token, TTokenOrder _order)
    {
        //1. 주문서 고객 정보 있는지 체크
        IOrderCustomer customer = _order.OrderCustomer;
        //2. 고객 정보 없으면 종료
        if (customer == null)
            return;
        //3. 완료된 토큰으로 영수증을 만들고
        OrderReceipt recipt = new(_token, _order);
        //4. 고객에게 콜백 보냄
        customer.OrderCallBack(recipt); //고객에게 호출
    }

}
