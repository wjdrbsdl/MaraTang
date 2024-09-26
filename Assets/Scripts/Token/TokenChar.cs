using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum CharStat
{
    MaxActionEnergy, CurActionEnergy, MaxActionCount, CurActionCount, Sight, Strenth, Dexility, Inteligent,
    MaxHp, CurHp
}

public class TokenChar : TokenBase
{
    public bool isMainChar = false;
    public bool m_isPlayerChar = false;

    [JsonProperty] private CharState m_state = CharState.Idle;
    [JsonProperty] private List<TokenAction> m_haveActionList = new(); //�� ĳ���Ͱ� ���ϰ� �ִ� �׼� ��ū��
    private TokenAction m_nextAction = null;
    private TokenBase m_nextTarget;
    private TokenCharMood m_mood;
    private GuildCard m_guildID;

    #region ĳ�� ��ū �����κ�
    public TokenChar()
    {

    }

    //������ ĳ�������� ����
    public TokenChar(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]); //��Ʈ �����ͻ� 0��°�� pid
        m_itemName = valueCode[1]; //1�� �̸�
        SetAction(ref m_haveActionList, valueCode[2]); //2�� ���� �׼� pid
        m_tokenType = TokenType.Char;
        m_tokenIValues = new int[System.Enum.GetValues(typeof(CharStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)CharStat.CurActionCount] = m_tokenIValues[(int)CharStat.MaxActionCount];
        m_tokenIValues[(int)CharStat.CurActionEnergy] = m_tokenIValues[(int)CharStat.MaxActionEnergy];
        m_tokenIValues[(int)CharStat.CurHp] = m_tokenIValues[(int)CharStat.MaxHp];
        
        
    }

    private void SetAction(ref List<TokenAction> _haveAction, string actionCode)
    {
        string[] actions = actionCode.Split(FixedValue.PARSING_LIST_DIVIDE);
        for (int i = 0; i < actions.Length; i++)
        {
            int actionPid = int.Parse(actions[i]);
            TokenAction masterAction = MgMasterData.GetInstance().GetMasterCharAction(actionPid);
            //���ǵ��� ���� �׼��̶�� �ѱ�. 
            if (masterAction == null)
                continue;
            TokenAction charAction = new TokenAction(masterAction);
            _haveAction.Add(charAction);
        }
    }

    //���纻 ĳ�� ���� : ĳ���� ���� �� ���
    public TokenChar(TokenChar _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = TokenType.Char;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //������ ������ ���� ����� ��ü ���� �迭 �� ����. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //���ݰ� ����
        //��ų �����Ͱ� ����
        for (int i = 0; i < _masterToken.GetActionList().Count; i++)
        {
            TokenAction copyAction = new TokenAction(_masterToken.GetActionList()[i]);
            m_haveActionList.Add(copyAction);
        }
    }

    public void MakeMood()
    {
        m_mood = new TokenCharMood();
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

    public void SetSprite()
    {
        //pid�� �°� objectToken�� ��������Ʈ�� ����
        int spriteIdx = m_tokenPid - 1;
        GetObject().SetSprite(TempSpriteBox.GetInstance().GetCharSprite(spriteIdx));
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
        
        return m_isPlayerChar;
    }

    public GuildCard GetGuildID()
    {
        return m_guildID;
    }
    #endregion

    #region Set
    public void SetGuildID(GuildCard _guildID)
    {
        m_guildID = _guildID;
    }

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

    public void RecoverActionCount()
    {
        m_tokenIValues[(int)CharStat.CurActionCount] = m_tokenIValues[(int)CharStat.MaxActionCount];
    }

    public void UseActionCount(int _useCount = 1)
    {
        m_tokenIValues[(int)CharStat.CurActionCount] -= _useCount;
    }

    public void RecoverActionEnergy()
    {
        m_tokenIValues[(int)CharStat.CurActionEnergy] = m_tokenIValues[(int)CharStat.MaxActionEnergy];
    }

    public void UseActionEnergy(int _useEnergy = 1)
    {
        m_tokenIValues[(int)CharStat.CurActionEnergy] -= _useEnergy;
    }
    #endregion

    public void Death()
    {
        //0. ������Ʈ ����
        if (m_object != null)
            m_object.DestroyObject();

        //1. ����� ó��
        SendQuestCallBack();
        DropItem();

        //2. ������ ���� ����
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.RemoveCharToken(this);
        MgToken.GetInstance().RemoveCharToken(this);
    }

    protected override void SendQuestCallBack()
    {
        base.SendQuestCallBack();
        if (m_policy != null)
            m_policy.SendNationCallBack(this);

    }

    public override void CleanToken()
    {
        base.CleanToken();
        Debug.Log("ĳ���� ���� ��");
        //0. ������Ʈ ���� 
        if (m_object != null)
            m_object.DestroyObject();

        //1. ������ ���� ����
        TokenTile place = GameUtil.GetTileTokenFromMap(GetMapIndex());
        place.RemoveCharToken(this);
        MgToken.GetInstance().RemoveCharToken(this);
    }
}
