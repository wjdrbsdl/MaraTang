﻿using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum EquiptPartEnum
{
    None, Weapon, Armor, Shoese, Gloves, Helmet
}

public class EquiptItem : TokenBase
{
    public EquiptPartEnum m_part = EquiptPartEnum.None;
    public List<TOrderItem> m_effect = new(); //플레이어 스텟에 가하는 요소

    public EquiptItem()
    {
        
    }

    public EquiptItem(string[] _dbValueList)
    {
        m_tokenPid = int.Parse(_dbValueList[0]);
        m_tokenType = TokenType.Equipt;
        m_itemName = _dbValueList[1];

        int partIdx = 2;
        if (System.Enum.TryParse(typeof(EquiptPartEnum), _dbValueList[partIdx], out object parsePart))
            m_part = (EquiptPartEnum)parsePart;

    }

    public EquiptItem(EquiptItem _masterData)
    {
        m_tokenPid = _masterData.m_tokenPid;
        m_tokenType = _masterData.m_tokenType;
        m_itemName = _masterData.m_itemName;
        for (int i = 0; i < _masterData.m_effect.Count; i++)
        {
            TOrderItem effect = _masterData.m_effect[i];
            m_effect.Add(effect);
        }
    }

}
