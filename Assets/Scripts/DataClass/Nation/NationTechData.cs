using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TechTreeStat
{
    Class, NeedTurn
}

public class NationTechData
{

    //국가 기술 하나 정보 
    private int m_techPid;
    private string m_techName;
    private int m_techClass;

    public TItemListData ResearchCostData;
    public TItemListData TechEffectData;

    public NationTechData(List<int[]> matchCode, string[] _parsingData)
    {
        m_techPid = int.Parse(_parsingData[0]);
        m_techName = _parsingData[1];
        m_techClass = (int.Parse(_parsingData[2]));
        //학습 비용 적어놓은 칸이 있으면
        int costIndex = 4; //구글 sheet상 열 인덱스
        ResearchCostData = GameUtil.ParseCostDataArray(_parsingData, costIndex);
        int effectIndex = 5;
        TechEffectData = GameUtil.ParseCostDataArray(_parsingData, effectIndex);
    }

    public int GetPid()
    {
        return m_techPid;
    }

    public string GetTechName()
    {
        return m_techName;
    }

    public int GetTechClass()
    {
        return m_techClass;
    }
}
