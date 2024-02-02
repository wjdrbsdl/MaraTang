using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenChar : TokenBase
{
    public int actionCount = 0;
    public int charNum = 0;
    public bool isMainChar = false;

    #region 캐릭 토큰 생성부분
    public TokenChar()
    {

    }
    public TokenChar(TokenType _type)
    {
        //타입에 따라 생성되는걸로;
    }
    //이후 플레이어 토큰을 만들땐 매개변수에 플레이어 정보를 넣어서, 받는 형식으로. 아니면 그냥 해도되긴될듯. 
    public TokenChar MakePlayerToken()
    {
        TokenChar playerToken = new TokenChar();
        playerToken.m_tokenIValues = new int[(int)CharStat.StatSize];
        playerToken.m_tokenType = TokenType.Player;
        playerToken.SetStat(CharStat.MaxActonEnergy, 10);
        playerToken.SetStat(CharStat.CurActionEnergy, 10);
        playerToken.m_haveActionList = new List<TokenAction>();
        return playerToken;
    }
    public TokenChar MakeTestMonsterToken(string _name, int index)
    {
        TokenChar monsterToken = new TokenChar();
        monsterToken.m_itemName = _name;
        monsterToken.charNum = index;
        monsterToken.m_tokenIValues = new int[(int)CharStat.StatSize];
        monsterToken.m_tokenType = TokenType.Char;
        monsterToken.SetStat(CharStat.MaxActonEnergy, 10);
        monsterToken.SetStat(CharStat.CurActionEnergy, 10);
        monsterToken.m_haveActionList = new List<TokenAction>();
        return monsterToken;
    }
    #endregion
    private CharState m_state = CharState.Idle;
    private List<TokenAction> m_haveActionList = new(); //이 캐릭터가 지니고 있는 액션 토큰들
    private TokenAction m_nextAction = null;
    private TokenBase m_nextTarget;
    private bool m_isPlayerChar = false;

    public void ShowAction(bool isShow)
    {
        if (m_object == null)
            return;

        if (isShow && m_nextAction != null)
            m_object.ShowActionIcon(m_nextAction.GetActionType());
        else if (isShow == false)
            m_object.OffActionIcon();
    }

    public void SetState(CharState _state)
    {
        if (m_object == null)
            return;

        m_object.PlayAnimation(_state);
    }

    #region Get
    public TokenBase GetTarget()
    {
        return m_nextTarget;
    }

    public int GetActionCount()
    {
        return actionCount;
    }

    public List<TokenAction> GetActionList()
    {
        return m_haveActionList;
    }

    public TokenAction GetNextAction()
    {
        return m_nextAction;
    }

    public void ClearNextAction()
    {
        m_nextAction = null;
    }

    public TokenAction GetNextActionToken()
    {
        return m_nextAction;
    }

    public bool IsPlayerChar()
    {
        //나중엔 토큰매니저나 규칙으로 인해 m_isPlayerChar를 바꾸고 지금은 오브젝트에서 당기는걸로
        m_isPlayerChar = GetObject().m_testPlayerToken;
        return m_isPlayerChar;
    }
    #endregion

    #region Set
    public void SetNextAction(TokenAction _action)
    {
        m_nextAction = (_action);
    }

    public override void SetMapIndex(int _x, int _y)
    {
        base.SetMapIndex(_x, _y); //좌표상 옮기고
       // Debug.Log("<color=yellow>"+num + "번" + _x + " ," + _y + "으로 옮김</color>");
    }

    public void SetObjectPostion(int _x, int _y)
    {
        m_object.SyncObjectPosition(_x, _y);
    }

    public void SetTarget(TokenBase _tokenBase)
    {
        m_nextTarget = _tokenBase;
    }

    public void SetActionToken(TokenAction _action)
    {
        m_haveActionList.Add(_action);
    }
    #endregion

    public void RecoverActionTokenCount()
    {
        for (int i = 0; i < m_haveActionList.Count; i++)
        {
            m_haveActionList[i].RcoverRemainCountInTurn();
        }
    }
}
