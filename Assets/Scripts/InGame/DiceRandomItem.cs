using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public enum RandomTypeEnum
{
    Capital1, EquiptAllPart, Capital
}

public class DiceRandomItem
{
 

    public TOrderItem GetDiceItem(TOrderItem _item)
    {
        TOrderItem item = new TOrderItem(TokenType.None, 0, 0);
        RandomTypeEnum randomType = (RandomTypeEnum)_item.SubIdx;
        switch (randomType)
        {
            case RandomTypeEnum.EquiptAllPart:
                //장비 모든파트중 하나를 반환
                return item;
            case RandomTypeEnum.Capital1:
                int tier = 1;
                int amount = _item.Value;
                return GetDiceCapital(tier, amount);
            case RandomTypeEnum.Capital:
                int itemtier = _item.Value; //
                return GetDiceCapital(itemtier);
        }

        return item;
    }

    private TOrderItem GetDiceCapital(int _tier, int _amount)
    {
        TOrderItem noneItem = new TOrderItem(TokenType.None, 0, 0);
        int amount = _amount; //레벨에 맞는 자원들을 훑어와야함. 
        List<Capital> randomList = new();
        Array capital = System.Enum.GetValues(typeof(Capital));
        for (int i = 1; i < capital.Length; i++)
        {
            //0 인덱스는 None이므로 1부터 진행 
            Capital test = (Capital)capital.GetValue(i);
            CapitalData capitalData = MgMasterData.GetInstance().GetCapitalData(test);
            if (capitalData == null)
            {
                if (FixedValue.CAPITAL_NONE_ARALM == false)
                {
                    Debug.LogError("없는 데이터 토큰타입" + test);
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
        return new TOrderItem(TokenType.Capital, (int)selectCapital, amount);
        
    }

    private TOrderItem GetDiceCapital(int _tier)
    {
        TOrderItem noneItem = new TOrderItem(TokenType.None, 0, 0);

        List<Capital> randomList = new();
        Array capital = System.Enum.GetValues(typeof(Capital));
        for (int i = 1; i < capital.Length; i++)
        {
            //0 인덱스는 None이므로 1부터 진행 
            Capital test = (Capital)capital.GetValue(i);
            CapitalData capitalData = MgMasterData.GetInstance().GetCapitalData(test);
            if (capitalData == null)
            {
                if (FixedValue.CAPITAL_NONE_ARALM == false)
                {
                    Debug.LogError("없는 데이터 토큰타입" + test);
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
        Debug.Log((Capital)capitalDataf.capitalPid + "의 기본 채집수량 " + capitalDataf.baseAmount);
        return new TOrderItem(TokenType.Capital, (int)selectCapital, capitalDataf.baseAmount);

    }

    public TOrderItem GetDiceEquiptOption(TOrderItem _equiptItem)
    {
        TOrderItem equiptItem = _equiptItem;
        int poolPid = _equiptItem.SubIdx;
        EquiptItemPool equiptPool = MgMasterData.GetInstance().GetEquiptPoolData(poolPid); //풀 정보 가져옴
        int tier = _equiptItem.Value;
        EquiptItem equipt = equiptPool.GetItem(tier);
        equiptItem.tokenItem = equipt;
        return equiptItem;
    }
}