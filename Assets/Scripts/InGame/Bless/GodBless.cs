using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GodClassEnum
{
    무, 마법, 전사, 궁사, 여정
}

public class GodBless : TokenBase
{
    public GodClassEnum m_classCategory = GodClassEnum.무; //가호 시너지 체크를 위해 분류 
    public int m_godPid = 0; //신의 이름
    public List< TOrderItem> m_effect; //플레이어 스텟에 가하는 요소

    public GodBless(string[] _dbValueList)
    {
        m_tokenPid = int.Parse(_dbValueList[0]);
        m_tokenType = TokenType.Bless;
        m_itemName = _dbValueList[1];
     
        m_effect = new();
        GameUtil.ParseOrderItemList(m_effect, _dbValueList[2]);


        //for (int i = 0; i < m_effect.Count; i++)
        //{
        //   Debug.Log(GameUtil.FindEnum(m_effect[i].Tokentype, m_effect[i].SubIdx)+"에 효과" + m_effect[i].Value);
        //}
    }

    public GodBless(GodBless _origin)
    {
        m_tokenPid  = _origin.m_tokenPid ;
        m_tokenType = _origin.m_tokenType;
        m_itemName = _origin.m_itemName;
        m_classCategory = _origin.m_classCategory;
        m_godPid = _origin.m_godPid;
        m_effect = new();
        for (int i = 0; i < _origin.m_effect.Count; i++)
        {
            m_effect.Add(_origin.m_effect[i]);
        }
    }
}
