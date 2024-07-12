using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NationPolicy
{
    private MainPolicy m_curMainPolicy = MainPolicy.None; //현재 정책 상황
    private int m_holdPolicyCount = 0; //현재 정책 유지된 턴
    private TokenBase m_planToken; //목적지
    private int m_planIndex = FixedValue.No_INDEX_NUMBER;  //메인 정책당 구체적인 계획의 인덱스
    private TokenChar m_worker; //노동자
    private int m_nationNum;
    private bool m_complete = false;

    //안건 정보를 담아 생성
    public NationPolicy(MainPolicy _mainPolicy, int _nationNum)
    {
        m_curMainPolicy = _mainPolicy;
        m_nationNum = _nationNum;
        m_holdPolicyCount = 0;
        m_planToken = null;
        m_planIndex = FixedValue.No_INDEX_NUMBER;
    }

    public NationPolicy(MainPolicy _mainPolicy, TokenBase _planToken, int _planIndex, int _nationNum)
    {
        m_curMainPolicy = _mainPolicy;
        m_planToken = _planToken;
        m_planIndex = _planIndex;
        m_holdPolicyCount = 0;
        m_nationNum = _nationNum;
    }

    public void Excute()
    {
        switch (m_curMainPolicy)
        {
            case MainPolicy.ExpandLand:
            case MainPolicy.ManageLand:
                //땅 경작류 작업은 노동자를 뽑아서 해당 위치로 날려야함. 
                Nation nation = MgNation.GetInstance().GetNation(m_nationNum);
                //그 나라의 수도 
                TokenTile nationCapitalTile = nation.GetCapital();
                m_worker = MgToken.GetInstance().SpawnCharactor(nationCapitalTile.GetMapIndex(), 5); //
                m_worker.NationPolicy = this;
                Debug.Log("일꾼 생성");
                break;
        }
    }

    public void SendNationCallBack(TokenBase _token)
    {
        Debug.Log("공격 받았음을 전달 받음");
    }

    public void Remind()
    {
        //국가 턴이 종료 될 때 해당 안건을 되돌아보며 턴 진행, 필요 턴 진행했을때 조건 고려, 취소, 변경등을 고려. 
        if (m_holdPolicyCount >= 3)
        {
            //해당 정책 포기해볼까. 
        }
        m_holdPolicyCount += 1;
        TokenTile tile = (TokenTile)m_planToken;
        switch (m_curMainPolicy)
        {
            case MainPolicy.ManageLand:
                Debug.LogFormat("{0},{1}번 땅을 {2} 번 유형으로 {3} 턴 째 개척 진행중", tile.GetXIndex(), tile.GetYIndex(), m_planIndex, m_holdPolicyCount);
                break;
            default:
                Debug.LogFormat("{0}번 타입 진행중 " + m_curMainPolicy.ToString());
                break;
        }
    }

    public void Done()
    {
        m_complete = true;
    }

    #region GetSet
    public MainPolicy GetMainPolicy()
    {
        return m_curMainPolicy;
    }

    public TokenBase GetPlanToken()
    {
        return m_planToken;
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
}

