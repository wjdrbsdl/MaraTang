using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgMasterData : Mg<MgMasterData>
{
    private Dictionary<int, TileTypeData> m_tileTypeDataDic;
    private Dictionary<int, TokenChar> m_charDataDic;

    #region 생성자
    public MgMasterData()
    {
        InitiSet();
    }
    #endregion

    public override void InitiSet()
    {
        g_instance = this;
    }

    public override void ReferenceSet()
    {
        Debug.Log("마스터데이터 레퍼런스 시작");
        SetTileTypeData();
        SetCharData();
    }

    public Dictionary<int, TileTypeData> GetTileData()
    {
        return m_tileTypeDataDic;
    }

    #region 마스터 데이터 생성
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

    private void SetCharData()
    {
        m_charDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.CharData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TokenChar masterChar = new(parseData.MatchCode, parseData.DbValueList[i]);
            m_charDataDic.Add(masterChar.GetPid(), masterChar);
        }
        Debug.Log("캐릭터 마스터 데이터 완료");
    }

    #endregion
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

