using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNation : Mg<MgNation>
{
    private List<Nation> m_nationList;

    public MgNation()
    {
        g_instance = this;
        m_nationList = new List<Nation>();
    }

    public void MakeNation(TokenTile _capital)
    {
        Nation newNation = new Nation().MakeNewNation(_capital);
        m_nationList.Add(newNation);
    }

    public void AlarmNations()
    {
        for (int i = 0; i < m_nationList.Count; i++)
        {
            Nation nation = m_nationList[i];
            int[] pos = nation.GetCapital().GetMapIndex();
            Debug.Log("해당 국가의 좌표는 " + pos[0] + "_" + pos[1]);
            nation.ShowTerritory();
        }
    }

    public void DestroyNation(Nation _nation)
    {
        _nation.Destroy();
        m_nationList.Remove(_nation);
    }
}
