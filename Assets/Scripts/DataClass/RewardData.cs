using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EOrderType
{
  None, Capital, Content, CharStat, SpawnEvent, SpawnMonster
}

public enum ESpawnPosType
{
    //���� �����Ҷ� Ÿ�� 
    Random, Near
}

public enum RewardMethod
{
    Fixed, Select
}


public class RewardData 
{
    public RewardMethod RewardMethod = RewardMethod.Select;
    public ERewardType RewardType;
    public EOrderType OrderType = EOrderType.None;
    public TTokenOrder RewardOrder;

    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = _rewardType;

        //� ��������� ���� ������ �ϴ� ��������� ����� ȣ��Ǵºκб��� �׽�Ʈ 
        TOrderItem charItem = new TOrderItem().WriteCharItem(CharStat.Strenth, 50);
        TOrderItem capitalItem = new TOrderItem().WriteCapitalItem(Capital.Green, 150);
        TOrderItem actionItem = new TOrderItem().WriteActionItem(2, 50);
        List<TOrderItem> orderList = new List<TOrderItem>() { charItem, capitalItem, actionItem};

        RewardOrder = new TTokenOrder().Select(EOrderType.CharStat, orderList, _chunkNum);
    }
}
public struct TTokenOrder
{
    public EOrderType OrderType;
    public ESpawnPosType SpawnPosType;
    public IOrderCustomer OrderCustomer;
    public int SubIdx;
    public int Value;
    public List<TOrderItem> orderItemList;
    public List<int> subIdxList;
    public List<int> valueList;
    public int ChunkNum; //�ƹ� ������ �ƴϸ� - 1
    public int OrderExcuteCount;

    //�ʵ忡 ��ȯ��Ű�� Ÿ�� �ֹ���
    public TTokenOrder Spawn(EOrderType _spawnType, int _spawnPid, int _spawnCount, ESpawnPosType _spawnPosType, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new List<int>();
        order.subIdxList.Add(_spawnPid);
        order.valueList = new List<int>();
        order.valueList.Add(_spawnCount);
        order.OrderType = _spawnType;
        order.SubIdx = _spawnPid;
        order.Value = _spawnCount;
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

    //����
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

public enum ETokenGroup
{
    None, CharStat, ActionToken, GamePlay, Capital    
}

public struct TOrderItem
{
    //�ֹ��� ������ ���� ������ �׸� ����
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