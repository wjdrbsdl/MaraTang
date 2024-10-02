using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NationPopular
{
    private Nation m_nation;
    private int m_mealRatio = 10; //필요 식량 비율 - 식량 효율이 좋을수록 값 감소

    public NationPopular(Nation _nation)
    {
        m_nation = _nation;
    }

    public void ManagePopular()
    {
        //1. 현재 인구수 만큼 식량 소비 진행 
        ExpandMeal();
    }

    private void ExpandMeal()
    {
        int population = m_nation.GetResourceAmount(Capital.Person);
        int meal = m_nation.GetResourceAmount(Capital.Food);
        int ablePopulation = AbleEatPopulation();
        if (ablePopulation < population)
        {
            //식량이 부족하면 먹일수 있는 식량 만큼 식량 감소
            int ableMeal = ablePopulation * m_mealRatio;
            m_nation.CalResourceAmount(Capital.Food, -ableMeal);
            int hungryPopulation = population - ablePopulation; //먹지 못한 인간들
            Debug.Log(hungryPopulation + "인구가 먹지 못함");
        }
        else if(population <= ablePopulation)
        {
            //식량이 풍족하면 현재 인원수만큼 식량 감소
            int needMeal = population * m_mealRatio;
            m_nation.CalResourceAmount(Capital.Food, -needMeal);
        }

    }

    private int AbleEatPopulation()
    {
        return m_nation.GetResourceAmount(Capital.Food) % m_mealRatio;
    }

}
