using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyLevelUp : NationPolicy
{
    
    public override void MakePlan()
    {
       // Debug.Log("레벨 계획");
        TokenBase planToken = m_nation.GetCapital();
        SetPlanToken(planToken);

    }

    public override void Excute()
    {
     //   Debug.Log("레벨 집행");
        LevelUp();
    }

    private bool LevelUp()
    {
        Announcer.Instance.AnnounceState("국가 레벨 상승 :" + m_nation.GetNationLevel() + "Lv");
    
        m_nation.LevelUp();
        return true;
    }


}

