using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public enum Tier
{
    Nomal, Magic, Rare, Unique, Legend
}

public enum OnChangeEnum
{
    OnPlaceChange
}

public enum TokenType
{
    Tile, Char, CharStat, Action, Player, Capital, Event, Nation, None, Content, NationTech, Conversation, NationStat, OnChange, Bless
}

public class TokenBase
{
    #region ������ ����
    [JsonProperty] protected string m_itemName;
    [JsonProperty] protected int m_tokenPid;
    [JsonProperty] protected Tier m_tier;

    [JsonProperty][SerializeField]
    protected int[] m_tokenIValues; //�� ��ū���� ����� �������� enum���� �����ؼ� �ε����� ���
    [JsonProperty][SerializeField]
    protected int m_xIndex = 0;//������ ��� ������
    [JsonProperty][SerializeField]
    protected int m_yIndex = 0; //������ ��� ������
    [JsonProperty][SerializeField]
    protected TokenType m_tokenType;
    #endregion

    #region ����Ƽ ����
    public NationPolicy m_policy;
    protected ObjectTokenBase m_object;
    #endregion

    #region ������
    public TokenBase()
    {

    }

    public TokenBase(Capital _capital, int _value = 0)
    {
        //�ڿ��� ��ū ����
        m_tokenIValues = new int[GameUtil.EnumLength(CapitalStat.Amount)];
        m_tokenPid = (int)_capital;
        m_tokenType = TokenType.Capital;
        m_itemName = _capital.ToString();
        if (_value != 0)
            m_tokenIValues[(int) CapitalStat.Amount] = _value;
    }
    #endregion

    #region Get Set
    public string GetItemName()
    {
        return m_itemName;
    }

    public int GetPid()
    {
        return m_tokenPid;
    }

    public void SetObject(ObjectTokenBase _obj)
    {
        m_object = _obj;
    }

    public void SetSprite(Sprite _sprite)
    {
        m_object.SetSprite(_sprite);
    }

    public ObjectTokenBase GetObject()
    {
        return m_object;
    }

    public TokenType GetTokenType()
    {
        return m_tokenType;
    }

    public void SetPolicy(NationPolicy _policy)
    {
        m_policy = _policy; //��å ��� ����
    }

    public NationPolicy GetPolicy()
    {
        return m_policy;
    }

    public void ResetPolicy()
    {
        m_policy = null;
    }
    #endregion

    #region ���� �迭 �����ϴ� �κ�
    public int GetStat(System.Enum _enumIndex)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        return m_tokenIValues[index];
    }
    public void SetStatValue(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_tokenIValues[index] = _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    public virtual void CalStat(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex); 
        m_tokenIValues[index] += _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    #endregion

    #region ��ǥ ��
    public int[] GetMapIndex()
    {
        return new int[] { m_xIndex, m_yIndex };
    }

    public int GetXIndex()
    {
        return m_xIndex;
    }
    public int GetYIndex()
    {
        return m_yIndex;
    }
    public virtual void SetMapIndex(int _x, int _y)
    {
        //������ ��� �Է� 
        m_xIndex = _x;
        m_yIndex = _y;
    }

    #endregion

    public void DropItem()
    {
        GameUtil.DropMagnetItem(GetMapIndex());
    }

    protected virtual void SendQuestCallBack()
    {

    }

    public virtual void CleanToken()
    {
        //�ش� token�� �ƹ��� ������� �����ϴ� �κ� 
        //1. ���� - ���� ���� ����, mapIndex���� ����, ������Ʈ ����
        //2. ���� - �����ִ� �������� ����, ������Ʈ ����
        //3. �̺�Ʈ - �����ִ� �������� ����, ������Ʈ ���� 
    }

}
