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
            case EOrderType.SpawnEvent:
                OrderSpawnToken(_order, orderType);
                break;
        }
    }

    #region 오더 집행
 
    private void OrderSpawnToken(TTokenOrder _order, EOrderType _spawnType)
    {
        List<int> charList = _order.subIdxList;
        List<int> spawnCountList = _order.valueList;
        ESpawnPosType spawnPosType = _order.SpawnPosType;
        int chunkNum = _order.ChunkNum;
        for (int orderNum = 0; orderNum < charList.Count; orderNum++)
        {
            //1. 스폰할 몬스터
            int tokenPid = charList[orderNum];
            //2. 스폰할 갯수 
            int spawnCount = spawnCountList[orderNum];
            //3. 스폰 장소 - 청크 최대 숫자중, 스폿 카운트 만큼 뽑기 진행
            List<int[]> spawnPosList = GameUtil.GetSpawnPos(spawnPosType, spawnCount, chunkNum);
            //4. 스폰 진행
            for (int i = 0; i < spawnCount; i++)
            {
                _order.OrderExcuteCount += 1;
                int[] spawnCoord = spawnPosList[i];
                TokenBase spawnToken = null;
                if(_spawnType.Equals(EOrderType.SpawnMonster))
                    spawnToken = MgToken.GetInstance().SpawnCharactor(spawnCoord, tokenPid); //월드 좌표로 pid 토큰 스폰 
                else if (_spawnType.Equals(EOrderType.SpawnEvent))
                    spawnToken = MgToken.GetInstance().SpawnEvent(spawnCoord, tokenPid);

                CallBackOrder(spawnToken, _order); //스폰된 토큰과 주문서로 고객에게 콜백
            }

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

    #region 오더 아이템 선택시
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

public enum ETokenGroup
{
    None, CharStat, ActionToken, GamePlay, Capital
}

public struct TTokenOrder
{
    public EOrderType OrderType;
    public ESpawnPosType SpawnPosType;
    public IOrderCustomer OrderCustomer;
    public List<TOrderItem> orderItemList;
    public List<int> subIdxList;
    public List<int> valueList;
    public int ChunkNum; //아무 지정이 아니면 - 1
    public int OrderExcuteCount;

    //필드에 소환시키는 타입 주문서
    public TTokenOrder Spawn(EOrderType _spawnType, int _spawnPid, int _spawnCount, ESpawnPosType _spawnPosType, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new List<int>();
        order.subIdxList.Add(_spawnPid);
        order.valueList = new List<int>();
        order.valueList.Add(_spawnCount);
        order.OrderType = _spawnType;
        order.SpawnPosType = _spawnPosType;
        order.ChunkNum = _chunkNum;
        OrderCustomer = null;
        return order;
    }
    public TTokenOrder Spawn(EOrderType _spawnType, List<int> _spawnPid, List<int> _spawnCount, ESpawnPosType _spawnPosType, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new List<int>(_spawnPid);
        order.valueList = new List<int>(_spawnCount);
        order.OrderType = _spawnType;
        order.SpawnPosType = _spawnPosType;
        order.ChunkNum = _chunkNum;
        OrderCustomer = null;
        return order;
    }

    //선택
    public TTokenOrder Select(EOrderType _type, TOrderItem orderItem, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new();
        order.valueList = new();
        order.orderItemList = new();

        order.orderItemList.Add(orderItem);
        order.OrderType = _type;
        order.SpawnPosType = ESpawnPosType.Random;

        order.ChunkNum = _chunkNum;
        order.OrderCustomer = null;

        return order;
    }
    public TTokenOrder Select(EOrderType _type, List<TOrderItem> _orderItemList, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new List<int>();
        order.valueList = new List<int>();
        order.orderItemList = new List<TOrderItem>(_orderItemList);
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

public struct TOrderItem
{
    //주문서 내부의 개별 아이템 항목 정보
    public ETokenGroup MainIdx;
    public int SubIdx;
    public int Value;

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
