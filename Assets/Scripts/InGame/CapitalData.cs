using System;
using System.Collections.Generic;
using UnityEngine;

public class CapitalData
{
    public int capitalPid = (int)Capital.None;
    public int Tier = 0;
    public int baseRatio = 0; //기본 채집확률
    public int baseAmount = 0; //기본 채집수량
    public TileType ablePlace = TileType.None; //발견 가능장소
    public CapitalData(List<int[]> matchCode, string[] _parsingData)
    {
        int capitalPidIndex = 0;
        if (System.Enum.TryParse(typeof(Capital), _parsingData[capitalPidIndex], out object parseTileType))
            capitalPid = (int)parseTileType;

        Tier = int.Parse(_parsingData[2]);
        baseRatio = int.Parse(_parsingData[3]);

        int ableTileIndex = 4;
        if (System.Enum.TryParse(typeof(TileType), _parsingData[ableTileIndex], out object ableTileType))
            ablePlace = (TileType)ableTileType;

      //  Debug.Log((Capital)capitalPid + "자원 발견 가능장소 " + ablePlace);
        baseAmount = int.Parse(_parsingData[5]);


    }
}

