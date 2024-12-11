using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TechEnum
{
   None, Build2, Serach, Harvest, SpecialBuild,
    WorkEffeciency, GoodTalk, SsabaSsaba, GoodPrepare, GodTalking, GreedBless }

public class NationTechData
{
    //국가 기술 하나 정보 
    private int m_techPid;
    private string m_techName;
    private int m_techClass;

    public int NeedTurn; //연구에 필요한 턴
    public int NeedLabor; //연구에 필요한 노동

    public List<int> NeedPreTech; //연구에 필요한 선행 연구들

    public TItemListData ResearchCostData;
    public TItemListData TechEffectData;

    public NationTechData(List<int[]> matchCode, string[] _parsingData)
    {
        if (System.Enum.TryParse(typeof(TechEnum), _parsingData[0], out object parseTechPid))
            m_techPid = (int)parseTechPid;
        m_techName = _parsingData[1];
        m_techClass = (int.Parse(_parsingData[2]));

        int needTurnIndex = 3;
        int needlaborIndex = needTurnIndex + 1;
        int costIndex = needTurnIndex+1;
        int effectIndex = costIndex + 1;

        NeedTurn = (int.Parse(_parsingData[needTurnIndex]));
        NeedLabor = (int.Parse(_parsingData[needlaborIndex]));
        ResearchCostData = GameUtil.ParseCostDataArray(_parsingData, costIndex);
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
