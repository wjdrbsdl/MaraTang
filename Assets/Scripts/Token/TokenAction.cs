using UnityEngine;
using System.Collections.Generic;

public enum ActionType
{
    Move, Attack
}

public enum ActionStat
{
    Range, MaxCountInTurn, RemainCountInTurn, Power, NeedActionEnergy, NeedActionCount, Value2
}

[System.Serializable]
public class TokenAction : TokenBase
{
    [SerializeField]
    private ActionType actionType = ActionType.Move;
    private TokenType actionTarget = TokenType.Tile;
    private int[] m_targetPos; //작용할 위치 
    private TokenBase m_caster; //사용자\

    #region 액션 토큰 : 생성부분 추후 테이블 파싱 값으로 생성하기
    public TokenAction()
    {

    }

    public TokenAction(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]);
        m_itemName = valueCode[1];
        m_tokenIValues = new int[System.Enum.GetValues(typeof(ActionStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)ActionStat.RemainCountInTurn] = m_tokenIValues[(int)ActionStat.MaxCountInTurn];
    }

    public TokenAction MakeTestAction(ActionType _type)
    {
        TokenAction actionToken = new TokenAction();
        actionToken.m_tokenIValues = new int[System.Enum.GetValues(typeof(ActionStat)).Length];
        actionToken.m_targetPos = null;
        actionToken.m_tokenType = TokenType.Action;
        actionToken.actionType = _type;
        if (_type.Equals(ActionType.Move))
        {
            actionToken.actionTarget = TokenType.Tile;
            actionToken.SetStatValue(ActionStat.Range, 3);
            actionToken.SetStatValue(ActionStat.RemainCountInTurn, 2);
            actionToken.SetStatValue(ActionStat.MaxCountInTurn, 2);
        }
        else if (_type.Equals(ActionType.Attack))
        {
            actionToken.actionTarget = TokenType.Char;
            actionToken.SetStatValue(ActionStat.Range, 1);
            actionToken.SetStatValue(ActionStat.RemainCountInTurn, 1);
            actionToken.SetStatValue(ActionStat.MaxCountInTurn, 1);
        }
        
        return actionToken;
    }

    #endregion

    #region 타겟 수정 관련
    public ActionType GetActionType()
    {
        return actionType;
    }

    public int[] GetTargetPos()
    {
        return m_targetPos;
    }

    public TokenType GetTargetType()
    {
        return actionTarget;
    }

    public void SetTargetCoordi(int[] _mapIdx)
    {
        m_targetPos = _mapIdx;
    }

    public void ClearTarget()
    {
        m_targetPos =null;
    }
    #endregion

    public void RcoverRemainCountInTurn()
    {
        m_tokenIValues[(int)ActionStat.RemainCountInTurn] = m_tokenIValues[(int)ActionStat.MaxCountInTurn];
    }

    public bool AbleUse()
    {
        //1. 남은 횟수가 1 이상이면
        if (0 == m_tokenIValues[(int)ActionStat.RemainCountInTurn])
        {
            return false;
        }
        return true;
    }
}
