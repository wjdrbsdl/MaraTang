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
    public  List<RewardInfo> RewardsList;
    public RewardData(RewardType _rewardType) //어떠한 데이트가 들어오면 거기에 맞게 보상 설정. 
    {
        RewardType = _rewardType;
        RewardsList = new();
        SubType = (int)Capital.Red;
        RewardValue = 125;
        switch (_rewardType)
        {
            case RewardType.CharStat:
                RewardsList.Add(new RewardInfo(RewardType.CharStat, (int)CharStat.Dexility, 10));
                RewardsList.Add(new RewardInfo(RewardType.CharStat, (int)CharStat.Strenth, 10));
                RewardsList.Add(new RewardInfo(RewardType.CharStat, (int)CharStat.Inteligent, 10));
                break;
        }
    }

}
public struct RewardInfo
{
    public RewardType RewardType;
    public int SubIdx;
    public int Value;

    public RewardInfo(RewardType _type, int _subIdx, int _value)
    {
        RewardType = _type;
        SubIdx = _subIdx;
        Value = _value;
    }
}