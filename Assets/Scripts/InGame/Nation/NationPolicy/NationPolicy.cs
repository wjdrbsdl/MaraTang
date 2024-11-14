using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainPolicyEnum
{
    None, NationLevelUP, ExpandLand, ManageLand, TechTree, Support
}

public abstract class NationPolicy
{
    //적절한 작업서를 만들기위해 정책클래스가 필요하다.

    protected Nation m_nation;
    protected MainPolicyEnum m_curMainPolicy = MainPolicyEnum.None; //현재 정책 상황
    protected int m_holdPolicyCount = 0; //현재 정책 유지된 턴
    protected TokenBase m_planToken; //목적지
    protected int m_planIndex = FixedValue.No_INDEX_NUMBER;  //메인 정책당 구체적인 계획의 인덱스
    protected int m_nationNum;
    protected bool m_complete = false;
    public WorkOrder m_workOrder = null;
    public bool m_donePlan = false; //계획이 세워졌는지


    //안건 정보를 담아 생성
    public void SetInfo(MainPolicyEnum _mainPolicy, Nation _nation)
    {
        m_curMainPolicy = _mainPolicy;
        m_nation = _nation;
        m_nationNum = 1;
        m_holdPolicyCount = 0;
        m_planToken = null;
        m_planIndex = FixedValue.No_INDEX_NUMBER;
    }
 
    public abstract void MakePlan();

    public void SetDonePlan(bool _isDone)
    {
        m_donePlan = _isDone;
    }

    public abstract WorkOrder WriteWorkOrder();

    #region GetSet
    public MainPolicyEnum GetMainPolicy()
    {
        return m_curMainPolicy;
    }

    public TokenBase GetPlanToken()
    {
        return m_planToken;
    }

    public int GetHoldCount()
    {
        return m_holdPolicyCount;
    }

    public void SetPlanToken(TokenBase _planToken)
    {
        m_planToken = _planToken;
    }

    public int GetPlanIndex()
    {
        return m_planIndex;
    }

    public void SetPlanIndex(int _planIndex)
    {
        m_planIndex = _planIndex;
    }

    public int GetNaionNum()
    {
        return m_nationNum;
    }

    #endregion


}

