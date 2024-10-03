using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PolicyLevelUp : NationPolicy
{
    
    public override void MakePlan()
    {
        Debug.Log("레벨 계획");
        TokenBase planToken = m_nation.GetCapital();
        SetPlanToken(planToken);

    }

    public override void Excute()
    {
        Debug.Log("레벨 집행");
        LevelUp();
    }

    private bool LevelUp()
    {
        if (AbleLevelUp() == false)
        {
            //   Debug.Log("레벨업 조건 미충족");
            return false;
        }

        Announcer.Instance.AnnounceState("국가 레벨 상승 :" + m_nation.GetNationLevel() + "Lv");
        int needPerson = m_nation.GetNationLevel() * 40;
        int needFood = needPerson;
        //비용 정산필요

        //CalResourceAmount(Capital.Food, -needFood);
        m_nation.LevelUp();
        return true;
    }


    private bool AbleLevelUp()
    {
        //비용정산 필요

        //int needPerson = m_nationLevel * 40;
        //int needFood = needPerson;
        //if (GetResourceAmount(Capital.Person) < needPerson)
        //{
        //    //   Debug.Log("등급상승 만족 인구 부족");
        //    return false;
        //}

        //if (GetResourceAmount(Capital.Food) < needFood)
        //{
        //    //  Debug.Log("등급상승 식량 부족");
        //    return false;
        //}
        return true;
    }


}

