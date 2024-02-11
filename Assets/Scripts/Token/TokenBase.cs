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
    protected int[] m_tokenIValues; //각 토큰마다 사용할 벨류들을 enum으로 선언해서 인덱스로 사용
    [SerializeField]
    protected int m_xIndex = 0;//지도상 행렬 포지션
    [SerializeField]
    protected int m_yIndex = 0; //지도상 행렬 포지션
    [SerializeField]
    protected TokenType m_tokenType;
    protected int m_tokenPid;
    protected Sprite m_tokenImage;
    protected GameObject m_prefeb; //뭔가 형태를 띄우고 싶을때 쓰는 부분. 
    //[JsonProperty] 
    protected TokenTier m_tier;
    // [JsonProperty] 
    protected string m_itemName;
    protected ObjectTokenBase m_object;

    #region Reset
    //오리지널 Class 생성시 필요한 유니티 자료를 연계하는곳, 그 자료명은 아이템명에 + icon, prefeb 형식으로 지정. 
    public void SetIconFromResource(string _resourcePath = null)
    {
        //클래스명 + Icon 으로 네이밍을해서 불러오며, sprite가 준비되지않은경우에는, testIcon으로 대체해서 진행하고, 이름도 바꿔서, json으로 미비된 부분을 확인할 수 있도록 하기
        string iconName = m_itemName + " Icon";
        m_tokenImage = Resources.Load<Sprite>(_resourcePath + iconName);
        if (m_tokenImage == null)
        {
            // Debug.LogError(iconName +" 확인필요");
            if (iconName == " Icon")
                Debug.LogError(this.GetType().Name);
            iconName = "TestBear";
            m_tokenImage = Resources.Load<Sprite>(iconName);
        }
    }

    public void SetPrefebFromResource(string _resourcePath = null)
    {
        //리소스 폴더내에 따로 폴더를 통해 루트를 수정했으면 해당 수정된 루트를 입력
        //일단은 CropPrefeb, SkillPrefeb 두 폴더를 상정 하고 최초 DB를 생성하는 DbItem.cs와 DbSkill.cs 에서 리셋할때 입력하도록 진행. 
        string prefebName = m_itemName + " Prefeb";
        m_prefeb = Resources.Load<GameObject>(_resourcePath + prefebName);
        if (m_prefeb == null)
        {
            // Debug.LogError(prefebName+ " 없는 파일명 확인필요");
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
    public void CalStat(System.Enum _enumIndex, int _value)
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
}
