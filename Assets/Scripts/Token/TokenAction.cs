using UnityEngine;
using System.Collections.Generic;

public enum ActionType
{
    Move, Attack
}

public enum ActionStat
{
    Range, MaxTargetCount, Power, NeedActionEnergy
}

[System.Serializable]
public class TokenAction : TokenBase
{
    [SerializeField]
    private ActionType actionType = ActionType.Move;
    private TokenType actionTarget = TokenType.Tile;
    private List<TokenBase> m_targetList = new List<TokenBase>(); //대상
    private TokenBase m_caster; //사용자\

    #region 액션 토큰 : 생성부분 추후 테이블 파싱 값으로 생성하기
    public TokenAction()
    {

    }

    public TokenAction MakeTestAction(ActionType _type)
    {
        TokenAction actionToken = new TokenAction();
        actionToken.m_tokenIValues = new int[System.Enum.GetValues(typeof(ActionStat)).Length];
        actionToken.m_targetList = new();
        actionToken.m_tokenType = TokenType.Action;
        actionToken.actionType = _type;
        if (_type.Equals(ActionType.Move))
        {
            actionToken.actionTarget = TokenType.Tile;
            actionToken.SetStat(ActionStat.Range, 3);
            actionToken.SetStat(ActionStat.MaxTargetCount, 2);
        }
        else if (_type.Equals(ActionType.Attack))
        {
            actionToken.actionTarget = TokenType.Char;
            actionToken.SetStat(ActionStat.Range, 1);
            actionToken.SetStat(ActionStat.MaxTargetCount, 1);
        }
        
        
        return actionToken;
    }
    #endregion

    #region 타겟 수정 관련
    public ActionType GetActionType()
    {
        return actionType;
    }

    public List<TokenBase> GetTargetList()
    {
        return m_targetList;
    }

    public TokenType GetTargetType()
    {
        return actionTarget;
    }

    public void AddTarget(TokenBase _token)
    {
        m_targetList.Add(_token);
    }

    public void ClearTarget()
    {
        m_targetList.Clear();
    }
    #endregion

}
