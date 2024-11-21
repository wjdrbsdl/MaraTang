using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum EquiptEnum
{
    PartType
}

public class EquiptItem : TokenBase
{
    public List<TOrderItem> m_effect = new(); //플레이어 스텟에 가하는 요소

    public EquiptItem()
    {
        
    }

    public EquiptItem(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]);
        m_itemName = valueCode[1];

    }

    public EquiptItem(EquiptItem _masterData)
    {
        m_tokenPid = _masterData.m_tokenPid;
        m_itemName = _masterData.m_itemName;
        for (int i = 0; i < _masterData.m_effect.Count; i++)
        {
            TOrderItem effect = _masterData.m_effect[i];
            m_effect.Add(effect);
        }
    }

}
