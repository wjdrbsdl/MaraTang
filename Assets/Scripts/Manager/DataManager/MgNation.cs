using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNation : Mg<MgNation>
{
    private List<Nation> m_nationList;

    #region 국가 생성 파괴
    public MgNation()
    {
        g_instance = this;
        m_nationList = new List<Nation>();
    }

    public void MakeNation(TokenTile _capital, int _nationNumber)
    {
        Nation newNation = new Nation().MakeNewNation(_capital, _nationNumber);
        m_nationList.Add(newNation);
    }

    public void DestroyNation(Nation _nation)
    {
        _nation.Destroy();
        m_nationList.Remove(_nation);
    }
    #endregion

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

    public void AddTerritoryToNation(int nationNum, TokenTile _tile)
    {
        Nation nation = m_nationList[nationNum];
        nation.AddTerritory(_tile);
        nation.ShowTerritory();
    }

    public Nation GetNation(int _nationNum)
    {
        //국가 넘버가 0보다 작거나, 국가 수보다 크거나 같으면 없는 국가 - 국가 생성 순서로 국가 번호를 넣기 때문에.
        if (_nationNum < 0)
            return null;
        if (_nationNum >= m_nationList.Count)
            return null;

        return m_nationList[_nationNum];
    }

    public List<Nation> GetNationList()
    {
        return m_nationList;
    }
}
