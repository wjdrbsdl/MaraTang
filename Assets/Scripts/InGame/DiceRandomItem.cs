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
                //장비 모든파트중 하나를 반환
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
        //Debug.Log(_tier + "장비 랜덤 드랍");
        //1. 티어대로 드랍가능한 티어 선출
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
                dropRatio.Add(equiptData.dropRatio); //드랍가중치도 추가
                totalRatio += equiptData.dropRatio; //총 드랍치 합산
               // Debug.Log(equiptData.m_pid + "번 장비 " + _tier + "티어로 드랍 가능 리스트에 추가");
            }
        }
        //2. 정해진 장비를 가지고 룰렛 진행 
        int diceNum = UnityEngine.Random.Range(1, totalRatio + 1);
        int curRatio = 0;
        
        for (int i = 0; i < dropRatio.Count; i++)
        {
            curRatio += dropRatio[i];
            //가중치합이 뽑기번호보다 큰순간 해당 장비가 뽑힌거
            if(diceNum <= curRatio)
            {
                int selectPid = ableEquipt[i]; //선택된 장비 pid
                int equiptTier = _tier; //장비에 할당할 티어
                TOrderItem equipt = new TOrderItem(TokenType.Equipt, selectPid, equiptTier);
              //  Debug.Log(selectPid + "장비를 " + equiptTier + "티어로 생성하기로 하고 옵션 선택 진행");
                return GetDiceEquiptOption(equipt); //3. 해당 장비 해당 티어에서 옵션뽑기 진행 후 반환. 
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
        //Debug.Log((Capital)capitalDataf.capitalPid + "의 기본 채집수량 " + capitalDataf.baseAmount);
        return new TOrderItem(TokenType.Capital, (int)selectCapital, capitalDataf.baseAmount);

    }

    public TOrderItem GetDiceEquiptOption(TOrderItem _equiptItem)
    {
        TOrderItem equiptItem = _equiptItem;
        int poolPid = _equiptItem.SubIdx;
        EquiptItemData equiptPool = MgMasterData.GetInstance().GetEquiptPoolData(poolPid); //풀 정보 가져옴
        int tier = _equiptItem.Value;
        EquiptItem equipt = equiptPool.GetItem(tier);
        equiptItem.tokenItem = equipt;
        return equiptItem;
    }
}