using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainPolicyEnum
{
    None, NationLevelUP, ExpandLand, ManageLand, TechTree, Support
}

public abstract class NationPolicy
{
    protected Nation m_nation;
    protected MainPolicyEnum m_curMainPolicy = MainPolicyEnum.None; //현재 정책 상황
    protected int m_holdPolicyCount = 0; //현재 정책 유지된 턴
    protected TokenBase m_planToken; //목적지
    protected int m_planIndex = FixedValue.No_INDEX_NUMBER;  //메인 정책당 구체적인 계획의 인덱스
    protected TokenChar m_worker; //노동자
    protected int m_nationNum;
    protected bool m_complete = false;
    protected WorkOrder m_workOrder = null;


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
 
    public void SendNationCallBack(TokenBase _token)
    {
        Debug.Log("공격 받았음을 전달 받음");
    }

    public abstract void MakePlan();

    public void DoWork()
    {
        //작업 진행
        if(m_workOrder == null)
        {
            
            Excute();//작업서가 필요없는 정책이면 바로 집행
            return;
        }
        //아니라면 작업서 작업진행하고
        m_workOrder.DoWork();
        if(m_workOrder.IsDoneWork() == true)
        {
            
            Excute();//작업서가 완료가 되었으면
        }

    }

    public abstract void Excute();
    
    public void Done()
    {
        m_complete = true;
    }

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

    public bool IsDone()
    {
        return m_complete;
    }
    #endregion

    public void Reset()
    {
        if (m_planToken != null)
            m_planToken.ResetPolicy();

        m_planIndex = FixedValue.No_INDEX_NUMBER;
    }

    public void MakeWorkOrder()
    {
        if (m_curMainPolicy != MainPolicyEnum.ManageLand)
            return;
    //    Debug.Log("작업서 만들기");
        m_workOrder = new WorkOrder(m_planIndex);

    }

    public void ShowWorkOrder()
    {
        if (m_workOrder == null)
            return;
    }
}

