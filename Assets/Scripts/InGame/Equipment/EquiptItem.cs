using System.Collections;
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

    public EquiptItem (int _pid, string _name, EquiptPartEnum _part, int _tier, List<TOrderItem> _effectList)
    {
        m_tokenType = TokenType.Equipt;
        m_tokenPid = _pid;
        m_itemName = _name;
        m_part = _part;
        m_effect = _effectList;
        m_tier = _tier;
    }


}
