using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NationStatPart
{
    private Nation m_nation;
    private int[] m_nationStatValues;

    public NationStatPart(Nation _nation)
    {
        m_nation = _nation;
        m_nationStatValues = _nation.GetStatValues();
    }

    public void RelateStat()
    {
     
    }

  
}
