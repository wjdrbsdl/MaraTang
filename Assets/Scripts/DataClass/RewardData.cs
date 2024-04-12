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
 
    public RewardData(ERewardType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = _rewardType;
        RewardsList = new();
        //������ Ÿ�Կ� ����, ������ pid�� value�� �Ű������� �־���� 

        //�ӽ÷� ĳ�� ������ �������� ����
        RewardsList.Add(new TTokenOrder().Select(EOrderType.CharStat, (int)CharStat.Dexility, 10, _chunkNum));
        RewardsList.Add(new TTokenOrder().Select(EOrderType.CharStat, (int)CharStat.Strenth, 10, _chunkNum));
        RewardsList.Add(new TTokenOrder().Select(EOrderType.CharStat, (int)CharStat.Inteligent, 10, _chunkNum));
    }
}
public struct TTokenOrder
{
    public EOrderType OrderType;
    public ESpawnPosType SpawnPosType;
    public IOrderCustomer OrderCustomer;
    public int SubIdx;
    public int Value;
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
    public TTokenOrder Select(EOrderType _type, int _subIdx, int _value, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new List<int>(_subIdx);
        order.subIdxList.Add(_subIdx);
        order.valueList = new List<int>(_value);
        order.valueList.Add(_value);
        order.OrderType = _type;
        order.SpawnPosType = ESpawnPosType.Random;
        order.SubIdx = _subIdx;
        order.Value = _value;
        order.ChunkNum = _chunkNum;
        order.OrderCustomer = null;
        
        return order;
    }

    public TTokenOrder Select(EOrderType _type, List<int> _subIdx, List<int> _value, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        TTokenOrder order = new();
        order.subIdxList = new List<int>(_subIdx);
        order.valueList = new List<int>(_value);
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