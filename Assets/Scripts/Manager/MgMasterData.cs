using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgMasterData : Mg<MgMasterData>
{
    private Dictionary<int, TileTypeData> m_tileTypeDataDic;
    private Dictionary<int, TokenChar> m_charDataDic;
    private Dictionary<int, TokenAction> m_tileActionDataDic;

    #region 생성자
    public MgMasterData()
    {
        InitiSet();
    }
    #endregion

    public override void InitiSet()
    {
        g_instance = this;
        SetTileTypeData();
        SetCharData();
        SetTileActionData();
    }

    public override void ReferenceSet()
    {
       // Debug.Log("마스터데이터 레퍼런스 시작");
   
    }

    public TileTypeData GetTileData(int _tileTypeID)
    {
        return m_tileTypeDataDic[_tileTypeID];
    }

    public TokenChar GetCharData(int _charPID)
    {
        return m_charDataDic[_charPID];
    }

    public TokenAction GetTileActions(int _actionPID)
    {
        return m_tileActionDataDic[_actionPID];
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
    //    Debug.Log("완료");
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
     //   Debug.Log("캐릭터 마스터 데이터 완료");
    }

    private void SetTileActionData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.TileActionData);
        m_tileActionDataDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            TokenAction tileAction = new TokenAction(parseContainer.MatchCode, parseContainer.DbValueList[i]);
            m_tileActionDataDic.Add(tileAction.GetPid(), tileAction);
        }
    }
    #endregion
}

public class TileTypeData{
    public int TypePID;
    public int[] AbleTileActionPID;

    public TileTypeData(string[] _parsingData)
    {
        TypePID = int.Parse(_parsingData[0]);
        string ables = _parsingData[1]; //가능한 작업이 나열되어있음
        string[] divideAble = ables.Trim().Split(" ");
        AbleTileActionPID = new int[divideAble.Length];
        for (int i = 0; i < divideAble.Length; i++)
        {
            AbleTileActionPID[i] = int.Parse(divideAble[i]);
        }
    }
}

