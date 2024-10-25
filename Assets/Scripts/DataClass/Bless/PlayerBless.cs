using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBless 
{
    public List<GodBless> m_haveList;
    public static PlayerBless g_instnace;

    public PlayerBless()
    {
        g_instnace = this;
        m_haveList = new();
    }

    public void AddBless(GodBless _bless)
    {
        Debug.Log("보유 은총 추가");
        m_haveList.Add(_bless);
    }
}
