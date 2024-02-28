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
    public int SubType; //�ڿ��̸� ��ڿ�����, ĳ�������̸� ���� �������� 
    public int RewardValue; //������ ��ġ
    public  List<RewardInfo> RewardsList;
    public RewardData(RewardType _rewardType) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
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