using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TechTreeStat
{
    Class, NeedTurn, NeedWood, NeedMineral
}

public class NationTechTree
{
    private int m_techPid;
    private string m_techName;
    private int m_techClass;
    private int[] m_techValues;

    public NationTechTree(List<int[]> matchCode, string[] valueCode)
    {
        m_techPid = int.Parse(valueCode[0]);
        m_techName = valueCode[1];
        m_techClass = (int.Parse(valueCode[2]));
        m_techValues = new int[GameUtil.EnumLength(TechTreeStat.Class)];
        GameUtil.InputMatchValue(ref m_techValues, matchCode, valueCode);
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
