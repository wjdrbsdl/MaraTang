using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlessMainCategory
{
    무, 마법, 전사, 궁사
}

public class GodBless
{
    public int PID;
    public BlessMainCategory m_mainCategory = BlessMainCategory.무; //가호 시너지 체크를 위해 분류 
    public List< TOrderItem> m_effect; //플레이어 스텟에 가하는 요소

    public GodBless(string[] _dbValueList)
    {
        PID = int.Parse(_dbValueList[0]);
        m_effect = new();
        GameUtil.ParseOrderItemList(m_effect, _dbValueList[3]);
        for (int i = 0; i < m_effect.Count; i++)
        {
           Debug.Log(GameUtil.FindEnum(m_effect[i].Tokentype, m_effect[i].SubIdx)+"에 효과" + m_effect[i].Value);
        }
    }
}
