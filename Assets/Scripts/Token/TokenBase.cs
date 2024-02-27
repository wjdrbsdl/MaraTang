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
    #region 데이터 변수
    [SerializeField]
    protected int[] m_tokenIValues; //각 토큰마다 사용할 벨류들을 enum으로 선언해서 인덱스로 사용
    [SerializeField]
    protected int m_xIndex = 0;//지도상 행렬 포지션
    [SerializeField]
    protected int m_yIndex = 0; //지도상 행렬 포지션
    [SerializeField]
    protected TokenType m_tokenType;
    protected int m_tokenPid;
    protected Tier m_tier;
    protected string m_itemName;
    #endregion

    #region 유니티 변수
    protected ObjectTokenBase m_object;
    public Sprite TokenImage;
    public GameObject Prefab; //뭔가 형태를 띄우고 싶을때 쓰는 부분. 
    public Quest QuestCard;
    #endregion

    #region 생성자
    public TokenBase()
    {

    }

    public TokenBase(Capital _capital, int _value = 0)
    {
        //자원용 토큰 생성
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

    #region 스텟 배열 적용하는 부분
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

    #region 좌표 값
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
        //생성된 행렬 입력 
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
