using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum CharStat
{
    MaxActionEnergy, CurActionEnergy, MaxActionCount, CurActionCount, Sight, Strenth, Dex, Inteligent,
    MaxHp, CurHp, EquiptSlotCount, BlessSlotCount, ActionSlotCount
}

public enum CharType
{
    Monster, Player, Devil, Npc
}
public class TokenChar : TokenBase
{
    public bool isMainChar = false;
    public bool m_isPlayerChar = false;
    public bool isLIve = true;

    private CharType m_charType = CharType.Monster;
    [JsonProperty] private CharState m_state = CharState.Idle;
    [JsonProperty] private List<TokenAction> m_haveActionList = new(); //�� ĳ���Ͱ� ���ϰ� �ִ� �׼� ��ū��
    private List<GodBless> m_blessList = new();
    private List<EquiptItem> m_equiptLIst = new(); //������ ����

    //���� �ٲ�� ���
    private TokenAction m_nextAction = null;
    private TokenTile m_nextTargetTile;
    private TokenChar m_nextTargetChar;
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
        m_charType = (CharType)System.Enum.Parse(typeof(CharType), valueCode[2]); //����Ÿ��
        SetAction(ref m_haveActionList, valueCode[3]); //3�� ���� �׼� pid
        m_tokenType = TokenType.Char;
        m_tokenIValues = new int[System.Enum.GetValues(typeof(CharStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)CharStat.CurActionCount] = m_tokenIValues[(int)CharStat.MaxActionCount];
        m_tokenIValues[(int)CharStat.CurActionEnergy] = m_tokenIValues[(int)CharStat.MaxActionEnergy];
        m_tokenIValues[(int)CharStat.CurHp] = m_tokenIValues[(int)CharStat.MaxHp];
        m_tokenIValues[(int)CharStat.EquiptSlotCount] = 3; //�ӽ÷� ���밡����� 3���� ����
        m_tokenIValues[(int)CharStat.BlessSlotCount] = 3;
        m_tokenIValues[(int)CharStat.ActionSlotCount] = 3;
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
        m_charType = _masterToken.m_charType;
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

    #endregion

    #region �׷���
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
    #endregion

    #region Get
    public List<GodBless> GetBlessList()
    {
        return m_blessList;
    }

    public TokenChar GetTargetChar()
    {
        return m_nextTargetChar;
    }

    public TokenTile GetTargetTile()
    {
        return m_nextTargetTile;
    }

    public int GetActionCount()
    {
        return m_tokenIValues[(int)CharStat.CurActionCount];
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

    public GuildCard GetGuildCard()
    {
        if(m_guildID == null)
        {
            m_guildID = new GuildCard();
        }
        return m_guildID;
    }

    public CharType GetCharType()
    {
        return m_charType;
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

    public void SetTargetChar(TokenChar _char)
    {
        m_nextTargetChar = _char;
    }

    public void SetTargetTile(TokenTile _tokenBase)
    {
        m_nextTargetTile = _tokenBase;
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

    #region ��ȣ 
    public bool AquireBless(GodBless _bless)
    {
        if (CheckBlessSlotCount() == true)
        {
            AddBless(_bless);
            return true;
        }

        //���������� ���
        if (IsPlayerChar() == true)
        {
          //  Debug.Log("���Ѻ��� UI���� �����Ұ��� ��û");
            MgUI.GetInstance().ShowPlayerBless();  //������ ��ȣ�� ��쵵�� ���� 
        }

        return false;
    }

    public bool HaveBless(GodBless _bless)
    {
        return IsHaveCheck<GodBless>(_bless, m_blessList);
    }

    public void AddBless(GodBless _bless)
    {
       // Debug.Log(_bless.Name + "���� �߰�");
   
        m_blessList.Add(_bless);
        AdaptBless(_bless);
        CheckActionSynerge();
    }

    public void RemoveBless(GodBless _bless)
    {
        m_blessList.Remove(_bless);
        List<TOrderItem> reverseEffect = GameUtil.ReverseItemList(_bless.m_effect);
        _bless.m_effect = reverseEffect;
        AdaptBless(_bless);
        CheckActionSynerge();
    }


    public void AdaptBless(GodBless _godBless)
    {
        // Debug.Log("��� Ŭ�������� ���� �����ϱ�");
        OrderExcutor excutor = new();
    
        for (int i = 0; i < _godBless.m_effect.Count; i++)
        {
            TOrderItem blessEffect = _godBless.m_effect[i];
            excutor.AdaptItem(blessEffect);
           // Debug.Log(blessEffect.Value);
        }

    }

    private void CheckActionSynerge()
    {
      //  Debug.Log("���� ��ų �� �ó��� üũ ����");
        for (int i = 0; i < m_haveActionList.Count; i++)
        {
            m_haveActionList[i].CheckBlessSynerge(this);
        }
    }

    private bool CheckBlessSlotCount()
    {

        return m_blessList.Count < GetStat(CharStat.BlessSlotCount);
    }
    #endregion

    #region �׼�
    public bool AquireAction(TokenAction _actionItem)
    {
        //1. �н��Ѱ��� üũ
        if (IsHaveAction(_actionItem) == true)
        {
            Debug.Log("�̹� ���� �� �׼�");
            return false;
        }

        if(CheckActionSlot() == true)
        {
            AddAction(_actionItem);
            return true;
        }

        if(IsPlayerChar() == true)
        {
            Debug.Log("�׼� ����â ��� ���� ����");
        }

        return false;
    }
    public void AddAction(TokenAction _actionItem)
    {
        if (IsHaveAction(_actionItem) == true)
            return;

        m_haveActionList.Add(_actionItem);

        //5. �н� �ڵ� ����
        TOrderItem aquireSkill = new TOrderItem(TokenType.Action, _actionItem.GetPid(), 1);
        MGContent.GetInstance().SendActionCode(aquireSkill);
        CheckActionSynerge();
    }
    public void RemoveAction(TokenAction _actionItem)
    {
        if (IsHaveAction(_actionItem) == false)
            return;

        m_haveActionList.Remove(_actionItem);
        CheckActionSynerge();
    }
    public bool CheckActionSlot()
    {
        return m_haveActionList.Count < GetStat(CharStat.ActionSlotCount);
    }

    public bool IsHaveAction(TokenAction _actionItem)
    {
        return IsHaveCheck<TokenAction>(_actionItem, m_haveActionList);
    }

    public List<TokenAction> GetActionList()
    {
        return m_haveActionList;
    }
    #endregion

    #region ���
    public bool AquireEquipt(EquiptItem _equiptItem)
    {
        //��� �������� �Ҷ� 
        //���������
        if(CheckEquiptSlot() == true)
        {
            AddEquipt(_equiptItem); //���� �����ϰ�
            return true;
        }
        //���������� ���
        if(IsPlayerChar() == true)
        {
          //  Debug.Log("��񽽷�ĭ���� �����Ұ��� ��û");
            MgUI.GetInstance().ShowPlayerEquipt();
        }

        return false;
    }

    public void AddEquipt(EquiptItem _equiptItem)
    {
        //  Debug.Log("�ش��ɸ��� ��� ���� ���� " + _equiptitem.GetPid());
        if (m_equiptLIst.IndexOf(_equiptItem) != -1)
        {
          //  Debug.Log("�����ϴ� ������");
            return;
        }
        m_equiptLIst.Add(_equiptItem);
        AdaptEquipt(_equiptItem);
    }

    public void RemoveEquipt(EquiptItem _equiptItem)
    {
        if (m_equiptLIst.IndexOf(_equiptItem) == -1)
        {
          //  Debug.Log("�������� �ʴ� ���");
            return;
        }
            

        m_equiptLIst.Remove(_equiptItem);
        List<TOrderItem> reverseEffect = GameUtil.ReverseItemList(_equiptItem.m_effect);
        _equiptItem.m_effect = reverseEffect;
        AdaptEquipt(_equiptItem);
    }

    public void AdaptEquipt(EquiptItem _equiptItem)
    {
        // Debug.Log("��� Ŭ�������� ���� �����ϱ�");
        OrderExcutor excutor = new();

        for (int i = 0; i < _equiptItem.m_effect.Count; i++)
        {
            TOrderItem blessEffect = _equiptItem.m_effect[i];
            excutor.AdaptItem(blessEffect);
            // Debug.Log(blessEffect.Value);
        }

    }

    public bool CheckEquiptSlot()
    {
        return m_equiptLIst.Count < GetStat(CharStat.EquiptSlotCount);
    }

    public List<EquiptItem> GetEquiptList()
    {
        return m_equiptLIst;
    }
    #endregion

    private bool IsHaveCheck<T>(TokenBase _token, List<T> _list) where T : TokenBase
    {
        int checkPid = _token.GetPid();
        for (int i = 0; i < _list.Count; i++)
        {
            if (_list[i].GetPid() == checkPid)
                return true;
        }
        return false;
    }

    #region ���
    public void Death()
    {
        if (isMainChar)
        {
            PlayerDeath();
            return;
        }

        //0. ������Ʈ ����
        if (m_object != null)
            m_object.DestroyObject();

        isLIve = false;
        //1. ����� ó��
        DropItem();

        //2. ������ ���� ����
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.RemoveCharToken(this);
        MgToken.GetInstance().RemoveCharToken(this);
    }

    private void PlayerDeath()
    {
        GamePlayMaster.GetInstance().PlayerDead();
    }

    public void DropItem()
    {
        DropItemManager.GetInstance().DropItem(GetPid());
        GameUtil.DropMagnetItem(GetMapIndex());
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
    #endregion

}
