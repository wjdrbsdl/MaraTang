using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum EquiptEnum
{
    PartType
}

public class EquiptItem
{
    private int m_pid;
    private string m_name;
    public List<TOrderItem> m_effect = new(); //플레이어 스텟에 가하는 요소

    public EquiptItem()
    {
        
    }

    public EquiptItem(List<int[]> matchCode, string[] valueCode)
    {
        m_pid = int.Parse(valueCode[0]);
        m_name = valueCode[1];

    }

    public EquiptItem(EquiptItem _masterData)
    {
        m_pid = _masterData.m_pid;
        m_name = _masterData.m_name;
        for (int i = 0; i < _masterData.m_effect.Count; i++)
        {
            TOrderItem effect = _masterData.m_effect[i];
            m_effect.Add(effect);
        }
    }

    public int GetPid()
    {
        return m_pid;
    }
}
