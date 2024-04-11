using System.Collections.Generic;
using UnityEngine;

public static class OrderExcutor
{
    //T 주문서내용을 받으면 내용을 해석해서 집행시키는 부분. 

    public static void ExcuteOrder(QuestCondition _questCondition)
    {
        ExcuteOrder(_questCondition.TokenOrder);
    }

    public static void ExcuteOrder(TTokenOrder _order)
    {
        EOrderType orderType = _order.OrderType;
        Debug.Log(orderType + "주문 들어옴");
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
        //3. 스폰 장수 
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
            MgToken.GetInstance().SpawnEvent(tempSpawnPosList[i], tempEventPid);
        }
        

    }

    private static void OrderSpawnMonster(TTokenOrder _order)
    {
        //1. 스폰할 이벤트 타입 
        int tempEventPid = _order.SubIdx;
        //2. 스폰할 갯수 
        int tempMakeCount = _order.Value;
        //3. 스폰 장소 
        List<int> randomPosList = GameUtil.GetRandomNum(25, tempMakeCount);
        Chunk madeChunk = MGContent.GetInstance().GetChunk(_order.ChunkNum);
        for (int i = 0; i < tempMakeCount; i++)
        {
            int ranPos = randomPosList[i]; //타일번호를 뽑고 해당 번호를 타일좌표로 반환해야함
            int[] tilePos = GameUtil.GetXYPosFromIndex(madeChunk.tiles.GetLength(0), ranPos);
            int[] spawnCoord = madeChunk.tiles[tilePos[0], tilePos[1]].GetMapIndex();
            TokenChar questMonster = MgToken.GetInstance().SpawnCharactor(spawnCoord, tempEventPid); //몬스터의 경우 사망시에 설치
            CallBackOrder(questMonster, _order);
            //퀘스트에 생성된 부속물을 귀속시키는 부분 
            //현재 order에 퀘스트 할당이 안되있어서 연결이 불가
            
            //questMonster.SetQuest(m_Quest);
            //questMonster.QuestPid = m_Quest.QuestPid;
            //m_Quest.TempQuestTokens.Add(questMonster);
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
        OrderReceipt recipt = new();
        recipt.madeToken = _token;
        //4. 고객에게 콜백 보냄
        customer.OrderCallBack(recipt);
    }

}
