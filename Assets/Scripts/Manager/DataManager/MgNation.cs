using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgNation : Mg<MgNation>
{
    private List<Nation> m_nationList;
    private int m_turnNationNumber = 0; //현재 차례 국가 넘버 

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

    //턴 시작시 국가들 행동 할것
    /*
     * 1. 점유한 토지에 따른 생산 진행
     * 1-2. 초과분에 대한 정산
     * 2. 소비할것 소비 
     * 2-2. 부족분에 대한 정산
     * 3. 각 국가마다 정책턴이면 정책 수립
     * 4. 토지 확장 고려 
     */

    public void ManageNationTurn()
    {
        m_nationList[m_turnNationNumber].DoJob(NationManageStepEnum.IncomeCapital);
    }

    public void EndNationTurn()
    {
        //턴 진행중이던 국가로부터 자신의 턴이 끝났음을 전달 받으면
        int nationCount = m_nationList.Count;
        m_turnNationNumber += 1; //다음 진행 국가를 뽑고 할 놈이 남았는지 확인
        if (nationCount <= m_turnNationNumber)
        {
            //국가턴이 종료된 상황
            GamePlayMaster.GetInstance().DoneStep(GamePlayStep.NationTurn); //EndPlayTurn에서 모든 캐릭 턴 끝나면 호출
            return;
        }

        //그게 아니라면
        ManageNationTurn(); //다시 매니저 진행
    }

    public void AddTerritoryToNation(int nationNum, TokenTile _tile)
    {
        Nation nation = m_nationList[nationNum];
        nation.AddTerritory(_tile);
        nation.ShowTerritory();
    }

    public void ResetNationNum()
    {
        m_turnNationNumber = 0;
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
