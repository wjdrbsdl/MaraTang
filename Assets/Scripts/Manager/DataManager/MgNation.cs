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

    public void ManageNationTurn()
    {
        //턴 시작시 국가들 행동 할것
        /*
         * 1. 점유한 토지에 따른 생산 진행
         * 1-2. 초과분에 대한 정산
         * 2. 소비할것 소비 
         * 2-2. 부족분에 대한 정산
         * 3. 각 국가마다 정책턴이면 정책 수립
         * 4. 토지 확장 고려 
         */
      //  Debug.Log("국가들 턴 시작" + m_nationList.Count);
        foreach(Nation nation in m_nationList)
        {
            nation.ManageNation();
        }
    }
}
