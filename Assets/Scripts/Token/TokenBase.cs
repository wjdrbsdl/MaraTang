using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Tier
{
    Nomal, Magic, Rare, Unique, Legend
}

public enum TokenType
{
    Tile, Char, Item, Action, Player, Capital, Event
}

public class TokenBase
{
    #region ������ ����
    [SerializeField]
    protected int[] m_tokenIValues; //�� ��ū���� ����� �������� enum���� �����ؼ� �ε����� ���
    [SerializeField]
    protected int m_xIndex = 0;//������ ��� ������
    [SerializeField]
    protected int m_yIndex = 0; //������ ��� ������
    [SerializeField]
    protected TokenType m_tokenType;
    protected int m_tokenPid;
    protected Tier m_tier;
    protected string m_itemName;
    #endregion

    #region ����Ƽ ����
    protected ObjectTokenBase m_object;
    public Sprite TokenImage;
    public GameObject Prefab; //���� ���¸� ���� ������ ���� �κ�. 
    public Quest QuestCard;
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
    public Sprite GetIcon()
    {
        return TokenImage;
    }

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

    public ObjectTokenBase GetObject()
    {
        return m_object;
    }

    public TokenType GetTokenType()
    {
        return m_tokenType;
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

    public void SendQuestCallBack()
    {
        if (QuestCard != null)
            QuestCard.SendQuestCallBack(this);
    }
}
