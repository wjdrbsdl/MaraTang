using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum RandomTypeEnum
{
   EquiptAllPart, Capital
}

public class DiceRandomItem
{
 

    public TOrderItem GetDiceItem(TOrderItem _item)
    {
        TOrderItem noneItem = new TOrderItem(TokenType.None, 0, 0);
        RandomTypeEnum randomType = (RandomTypeEnum)_item.SubIdx;
        switch (randomType)
        {
            case RandomTypeEnum.EquiptAllPart:
                //��� �����Ʈ�� �ϳ��� ��ȯ
                return GetDiceEquipt(_item.Value);
            case RandomTypeEnum.Capital:
                int itemtier = _item.Value; //
                return GetDiceCapital(itemtier);
        }

        return noneItem;
    }

    private TOrderItem GetDiceEquipt(int _tier)
    {
        TOrderItem noneItem = new TOrderItem(TokenType.None, 0, 0);
        //Debug.Log(_tier + "��� ���� ���");
        //1. Ƽ���� ��������� Ƽ�� ����
        List<int> ableEquipt = new();
        List<int> dropWeight = new(); //��� ����ġ
        List<int> equiptPid = DropItemManager.GetInstance().equiptPidList;
        
        for (int i = 0; i < equiptPid.Count; i++)
        {
            EquiptItemData equiptData = MgMasterData.GetInstance().GetEquiptPoolData(equiptPid[i]);
            if(equiptData.ableDropTir<= _tier)
            {
                ableEquipt.Add(equiptData.m_pid);
                dropWeight.Add(equiptData.dropRatio); //�������ġ�� �߰�
  
               // Debug.Log(equiptData.m_pid + "�� ��� " + _tier + "Ƽ��� ��� ���� ����Ʈ�� �߰�");
            }
        }
        //2. ������ ��� ������ �귿 ���� 
        List<int> selectItem = GameUtil.DiceByWeight(1, dropWeight);
        if (selectItem.Count >= 1)
        {
            int selectEquiptPid = ableEquipt[selectItem[0]];
            int tier = _tier;
            TOrderItem equipt = new TOrderItem(TokenType.Equipt, selectEquiptPid, tier);
            //  Debug.Log(selectPid + "��� " + equiptTier + "Ƽ��� �����ϱ�� �ϰ� �ɼ� ���� ����");
            return GetDiceEquiptOption(equipt); //3. �ش� ��� �ش� Ƽ��� �ɼǻ̱� ���� �� ��ȯ. 
        }
  
        return noneItem;
    }

    private TOrderItem GetDiceCapital(int _tier)
    {
        TOrderItem noneItem = new TOrderItem(TokenType.None, 0, 0);

        List<Capital> randomList = new();
        Array capital = System.Enum.GetValues(typeof(Capital));
        for (int i = 1; i < capital.Length; i++)
        {
            //0 �ε����� None�̹Ƿ� 1���� ���� 
            Capital test = (Capital)capital.GetValue(i);
            CapitalData capitalData = MgMasterData.GetInstance().GetCapitalData(test);
            if (capitalData == null)
            {
                if (FixedValue.CAPITAL_NONE_ARALM == false)
                {
                    Debug.LogError("���� ������ ��ūŸ��" + test);
                    FixedValue.CAPITAL_NONE_ARALM = true;
                }

                continue;
            }

            if (capitalData.Tier == _tier)
            {
                randomList.Add((Capital)capitalData.capitalPid);
            }
        }
        if (randomList.Count == 0)
            return noneItem;

        Capital selectCapital = randomList[GameUtil.GetRandomNum(randomList.Count, 1)[0]];
        CapitalData capitalDataf = MgMasterData.GetInstance().GetCapitalData(selectCapital);
        //Debug.Log((Capital)capitalDataf.capitalPid + "�� �⺻ ä������ " + capitalDataf.baseAmount);
        return new TOrderItem(TokenType.Capital, (int)selectCapital, capitalDataf.baseAmount);

    }

    public TOrderItem GetDiceEquiptOption(TOrderItem _equiptItem)
    {
        TOrderItem equiptItem = _equiptItem;
        int poolPid = _equiptItem.SubIdx;
        EquiptItemData equiptPool = MgMasterData.GetInstance().GetEquiptPoolData(poolPid); //Ǯ ���� ������
        int tier = _equiptItem.Value;
        EquiptItem equipt = equiptPool.GetItem(tier);
        equiptItem.tokenItem = equipt;
        return equiptItem;
    }
}