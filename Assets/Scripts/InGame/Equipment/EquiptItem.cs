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
    public List<TOrderItem> m_effect = new(); //플레이어 스텟에 가하는 요소

    public EquiptItem()
    {
        
    }

    public EquiptItem(List<int[]> matchCode, string[] valueCode)
    {

    }

    public int GetPid()
    {
        return m_pid;
    }
}
