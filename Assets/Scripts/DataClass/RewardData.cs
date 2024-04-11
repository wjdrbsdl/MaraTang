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
    public  List<TTokenOrder> RewardsList;
    public RewardData(EOrderType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        OrderType = _rewardType;
        RewardsList = new();
        //������ Ÿ�Կ� ����, ������ pid�� value�� �Ű������� �־���� 

        //�ӽ÷� ĳ�� ������ �������� ����
        RewardsList.Add(new TTokenOrder(EOrderType.CharStat, (int)CharStat.Dexility, 10, _chunkNum));
        RewardsList.Add(new TTokenOrder(EOrderType.CharStat, (int)CharStat.Strenth, 10, _chunkNum));
        RewardsList.Add(new TTokenOrder(EOrderType.CharStat, (int)CharStat.Inteligent, 10, _chunkNum));
    }

    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = _rewardType;
        RewardsList = new();
        //������ Ÿ�Կ� ����, ������ pid�� value�� �Ű������� �־���� 

        //�ӽ÷� ĳ�� ������ �������� ����
        RewardsList.Add(new TTokenOrder(EOrderType.CharStat, (int)CharStat.Dexility, 10, _chunkNum));
        RewardsList.Add(new TTokenOrder(EOrderType.CharStat, (int)CharStat.Strenth, 10, _chunkNum));
        RewardsList.Add(new TTokenOrder(EOrderType.CharStat, (int)CharStat.Inteligent, 10, _chunkNum));
    }
}
public struct TTokenOrder
{
    public EOrderType OrderType;
    public ESpawnPosType SpawnPosType;
    public IOrderCustomer OrderCustomer;
    public int SubIdx;
    public int Value;
    public int ChunkNum; //�ƹ� ������ �ƴϸ� - 1

    public TTokenOrder Spawn(EOrderType _spawnType, int _spawnPid, int _spawnCount, ESpawnPosType _spawnPosType, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.OrderType = _spawnType;
        order.SubIdx = _spawnPid;
        order.Value = _spawnCount;
        order.SpawnPosType = _spawnPosType;
        order.ChunkNum = _chunkNum;
        OrderCustomer = null;
        return order;
    }

    public TTokenOrder(EOrderType _type, int _subIdx, int _value, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        OrderType = _type;
        SpawnPosType = ESpawnPosType.Random;
        SubIdx = _subIdx;
        Value = _value;
        ChunkNum = _chunkNum;
        OrderCustomer = null;
    }

    public void SetOrderCustomer(IOrderCustomer _customer)
    {
        OrderCustomer = _customer;
    }

}