using System;
using System.Collections.Generic;


public class CapitalData
{
    public int capital = (int)Capital.None;
    public int Tier = 0;
    public int baseRatio = 0; //기본 채집확률


    public CapitalData(List<int[]> matchCode, string[] _parsingData)
    {
        int capitalPidIndex = 0;
        if (System.Enum.TryParse(typeof(Capital), _parsingData[capitalPidIndex], out object parseTileType))
            capital = (int)parseTileType;

        Tier = int.Parse(_parsingData[2]);
        baseRatio = int.Parse(_parsingData[3]);
        
    }
}

