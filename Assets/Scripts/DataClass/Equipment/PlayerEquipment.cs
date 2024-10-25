using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerEquipment
{
    public List<EquiptItem> m_haveList;
    public List<EquiptItem> m_equipList;
    public static PlayerEquipment g_instnace;

    public PlayerEquipment()
    {
        g_instnace = this;
        m_haveList = new();
    }

    public void AddEquipment(EquiptItem _equipment)
    {
        Debug.Log("보유 은총 추가");
        m_haveList.Add(_equipment);
    }

    public void Equipt(EquiptItem _equipment)
    {
       
    }
}
