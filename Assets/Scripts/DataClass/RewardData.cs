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
    public  List<(int, int)> RewardsList;
    public RewardData(RewardType _rewardType) //어떠한 데이트가 들어오면 거기에 맞게 보상 설정. 
    {
        RewardType = _rewardType;
        RewardsList = new();
        SubType = (int)Capital.Red;
        RewardValue = 125;
        switch (_rewardType)
        {
            case RewardType.CharStat:
                RewardsList.Add(((int)CharStat.Dexility, 10));
                RewardsList.Add(((int)CharStat.Strenth, 10));
                RewardsList.Add(((int)CharStat.Inteligent, 10));
                break;
        }
    }

}
