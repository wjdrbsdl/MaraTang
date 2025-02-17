﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NationPopular
{
    private Nation m_nation;
    private int m_mealRatio = 2; //필요 식량 비율 - 식량 효율이 좋을수록 값 감소
    private int m_laborRatio = 10; //인구 몇당 노동코인 수 
    private int hungPeople = 0;
    private List<LaborCoin> m_LaborCoinList; //노동코인 
    /*
    1.  인구 증가 천장 - 수도 최적도 수치까지
        인구 증감 적용 함수는 BirthPeople, 과 DiePeople 
         - 각 적용 과정에서 변경된 인구수에 따라 노동코인 수도 갱신 
        
    2.  인구 최적도 - > 수도를 제외한 곳엔 각 장소마다 최대 노동코인이 머물 수 있으며, 기본 3개를 할당할 수 있고 노동코인은 인구 n당이므로 
        노동코인 배치 최대 수 * 노동코인 1당 인구 수 * 영역 토지 수
        수도에는 얼마든지 인구가 머물 수 있으며, 단 최적도는 수도의 벨류에 따른다. 최적도를 넘어서면 수도에서 민원이 발생할 수 있다. 

    3.  인구 증가 공식 
        1. 잉여 식량있을 경우, 현재 인구의 n%로 발생 // 그 수가 최적도 남은 숫자와 작은 수 
        2. 최소 증가 수치 존재 - 1
        3. n% 비율은 국가 스텟이나 버프등으로 변화할 수 있음. 
        
    4.  인구 감소 공식
        1. 식량을 못 먹은 인구 중 행복도, 안정도등 그밖에 국민 스텟으로 생존률 비율로 사망
        2. 적용 스텟 비율은 여러 버프등으로 변화할 수 있음. 
        
    5. 노동 코인 단위  - 10 인구당 1개 
        
    */

    public NationPopular(Nation _nation)
    {
        m_nation = _nation;
        m_LaborCoinList = new();
    }

    public void ManagePopular()
    {
        //1. 현재 인구수 만큼 식량 소비 진행 
        EatMeal();
        //2. 인구증감 진행
        IncreasePopular();
        //3. 남은 노동 토큰 배치
        PlaceLaborCoin();
    }

    public void IncreaseLaborCoin(int _count)
    {
       // Debug.Log(_count + "만큼 노동 코인 증가");
        for (int i = 0; i < _count; i++)
        {
            int curLaborCount = m_LaborCoinList.Count; //리스트에서 순번이 곧 pid
            LaborCoin newLabor = new LaborCoin(curLaborCount, m_nation);
           // Debug.Log(m_nation.GetNationNum() + "국가에 " + newLabor.ListIndex + "노동 코인 생성");
            m_LaborCoinList.Add(newLabor);
        }
    }

    public void DecreaseLaborCoin(int _count)
    {
      //  Debug.Log(_count + "만큼 노동 코인 감소");
        int curLaborCount = m_LaborCoinList.Count;
        int count = Mathf.Min(_count, curLaborCount);
        for (int i = 0; i < count; i++)
        {
            LaborCoin labor = m_LaborCoinList[0];
            labor.DieLaborCoin(); //복귀 시키고
            m_LaborCoinList.RemoveAt(0); //제거
        }
      //  Debug.Log("남은 코인" + m_LaborCoinList.Count);
    }

    #region 식량 먹이기
    private void EatMeal()
    {
        int population = m_nation.GetStat(NationStatEnum.Population);
        int meal = m_nation.GetResourceAmount(Capital.Food);
        int ablePeople = AbleEatPopulation();
        hungPeople = 0; //굶는사람 0
        if (ablePeople < population)
        {
            //식량이 부족하면 먹일수 있는 식량 만큼 식량 감소
            int ableMeal = ablePeople * m_mealRatio;
            m_nation.CalResourceAmount(Capital.Food, -ableMeal);
          //  Debug.Log("기존 식량" + meal +"식후 식량"+m_nation.GetResourceAmount(Capital.Food));
            hungPeople = population - ablePeople; //먹지 못한 인간들
         //   Debug.Log(hungPeople + "인구가 먹지 못함");
        
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
    #endregion 

    private void IncreasePopular()
    {
        if (hungPeople >= 1)
        {
            AdaptHungryPenalty();
        }
        else
        {
            AdaptBreed();
        }
    }

    private void AdaptHungryPenalty()
    {
        //부족한 식량에 따른 수치에 따라 죽을사람 산정
        //부정으로 행복도가 마이너스 되는경우, 죽어야할 사람을 넘어서 더죽음. 
        int happy = m_nation.GetStat(NationStatEnum.Happy);
        if (happy < 0)
            happy = 0;

        decimal power = happy * 0.001m;
        int livePeople =(int)(hungPeople * power);
      //  Debug.LogFormat("생존비율{0}사망 예정인구{1}버티기 인구{2}", power, hungPeople, livePeople);
        DeadPeople(hungPeople - livePeople);
    }

    private void AdaptBreed()
    {
        //여유 식량과 다른 공식을 이용해서 증가할 수치를 산정
        int optimalCount = GetOptimalPopulation(); //최적수치
        int curPeople = m_nation.GetStat(NationStatEnum.Population);
        int gap = optimalCount - curPeople;

        //만약 최적도를 넘어선 수치라면 인구 증가 안함
        if (gap <= 0)
            return;

        decimal increseRatio = m_nation.GetStat(NationStatEnum.BirthRatio) * 0.001m;
        int birthCount = (int)(curPeople * increseRatio);
        if (birthCount <= 0)
            birthCount = 1;

        birthCount = Mathf.Min(birthCount, gap); //최적도까지 남은수와 예상 탄생수 중 낮은값으로 진행 
      //  Debug.LogFormat("증감비율{0}현재 인구{1} 증가 인구{2}", increseRatio, curPeople, birthCount);
        BirthPeople(birthCount);
    }

    private void BirthPeople(int _birthCount)
    {
        int pre = m_nation.GetStat(NationStatEnum.Population);
        int final = pre + _birthCount; //증가된 인구 계산
        m_nation.SetStatValue(NationStatEnum.Population, final); //벨류 넣기

        //노동 코인 증감 계산
        int laborCoin = final / m_laborRatio; //100으로 나눴을때의 노동 
        int preLaborCoin = pre / m_laborRatio; //기존 발생가능했던 코인수 //인구와 별개로 노동코인이 삽입될수있기 때문에 인구증감에 따라서만 계산
       // Debug.LogFormat("{0}국가 탄생 후 남은 인구{1} ", m_nation.GetNationNum(), m_nation.GetStat(NationStatEnum.Population));
        IncreaseLaborCoin(laborCoin - preLaborCoin);
    }

    private void DeadPeople(int _deathCount)
    {
        int pre = m_nation.GetStat(NationStatEnum.Population);
        int final = pre - _deathCount; //증가된 인구 계산
        m_nation.SetStatValue(NationStatEnum.Population, final); //벨류 넣기
      //  Debug.LogFormat("{0}국가 사망 후 남은 인구{1} ", m_nation.GetNationNum(), m_nation.GetStat(NationStatEnum.Population));
        //노동 코인 증감 계산
        int laborCoin = final / m_laborRatio; //10으로 나눴을때의 노동 
        int preLaborCoin = pre / m_laborRatio; //기존 발생가능했던 코인수 //인구와 별개로 노동코인이 삽입될수있기 때문에 인구증감에 따라서만 계산
        DecreaseLaborCoin(preLaborCoin-laborCoin);
    }

    private void PlaceLaborCoin()
    {
        List<LaborCoin> noWorkLabor = new();
        for (int i = 0; i < m_LaborCoinList.Count; i++)
        {
            LaborCoin labor = m_LaborCoinList[i];
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

    private int GetOptimalPopulation()
    {
        //최적인구수 일단 보유 토지 *3 으로 정의
        int optimalCount = m_nation.GetTerritorry().Count * 3 * m_laborRatio;
        return optimalCount;
    }
}
