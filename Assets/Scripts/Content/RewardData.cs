using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RewardType
{
  None, Capital, Content, CharStat
}


public class RewardData 
{
    public RewardType RewardType = RewardType.None;
    public int SubType; //�ڿ��̸� ��ڿ�����, ĳ�������̸� ���� �������� 
    public int RewardValue; //������ ��ġ

    public RewardData() //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
    {
        RewardType = RewardType.Capital;
        SubType = (int)Capital.Red;
        RewardValue = 125;
    }
}
