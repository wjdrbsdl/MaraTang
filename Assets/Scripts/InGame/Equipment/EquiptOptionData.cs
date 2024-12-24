using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class EquiptOptionData
{
    public int Pid;
    public string Name;
    public int SubIdx; //각 토큰타입에 해당하는 SubIdx 보통 CharStat으로 적용될듯
    public int Tier1Value; //1티어 시작점 + Gap이 1티어 구간
    public int TierGap; //각 티어 폭
    public int PoolDiceValue; //해당 옵션 가중치 - 높을수록 여러 옵션중 잘뜸

    public EquiptOptionData(string[] _parsingData)
    {
        int pidIdx = 0;
        int nameIdx = pidIdx + 1;
        int subvalueIdx = nameIdx + 1;
        int tier1valueIdx = subvalueIdx + 1;
        int tierGapIdx = tier1valueIdx + 1;
        int pooldiceIdx = tierGapIdx + 1;

        Pid = int.Parse(_parsingData[pidIdx]);
        Name = _parsingData[nameIdx];
        if (System.Enum.TryParse(typeof(CharStat), _parsingData[subvalueIdx], out object sub))
            SubIdx = (int)sub;
        Tier1Value = int.Parse(_parsingData[tier1valueIdx]);
        TierGap = int.Parse(_parsingData[tierGapIdx]);
        PoolDiceValue = int.Parse(_parsingData[pooldiceIdx]);
        
    }


}
