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
        Debug.Log("블래스 적용하기");
    }
}
