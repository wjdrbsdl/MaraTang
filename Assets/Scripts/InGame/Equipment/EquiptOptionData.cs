using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class EquiptOptionData
{
    public int Pid;
    public string Name;
    public int SubIdx; //각 토큰타입에 해당하는 SubIdx 보통 CharStat으로 적용될듯
    public List<int> TierStepList; //1티어 시작점 + Gap이 1티어 구간
    public List<int> TierPowerList; //각 티어 폭
    public int PoolDiceValue; //해당 옵션 가중치 - 높을수록 여러 옵션중 잘뜸
    public int StartTier; //해당 옵션이 발생가능한 Tier


    public EquiptOptionData(string[] _parsingData)
    {
        int pidIdx = 0;
        int nameIdx = pidIdx + 1;
        int subvalueIdx = nameIdx + 1;
        int tierStepIdx = subvalueIdx + 1;
        int powerIdx = tierStepIdx + 1;
        int pooldiceIdx = powerIdx + 1;
     
        Pid = int.Parse(_parsingData[pidIdx]);
        Name = _parsingData[nameIdx];
        if (System.Enum.TryParse(typeof(CharStat), _parsingData[subvalueIdx], out object sub))
            SubIdx = (int)sub;
        string[] stepStr = _parsingData[tierStepIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        TierStepList = new();
        string[] powerStr = _parsingData[powerIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        TierPowerList = new();
        for (int i = 0; i < stepStr.Length; i++)
        {
            int step = int.Parse(stepStr[i]);
            int power = int.Parse(powerStr[i]);
            TierStepList.Add(step);
            TierPowerList.Add(power);
        }
        PoolDiceValue = int.Parse(_parsingData[pooldiceIdx]);
        StartTier = 1;
        if (TierStepList.Count >= 1)
            StartTier = TierStepList[0]; //맨 처음단긴애가 제일 최초 시작 티어 

    }

    public TOrderItem GetOptionValue(int _tier)
    {
        int dropTier = _tier;
        int adaptTier = dropTier - StartTier + 1; //해당 옵션이 등장가능한 티어
      //  Debug.Log(Name + "옵션 현재 티어 " + _tier + " 최소 발현가능한 티어 " + StartTier + "보정 티어" + adaptTier);
        if (SubIdx == 0 || adaptTier <= 0)
        {
            return new TOrderItem(TokenType.None, 0, 0);
        }

        //티어 스텝부터 시작해서 현재 티어 사이의 티어를 선출
        //선출된 티어중 하나 선택
        //선택된 티어의 벨류값으로 power 선택
        int ableStepCount = 1; // 뽑기할 스텝의 총수 
        for (int i = 1; i < TierStepList.Count; i++)
        {
            if (TierStepList[i] <= dropTier)
            {
                ableStepCount += 1; //단계 적용 티어 값이 드롭티어보다 낮으면 해당 단계도 가능이므로 가능 단계 +1
            }
            else
            {
                //아니라면 최대 스텝 찾기 종료
                break;
            }
        }

        int stepdice = Random.Range(0, ableStepCount); //가능한 단계수에서 뽑기시작 - 만약 1단계만 가능하면 0만 나와서 0번째 수치가 적용될것
        int selectPower = TierPowerList[stepdice];
        Debug.Log(Name + " 적용 "+ (stepdice + 1) + "단계 파워 적용 인덱스는 " + stepdice + " 효과 수치는 " + selectPower);
        TOrderItem effect = new TOrderItem(TokenType.CharStat, SubIdx, selectPower);
        return effect;
    }
}
