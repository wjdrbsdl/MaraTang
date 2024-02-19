using UnityEngine;
using System.Collections.Generic;

public enum ActionType
{
    Move, Attack
}

public enum CharActionStat
{
   MinRange, Range, MinRatio, MaxCountInTurn, RemainCountInTurn, Power, NeedActionEnergy, NeedActionCount, Value2
}

public enum TileActionStat
{
    NeedActionEnergy, NeedActionCount, TileActionType, SubValue
}

public enum TileActionType
{
    Harvest = 1, Build, CapitalChef
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
        m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)CharActionStat.RemainCountInTurn] = m_tokenIValues[(int)CharActionStat.MaxCountInTurn];
    }

    public TokenAction MakeTestAction(ActionType _type)
    {
        TokenAction actionToken = new TokenAction();
        actionToken.m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        actionToken.m_targetPos = null;
        actionToken.m_tokenType = TokenType.Action;
        actionToken.actionType = _type;
        actionToken.SetStatValue(CharActionStat.MinRange, 1);
        if (_type.Equals(ActionType.Move))
        {
            actionToken.actionTarget = TokenType.Tile;
            actionToken.SetStatValue(CharActionStat.Range, 3);
            actionToken.SetStatValue(CharActionStat.MinRatio, 2);
            actionToken.SetStatValue(CharActionStat.RemainCountInTurn, 2);
            actionToken.SetStatValue(CharActionStat.MaxCountInTurn, 2);
            actionToken.SetStatValue(CharActionStat.NeedActionCount, 1);
        }
        else if (_type.Equals(ActionType.Attack))
        {
            actionToken.actionTarget = TokenType.Char;
            actionToken.SetStatValue(CharActionStat.Range, 1);
            actionToken.SetStatValue(CharActionStat.MinRatio, 1);
            actionToken.SetStatValue(CharActionStat.RemainCountInTurn, 1);
            actionToken.SetStatValue(CharActionStat.MaxCountInTurn, 1);
            actionToken.SetStatValue(CharActionStat.NeedActionCount, 2);
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
        m_tokenIValues[(int)CharActionStat.RemainCountInTurn] = m_tokenIValues[(int)CharActionStat.MaxCountInTurn];
    }

    public bool AbleUse()
    {
        //1. 남은 횟수가 1 이상이면
        if (0 == m_tokenIValues[(int)CharActionStat.RemainCountInTurn])
        {
            return false;
        }
        return true;
    }

    public int GetFinalRange(TokenChar _char)
    {
        int finalRange = GetStat(CharActionStat.Range);
        int dex = _char.GetStat(CharStat.Dexility);
        int ratio = GetStat(CharActionStat.MinRatio);
        float ratioValue = (finalRange * dex * ratio * 0.01f);
        finalRange =(int)( finalRange + ratioValue);
        return finalRange;
    }
}
