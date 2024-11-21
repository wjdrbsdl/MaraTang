﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GodClassEnum
{
    무, 마법, 전사, 궁사
}

public enum BlessSubCategory
{
    전사_A,전사_B,전사_C,전사_1,전사_2
}

public class GodBless
{
    public int PID;
    public string Name;
    public GodClassEnum m_mainCategory = GodClassEnum.무; //가호 시너지 체크를 위해 분류 
    public BlessSubCategory m_subCategory = BlessSubCategory.전사_A; //신의 이름
    public List< TOrderItem> m_effect; //플레이어 스텟에 가하는 요소

    public GodBless(string[] _dbValueList)
    {
        PID = int.Parse(_dbValueList[0]);
        Name = _dbValueList[1];
        m_effect = new();
        GameUtil.ParseOrderItemList(m_effect, _dbValueList[3]);
        //for (int i = 0; i < m_effect.Count; i++)
        //{
        //   Debug.Log(GameUtil.FindEnum(m_effect[i].Tokentype, m_effect[i].SubIdx)+"에 효과" + m_effect[i].Value);
        //}
    }

    public GodBless(GodBless _origin)
    {
        PID = _origin.PID;
        Name = _origin.Name;
        m_mainCategory = _origin.m_mainCategory;
        m_subCategory = _origin.m_subCategory;
        m_effect = new();
        for (int i = 0; i < _origin.m_effect.Count; i++)
        {
            m_effect.Add(_origin.m_effect[i]);
        }
    }
}
