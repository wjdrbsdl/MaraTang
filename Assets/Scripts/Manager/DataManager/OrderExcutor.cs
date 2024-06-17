using System.Collections.Generic;
using UnityEngine;

public class OrderExcutor
{
    //T 주문서내용을 받으면 내용을 해석해서 집행시키는 부분. 

    public void ExcuteOrder(QuestCondition _questCondition)
    {
        ExcuteOrder(_questCondition.TokenOrder);
    }

    public void ExcuteOrder(RewardData _reward)
    {
        ExcuteOrder(_reward.RewardOrder);
    }

    public void ExcuteOrder(TokenEvent _eventToken)
    {
        ExcuteOrder(_eventToken.TokenOrder);
    }

    public void ExcuteOrder(TTokenOrder _order)
    {
        EOrderType orderType = _order.OrderType;
        //Debug.Log(orderType + "주문 들어옴");
        switch (orderType)
        {
            case EOrderType.ItemAdapt:
                Debug.Log("나열된 아이템들을 적용");
                for (int i = 0; i < _order.orderItemList.Count; i++)
                {
                    ExcuteOrderItem(_order.orderItemList[i]);
                }
                break;
            case EOrderType.ItemSelect:
                Debug.Log("보상으로 들어옴");
                MgUI.GetInstance().ShowItemList(_order);
                break;
            case EOrderType.SpawnMonster:
                ExcuteSpawnMonster(_order);
                break;
            case EOrderType.SpawnEvent:
                ExcuteSpawnEvent(_order);
                break;
        }
    }

    #region 오더 집행
 
    private void ExcuteSpawnMonster(TTokenOrder _order)
    {
        List<TOrderItem> charItemList = _order.orderItemList;
        ESpawnPosType spawnPosType = _order.SpawnPosType;
        int chunkNum = _order.ChunkNum;
        for (int orderNum = 0; orderNum < charItemList.Count; orderNum++)
        {
            //1. 스폰할 몬스터
            int tokenPid = charItemList[orderNum].SubIdx;
            //2. 스폰할 갯수 
            int spawnCount = charItemList[orderNum].Value;
            //3. 스폰 장소 - 청크 최대 숫자중, 스폿 카운트 만큼 뽑기 진행
            List<int[]> spawnPosList = GameUtil.GetSpawnPos(spawnPosType, spawnCount, chunkNum);
            //4. 스폰 진행
            for (int i = 0; i < spawnCount; i++)
            {
                _order.OrderExcuteCount += 1;
                int[] spawnCoord = spawnPosList[i];
                TokenBase spawnToken = null;
              
                spawnToken = MgToken.GetInstance().SpawnCharactor(spawnCoord, tokenPid); //월드 좌표로 pid 토큰 스폰 
           

                CallBackOrder(spawnToken, _order); //스폰된 토큰과 주문서로 고객에게 콜백
            }

        }

    }
    private void ExcuteSpawnEvent(TTokenOrder _order)
    {
        List<TOrderItem> eventOrderList = _order.orderItemList; //할당된 아이템 
        //1. itemList들을 eventToken에 Count만큼씩 할당
        ESpawnPosType spawnPosType = _order.SpawnPosType;
        int chunkNum = _order.ChunkNum;
        List<int[]> spawnPosList = GameUtil.GetSpawnPos(spawnPosType, eventOrderList.Count, chunkNum);
        for (int orderNum = 0; orderNum < eventOrderList.Count; orderNum++)
        {
            TOrderItem spawnEventOrder = eventOrderList[orderNum]; //스폰하려는 이벤트 호출 - pid, value는 내부오더만들때 쓰임
            //1. 스폰할 이벤트
            int tokenPid = spawnEventOrder.SubIdx;
            //2. 스폰 장소 
            int[] spawnPos = spawnPosList[orderNum];
            //3. 이벤트 타입 - 해당 이벤트 입장시 할당받은 아이템으로 뭘할건지 정하기 
            int orderType =  spawnEventOrder.Value;
            //4. 스폰 진행
            _order.OrderExcuteCount += 1; //작업한 수 올리고
            TokenEvent spawnToken = MgToken.GetInstance().SpawnEvent(spawnPos, tokenPid);

            CallBackOrder(spawnToken, _order); //스폰된 토큰과 주문서로 고객에게 콜백


        }

    }
    public void ExcuteOrderItem(TOrderItem _orderItem)
    {
        ETokenGroup tokenGroup = (ETokenGroup)_orderItem.MainIdx;
        int orderSubIdx = _orderItem.SubIdx;
        int orderValue = _orderItem.Value;
        //선택한 아이템이 다시 이벤트 생성 , 몬스터 소환같은거면 어떡함?
        switch (tokenGroup)
        {
            case ETokenGroup.CharStat:
                PlayerManager.GetInstance().GetMainChar().CalStat((CharStat)orderSubIdx, orderValue);
                break;
            case ETokenGroup.Capital:
                Capital rewardCapital = (Capital)orderSubIdx;
                PlayerCapitalData.g_instance.CalCapital(rewardCapital, orderValue);
                break;
            case ETokenGroup.ActionToken:
                break;

        }
    }
    private void CallBackOrder(TokenBase _token, TTokenOrder _order)
    {
        //1. 주문서 고객 정보 있는지 체크
        IOrderCustomer customer = _order.OrderCustomer;
        //2. 고객 정보 없으면 종료
        if (customer == null)
            return;
        //3. 완료된 토큰으로 영수증을 만들고
         OrderReceipt recipt = new(_token, _order);
        //4. 고객에게 콜백 보냄
        customer.OnOrderCallBack(recipt); //고객에게 호출
    }
    
    #endregion
}

public enum EOrderType
{
    None, ItemAdapt, ItemSelect, SpawnMonster, SpawnEvent
}

public enum ESpawnPosType
{
    //무언갈 스폰할때 타입 
    Random, CharRound
}

public enum GiveMethod
{
    //아이템 지급방식
    Fixed, Selecet
}



public struct TTokenOrder
{
    public EOrderType OrderType;
    public ESpawnPosType SpawnPosType;
    public IOrderCustomer OrderCustomer;
    public List<TOrderItem> orderItemList;
    public int ChunkNum; //아무 지정이 아니면 - 1
    public int OrderExcuteCount;

    public TTokenOrder Spawn(EOrderType _orderType, List<TOrderItem> _charList, ESpawnPosType _spawnPosType, int _chunkNum)
    {
        TTokenOrder order = new();
        order.orderItemList = _charList;
        if (order.orderItemList == null)
            order.orderItemList = new();
        order.OrderType = _orderType;
        order.SpawnPosType = _spawnPosType;
        order.ChunkNum = _chunkNum;
        OrderCustomer = null;
        return order;
    }

    public TTokenOrder Select(EOrderType _type, List<TOrderItem> _orderItemList, int _chunkNum)
    {
        TTokenOrder order = new();
        order.orderItemList = _orderItemList;
        if (order.orderItemList == null)
            order.orderItemList = new();
        order.OrderType = _type;
        order.SpawnPosType = ESpawnPosType.Random;
        order.ChunkNum = _chunkNum;
        order.OrderCustomer = null;

        return order;
    }

    public void SetOrderCustomer(IOrderCustomer _customer)
    {
        OrderCustomer = _customer;
    }

}

public enum ETokenGroup
{
    None, CharStat, ActionToken, GamePlay, Capital, Charactor, Event, Nation
}

public struct TOrderItem
{
    //주문서 내부의 개별 아이템 항목 정보
    public ETokenGroup MainIdx;
    public int SubIdx;
    public int Value;

    public TOrderItem (int _tokenGroup, int _subIdx, int _value)
    {
        MainIdx = (ETokenGroup)_tokenGroup;
        SubIdx = _subIdx;
        Value = _value;
    }

    public TOrderItem(ETokenGroup _tokenGroup, int _subIdx, int _value)
    {
        MainIdx = _tokenGroup;
        SubIdx = _subIdx;
        Value = _value;
    }

    public TOrderItem WriteCharItem(CharStat _charIdx, int _value)
    {
        TOrderItem item = new();
        item.MainIdx = ETokenGroup.CharStat;
        item.SubIdx = (int)_charIdx;
        item.Value = _value;
        return item;
    }

    public TOrderItem WriteActionItem(int _actionPid, int _value)
    {
        TOrderItem item = new();
        item.MainIdx = ETokenGroup.ActionToken;
        item.SubIdx = _actionPid;
        item.Value = _value;
        return item;
    }

    public TOrderItem WriteCapitalItem(Capital _capitalIdx, int _value)
    {
        TOrderItem item = new();
        item.MainIdx = ETokenGroup.Capital;
        item.SubIdx = (int)_capitalIdx;
        item.Value = _value;
        return item;
    }
}
