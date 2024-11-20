using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public enum EquiptEnum
{
    PartType
}

public class EquiptItem
{
    public TOrderItem m_effect; //플레이어 스텟에 가하는 요소

    public EquiptItem()
    {
        m_effect = new TOrderItem(TokenType.CharStat, (int)CharStat.MaxHp, 30);
    }
}
