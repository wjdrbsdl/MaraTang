using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class BlessAdaptor 
{
    public static BlessAdaptor g_instnace;

    public BlessAdaptor()
    {
        g_instnace = this;
    }

    public void AdaptBless(TokenChar _char)
    {
       // Debug.Log("어뎁터 클래스에서 블래스 적용하기");
        GodBless _bless = _char.GetBlessList()[0];
        _char.CalStat((CharStat)_bless.m_effect[0].SubIdx, 30);
    }
}
