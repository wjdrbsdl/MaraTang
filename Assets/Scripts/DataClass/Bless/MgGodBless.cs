using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MgGodBless : MgGeneric<MgGodBless>
{
    //은총 데이터 모아놓고
    //신전으로부터 은총 요구를 받으면 적당한 은총을 하사
    private List<GodBless> m_blessList = new();

    public override void ReferenceSet()
    {
        for (int i = 0; i < 5; i++)
        {
            GodBless newBless = new GodBless();
            m_blessList.Add(newBless);
        }
    }

    public GodBless PleaseBless()
    {
        return m_blessList[0];
    }

}
