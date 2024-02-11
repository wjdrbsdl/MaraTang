using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TokenTier
{
    Nomal, Magic, Rare, Unique, Legend
}

public enum CharExpand
{
    A
}



public enum TokenType
{
    Tile, Char, Item, Action, Player
}

[System.Serializable]
public class TokenBase
{
    [SerializeField]
    protected int[] m_tokenIValues; //�� ��ū���� ����� �������� enum���� �����ؼ� �ε����� ���
    [SerializeField]
    protected int m_xIndex = 0;//������ ��� ������
    [SerializeField]
    protected int m_yIndex = 0; //������ ��� ������
    [SerializeField]
    protected TokenType m_tokenType;
    protected int m_tokenPid;
    protected Sprite m_tokenImage;
    protected GameObject m_prefeb; //���� ���¸� ���� ������ ���� �κ�. 
    //[JsonProperty] 
    protected TokenTier m_tier;
    // [JsonProperty] 
    protected string m_itemName;
    protected ObjectTokenBase m_object;

    #region Reset
    //�������� Class ������ �ʿ��� ����Ƽ �ڷḦ �����ϴ°�, �� �ڷ���� �����۸� + icon, prefeb �������� ����. 
    public void SetIconFromResource(string _resourcePath = null)
    {
        //Ŭ������ + Icon ���� ���̹����ؼ� �ҷ�����, sprite�� �غ����������쿡��, testIcon���� ��ü�ؼ� �����ϰ�, �̸��� �ٲ㼭, json���� �̺�� �κ��� Ȯ���� �� �ֵ��� �ϱ�
        string iconName = m_itemName + " Icon";
        m_tokenImage = Resources.Load<Sprite>(_resourcePath + iconName);
        if (m_tokenImage == null)
        {
            // Debug.LogError(iconName +" Ȯ���ʿ�");
            if (iconName == " Icon")
                Debug.LogError(this.GetType().Name);
            iconName = "TestBear";
            m_tokenImage = Resources.Load<Sprite>(iconName);
        }
    }

    public void SetPrefebFromResource(string _resourcePath = null)
    {
        //���ҽ� �������� ���� ������ ���� ��Ʈ�� ���������� �ش� ������ ��Ʈ�� �Է�
        //�ϴ��� CropPrefeb, SkillPrefeb �� ������ ���� �ϰ� ���� DB�� �����ϴ� DbItem.cs�� DbSkill.cs ���� �����Ҷ� �Է��ϵ��� ����. 
        string prefebName = m_itemName + " Prefeb";
        m_prefeb = Resources.Load<GameObject>(_resourcePath + prefebName);
        if (m_prefeb == null)
        {
            // Debug.LogError(prefebName+ " ���� ���ϸ� Ȯ���ʿ�");
            if (m_itemName == " Prefeb")
                Debug.LogError(this.GetType().Name);
            prefebName = "TestAttack";
            m_prefeb = Resources.Load<GameObject>(prefebName);
        }
    }
    #endregion

    #region Get Set
    public Sprite GetIcon()
    {
        return m_tokenImage;
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
    public void CalStat(System.Enum _enumIndex, int _value)
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
}
