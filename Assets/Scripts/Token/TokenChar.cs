using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharStat
{
    MaxActionEnergy, CurActionEnergy, MaxActionCount, CurActionCount, Sight
}

public class TokenChar : TokenBase
{
    public bool isMainChar = false;
    private bool m_isPlayerChar = false;

    private CharState m_state = CharState.Idle;
    private List<TokenAction> m_haveActionList = new(); //�� ĳ���Ͱ� ���ϰ� �ִ� �׼� ��ū��
    private TokenAction m_nextAction = null;
    private TokenBase m_nextTarget;
    

    #region ĳ�� ��ū �����κ�
    public TokenChar()
    {

    }
    public TokenChar(TokenType _type)
    {
        //Ÿ�Կ� ���� �����Ǵ°ɷ�;
    }
    //���� �÷��̾� ��ū�� ���鶩 �Ű������� �÷��̾� ������ �־, �޴� ��������. �ƴϸ� �׳� �ص��Ǳ�ɵ�. 
    public TokenChar MakePlayerToken()
    {
        TokenChar playerToken = new TokenChar();
        playerToken.m_tokenIValues = new int[GameUtil.EnumLength(CharStat.CurActionEnergy)];
        playerToken.m_tokenType = TokenType.Player;
        playerToken.SetStatValue(CharStat.MaxActionEnergy, 10);
        playerToken.SetStatValue(CharStat.CurActionEnergy, 10);
        playerToken.m_haveActionList = new List<TokenAction>();
        return playerToken;
    }
    public static TokenChar MakeTestMonsterToken(string _name, int index)
    {
        TokenChar monsterToken = new TokenChar();
        monsterToken.m_itemName = _name;
        monsterToken.m_tokenIValues = new int[GameUtil.EnumLength(CharStat.CurActionEnergy)];
        monsterToken.m_tokenType = TokenType.Char;
        monsterToken.SetStatValue(CharStat.MaxActionEnergy, 10);
        monsterToken.SetStatValue(CharStat.CurActionEnergy, 10);
        monsterToken.SetStatValue(CharStat.MaxActionCount, 2);
        monsterToken.SetStatValue(CharStat.CurActionCount, 2);
        monsterToken.SetStatValue(CharStat.Sight, 3);
        monsterToken.m_haveActionList = new List<TokenAction>();
        monsterToken.m_tokenPid = index * 100;
        monsterToken.SetState(CharState.Sleep);
        return monsterToken;
    }
    #endregion

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
        m_state = _state;
        if (m_object == null)
            return;

        m_object.PlayAnimation(_state);
    }

    public CharState GetState()
    {
        return m_state;
    }

    #region Get
    public TokenBase GetTarget()
    {
        return m_nextTarget;
    }

    public int GetActionCount()
    {
        return m_tokenIValues[(int)CharStat.CurActionCount];
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
        //���߿� ��ū�Ŵ����� ��Ģ���� ���� m_isPlayerChar�� �ٲٰ� ������ ������Ʈ���� ���°ɷ�
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
        base.SetMapIndex(_x, _y); //��ǥ�� �ű��
       // Debug.Log("<color=yellow>"+num + "��" + _x + " ," + _y + "���� �ű�</color>");
    }

    public void SetObjectPostion(int _x, int _y)
    {
        m_object.SyncObjectPosition(_x, _y);
    }

    public void SetTarget(TokenBase _tokenBase)
    {
        m_nextTarget = _tokenBase;
    }

    public void AddActionToken(TokenAction _action)
    {
        m_haveActionList.Add(_action);
    }
    #endregion

    #region �׼� ī��Ʈ ȸ��
    public void RecoverActionTokenCount()
    {
        for (int i = 0; i < m_haveActionList.Count; i++)
        {
            m_haveActionList[i].RcoverRemainCountInTurn();
        }
    }

    public void RecvoerActionCount()
    {
        m_tokenIValues[(int)CharStat.CurActionCount] = m_tokenIValues[(int)CharStat.MaxActionCount];
    }

    public void UseActionCount(int _useCount = 1)
    {
        m_tokenIValues[(int)CharStat.CurActionCount] -= _useCount;
    }

    public void UseActionEnergy(int _useEnergy = 1)
    {
        m_tokenIValues[(int)CharStat.CurActionEnergy] -= _useEnergy;
    }
    #endregion

    public void Death()
    {
        if (m_object != null)
            m_object.Death();

        TokenTile place = GameUtil.GetTileTokenFromMap(GetMapIndex());
        place.RemoveToken(this);

        SendQuestCallBack();
    }
}
