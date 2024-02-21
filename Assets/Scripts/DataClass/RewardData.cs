using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
  None, Capital, Content, CharStat, TileEvent
}


public class RewardData 
{
    public RewardType RewardType = RewardType.None;
    public int SubType; //자원이면 어떤자원인지, 캐릭스텟이면 무슨 스텟인지 
    public int RewardValue; //보상할 수치

    public RewardData(RewardType _rewardType) //어떠한 데이트가 들어오면 거기에 맞게 보상 설정. 
    {
        RewardType = _rewardType;
        SubType = (int)Capital.Red;
        RewardValue = 125;
    }
}
