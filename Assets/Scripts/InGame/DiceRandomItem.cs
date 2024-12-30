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
        List<int> dropRatio = new();
        int totalRatio = 0;
        List<int> equiptPid = DropItemManager.GetInstance().equiptPidList;
        
        for (int i = 0; i < equiptPid.Count; i++)
        {
            EquiptItemData equiptData = MgMasterData.GetInstance().GetEquiptPoolData(equiptPid[i]);
            if(equiptData.ableDropTir<= _tier)
            {
                ableEquipt.Add(equiptData.m_pid);
                dropRatio.Add(equiptData.dropRatio); //�������ġ�� �߰�
                totalRatio += equiptData.dropRatio; //�� ���ġ �ջ�
               // Debug.Log(equiptData.m_pid + "�� ��� " + _tier + "Ƽ��� ��� ���� ����Ʈ�� �߰�");
            }
        }
        //2. ������ ��� ������ �귿 ���� 
        int diceNum = UnityEngine.Random.Range(1, totalRatio + 1);
        int curRatio = 0;
        
        for (int i = 0; i < dropRatio.Count; i++)
        {
            curRatio += dropRatio[i];
            //����ġ���� �̱��ȣ���� ū���� �ش� ��� ������
            if(diceNum <= curRatio)
            {
                int selectPid = ableEquipt[i]; //���õ� ��� pid
                int equiptTier = _tier; //��� �Ҵ��� Ƽ��
                TOrderItem equipt = new TOrderItem(TokenType.Equipt, selectPid, equiptTier);
              //  Debug.Log(selectPid + "��� " + equiptTier + "Ƽ��� �����ϱ�� �ϰ� �ɼ� ���� ����");
                return GetDiceEquiptOption(equipt); //3. �ش� ��� �ش� Ƽ��� �ɼǻ̱� ���� �� ��ȯ. 
            }
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