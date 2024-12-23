using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NationTerritoryPart
{
    private Nation m_nation;
   
    public NationTerritoryPart(Nation _nation)
    {
        m_nation = _nation;
    }

    public int GetMaxTerritoryCount()
    {
        //최대 영지 = 레벨 * 레벨당 토지 + 추가 토지
        int maxCount;

        int level = m_nation.GetNationLevel();
        int countByLevel = m_nation.GetStat(NationStatEnum.TerritoryByLevel);
        int addCount = m_nation.GetStat(NationStatEnum.AddTerritory);

        maxCount = (level * countByLevel) + addCount;

        return maxCount;
    }

  
}
