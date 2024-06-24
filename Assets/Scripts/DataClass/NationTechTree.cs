using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TechTreeStat
{
    Class, NeedTurn, NeedWood, NeedMineral
}

public class NationTechTree
{

    //국가 기술 하나 정보 
    private int m_techPid;
    private string m_techName;
    private int m_techClass;
    private int[] m_techValues;
    public OrderCostData ResearchCostData;

    public NationTechTree(List<int[]> matchCode, string[] _parsingData)
    {
        m_techPid = int.Parse(_parsingData[0]);
        m_techName = _parsingData[1];
        m_techClass = (int.Parse(_parsingData[2]));
        m_techValues = new int[GameUtil.EnumLength(TechTreeStat.Class)];
        GameUtil.InputMatchValue(ref m_techValues, matchCode, _parsingData);
        //학습 비용 적어놓은 칸이 있으면
        int costIndex = 4; //구글 sheet상 열 인덱스
        ResearchCostData = GameUtil.ParseCostDataArray(_parsingData, costIndex);

    }

    public int GetPid()
    {
        return m_techPid;
    }

    public string GetTechName()
    {
        return m_techName;
    }

    public int GetTechValue(TechTreeStat _stat)
    {
        return m_techValues[(int)_stat];
    }
}
