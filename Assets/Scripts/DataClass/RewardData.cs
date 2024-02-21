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

    public RewardData(RewardType _rewardType) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = _rewardType;
        SubType = (int)Capital.Red;
        RewardValue = 125;
    }
}
