using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NationPopular
{
    private Nation m_nation;
    private int m_mealRatio = 10; //필요 식량 비율 - 식량 효율이 좋을수록 값 감소
    private int hungPeople = 0;
    private List<LaborCoin> m_LaborCoin; //노동코인 
    public NationPopular(Nation _nation)
    {
        m_nation = _nation;
        m_LaborCoin = new();
    }

    public void ManagePopular()
    {
        //1. 현재 인구수 만큼 식량 소비 진행 
        EatMeal();
        //2. 굶은 사람에 대한 페널티 적용
        AdaptHungryPenalty();
        //3. 남은 노동 토큰 배치
        PlaceLaborCoin();
    }

    public void IncreaseLaborCoin(int _count)
    {
        for (int i = 0; i < _count; i++)
        {
            int curLaborCount = m_LaborCoin.Count; //리스트에서 순번이 곧 pid
            LaborCoin newLabor = new LaborCoin(curLaborCount, m_nation);
           // Debug.Log(m_nation.GetNationNum() + "국가에 " + newLabor.ListIndex + "노동 코인 생성");
            m_LaborCoin.Add(newLabor);
        }
    }

    private void EatMeal()
    {
        int population = m_nation.GetResourceAmount(Capital.Person);
        int meal = m_nation.GetResourceAmount(Capital.Food);
        int ablePeople = AbleEatPopulation();
        hungPeople = 0; //굶는사람 0
        if (ablePeople < population)
        {
            //식량이 부족하면 먹일수 있는 식량 만큼 식량 감소
            int ableMeal = ablePeople * m_mealRatio;
            m_nation.CalResourceAmount(Capital.Food, -ableMeal);
            Debug.Log("기존 식량" + meal +"식후 식량"+m_nation.GetResourceAmount(Capital.Food));
            hungPeople = population - ablePeople; //먹지 못한 인간들
            Debug.Log(hungPeople + "인구가 먹지 못함");
        }
        else if(population <= ablePeople)
        {
            //식량이 풍족하면 현재 인원수만큼 식량 감소
            int needMeal = population * m_mealRatio;
            m_nation.CalResourceAmount(Capital.Food, -needMeal);
        }

    }

    private int AbleEatPopulation()
    {
        return m_nation.GetResourceAmount(Capital.Food) / m_mealRatio;
    }

    private void AdaptHungryPenalty()
    {
        if (hungPeople == 0)
        {
           // Debug.Log("굶은사람 없음");
            return;
        }

        m_nation.DeadPeople(hungPeople);
      //  Debug.Log("굶은사람 사망" + hungPeople);
    }

    private void PlaceLaborCoin()
    {
        List<LaborCoin> noWorkLabor = new();
        for (int i = 0; i < m_LaborCoin.Count; i++)
        {
            LaborCoin labor = m_LaborCoin[i];
            if(labor.Pos.SequenceEqual(m_nation.GetCapital().GetMapIndex()))
            {
                //휴식하는 코인 찾아서
                noWorkLabor.Add(labor);
            }
        }

        List<TokenTile> territory = m_nation.GetTerritorry();
        //0은 수도이므로 1부터 체크
        int restCoinCount = noWorkLabor.Count;

        //남은 노동코인없으면 배치 함수는 종료
        if (restCoinCount == 0)
            return;

        int coinIndex = 0;
        int findLaborLevel = 0; //처음 찾는 순위
        while (true)
        {
            for (int i = 1; i < territory.Count; i++)
            {
                TokenTile tile = territory[i];
                if (tile.GetLaborCoinCount() == findLaborLevel)
                {
                    noWorkLabor[coinIndex].SendLaborCoin(tile);
                    //휴식중이던 토큰을 하나씩 투입
                    coinIndex += 1;
                    if (coinIndex == restCoinCount)
                    {
                        //채워넣은 코인의 수가 남아있던 수와 동일해지면 종료
                        return;
                    }
                }

            }
            findLaborLevel += 1; // 0짜리들이 없으면 1로 올려서 진행
            if(findLaborLevel == 4)
            {
                //모두 3개짜리 까지 찼다면 찾는거 포기 
                return;
            }
        }
    }

}
