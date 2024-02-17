using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgMasterData : Mg<MgMasterData>
{
    private Dictionary<int, TileTypeData> m_tileTypeDataDic;

    public MgMasterData()
    {
        InitiSet();
    }

    public override void InitiSet()
    {
        g_instance = this;
    }

    public override void ReferenceSet()
    {
        Debug.Log("마스터데이터 레퍼런스 시작");
        SetTileTypeData();
    }
    private void SetTileTypeData()
    {
        m_tileTypeDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.TileType);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TileTypeData newTileData = new(parseData.DbValueList[i]);
            m_tileTypeDataDic.Add(newTileData.TypePID, newTileData);
        }
        Debug.Log("완료");
    }

    public Dictionary<int, TileTypeData> GetTileData()
    {
        return m_tileTypeDataDic;
    }
}

public class TileTypeData{
    public int TypePID;
    public int[] AbleTileWork;

    public TileTypeData(string[] _parsingData)
    {
        TypePID = int.Parse(_parsingData[0]);
        string ables = _parsingData[1]; //가능한 작업이 나열되어있음
        string[] divideAble = ables.Trim().Split(" ");
        AbleTileWork = new int[divideAble.Length];
        for (int i = 0; i < divideAble.Length; i++)
        {
            AbleTileWork[i] = int.Parse(divideAble[i]);
        }
    }
}