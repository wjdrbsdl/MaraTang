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
    public  List<(int, int)> RewardsList;
    public RewardData(RewardType _rewardType) //��� ����Ʈ�� ������ �ű⿡ �°� ���� ����. 
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
