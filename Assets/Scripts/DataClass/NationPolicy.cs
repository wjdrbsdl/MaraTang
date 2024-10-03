using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainPolicyEnum
{
    None, NationLevelUP, ExpandLand, ManageLand, TechTree, Support
}

public class NationPolicy
{
    private MainPolicyEnum m_curMainPolicy = MainPolicyEnum.None; //현재 정책 상황
    private int m_holdPolicyCount = 0; //현재 정책 유지된 턴
    private TokenBase m_planToken; //목적지
    private int m_planIndex = FixedValue.No_INDEX_NUMBER;  //메인 정책당 구체적인 계획의 인덱스
    private TokenChar m_worker; //노동자
    private int m_nationNum;
    private bool m_complete = false;

    //안건 정보를 담아 생성
    public NationPolicy(MainPolicyEnum _mainPolicy, int _nationNum)
    {
        m_curMainPolicy = _mainPolicy;
        m_nationNum = _nationNum;
        m_holdPolicyCount = 0;
        m_planToken = null;
        m_planIndex = FixedValue.No_INDEX_NUMBER;
    }
 
    public void SendNationCallBack(TokenBase _token)
    {
        Debug.Log("공격 받았음을 전달 받음");
    }

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
}

