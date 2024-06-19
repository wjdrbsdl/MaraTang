using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MainPolicy
{
    None, NationLevelUP, ExpandLand, ManageLand, TechTree, Support
}

public class Nation : ITradeCustomer
{
    private int m_nationNumber;
    private int m_nationLevel = 1;
    private int m_range = 1; //현재 확장된 거리
    private TokenTile m_capitalCity;
    private List<TokenTile> m_territorryList;
    private int[] m_resources;
    private List<int> m_doneTech; // 완료한 테크 Pid
    static Color[] nationColor = { Color.red, Color.yellow, Color.blue };
    private MainPolicy m_curMainPolicy = MainPolicy.None; //현재 정책 상황
    private int m_holdPolicyCount = 0; //현재 정책 유지된 턴
    private TokenBase m_planToken;
    private int m_planIndex = FixedValue.No_INDEX_NUMBER;  //메인 정책당 구체적인 계획의 인덱스

    #region 국가 생성자
    public Nation()
    {
        m_territorryList = new();
    }

    public Nation MakeNewNation(TokenTile _capitalCity, int _nationNuber)
    {
        Nation nation = new();
        nation.m_nationNumber = _nationNuber;
        nation.SetCapitalCity(_capitalCity);
        nation.AddTerritory(_capitalCity);
        //주변 1칸은 기본적으로 해당 국가 영토로 편입
        List<TokenTile> boundaryTile = GameUtil.GetTileTokenListInRange(1, _capitalCity.GetXIndex(), _capitalCity.GetYIndex(),1);
        for (int i = 0; i < boundaryTile.Count; i++)
        {
            nation.AddTerritory(boundaryTile[i]);
        }
        nation.m_resources = new int[GameUtil.EnumLength(Capital.Food)];
        nation.m_doneTech = new List<int>(); //작업 완료 테크
        return nation;
    }

    public void Destroy()
    {
        //국가 소멸시 할 작업들. 
    }
    #endregion

    #region 국가 영토 관리
    public void AddTerritory(TokenTile _tileToken)
    {
        //이미 있는 영토
        if(m_territorryList.IndexOf(_tileToken)>= 0)
                return;

        m_territorryList.Add(_tileToken);
        _tileToken.SetNation(m_nationNumber);
    }

    public void RemoveTerritory(TokenTile _tileToken)
    {
        m_territorryList.Remove(_tileToken);
    }

    public void SetCapitalCity(TokenTile _tileToken)
    {
        m_capitalCity = _tileToken;
    }

    public TokenTile GetCapital()
    {
        return m_capitalCity;
    }

    public void ShowTerritory()
    {
        for (int i = 1; i < m_territorryList.Count; i++)
        {
            TokenTile tile = m_territorryList[i];
            tile.Dye(nationColor[m_nationNumber]);
            //   Debug.Log("해당 타일의 타입은 " + tile.GetTileType());
        }
    }

    #endregion

    public void ManageNation()
    {
        //국가운영 
        IncomeTerritoryResource(); //영토에서 자원 수급
        SelectPolicy(); //정책 수립
        ExcutePolicy(); //정책 수행
        RemindPolicy();
    }

    #region 정책수립
    private void SelectPolicy()
    {
        //정책 정함
        //기존 정책이 있으면 회의 종료
        if (HavePolicy())
        {
            m_holdPolicyCount += 1;
         //   Debug.Log("기존 정책 " + m_curMainPolicy + " 유지" + m_mainPolicyCount);
            return;
        }
            

        //아니면 여기서 정함 일단 랜덤
        int randomPolicy = Random.Range(1, (int)MainPolicy.Support);

        //어떤류 할지 정하고
        m_curMainPolicy = (MainPolicy)randomPolicy;
        Announcer.Instance.AnnounceState(m_nationNumber + "국가에서 메인 정책 결정 " + m_curMainPolicy);

        //그에 대한 타겟을 정한다
        MakePlan(m_curMainPolicy);
        
    }

    private bool HavePolicy()
    {
        return m_curMainPolicy.Equals(MainPolicy.None) == false;
    }

    private bool HavePlanToken()
    {
        return !(m_planToken == null && m_planIndex == FixedValue.No_INDEX_NUMBER);
    }

    private void MakePlan(MainPolicy _mainPolicy)
    {
        switch (_mainPolicy)
        {
            case MainPolicy.ExpandLand:
                FindExpandLand();
                break;
            case MainPolicy.ManageLand:
                FindManageLand();
                break;
            case MainPolicy.NationLevelUP:
                m_planToken = GetCapital();
                break;
            case MainPolicy.TechTree:
                SelectTechTree();
                break;
        }
    }

  
    private void FindExpandLand()
    {
        int findExpandCount = 1; //3개씩 확장하는걸로 
        int startRange = m_range; //시작할 위치 현재 뻗어간 영토 거리
        for (int i = startRange; i <= m_nationLevel; i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_capitalCity.GetXIndex(), m_capitalCity.GetYIndex(), i);
            //2. 무소속이면 대상 토지를 편입. 

            m_range = i; //현재 번창된 사거리 갱신 
            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
                if (tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER))
                {
                    m_planToken = tile; //무소속이면 해당 타일 편입
                    findExpandCount -= 1;
                    if (findExpandCount.Equals(0))
                    {
                        break;
                    }
                }
            }

            if (findExpandCount.Equals(0))
            {
                break;
            }
        }

        if (m_planToken == null)
        {
            ResetPolicy();
           // Debug.Log("확장할 영토를 찾지 못해 정책 초기화");
        }
            
    }

    private void FindManageLand()
    {
        int endRange = m_range; //최종 살펴볼 위치
        for (int i = 1; i <= endRange; i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_capitalCity.GetXIndex(), m_capitalCity.GetYIndex(), i);
            //2. 무소속이면 대상 토지를 편입. 

            m_range = i; //현재 번창된 사거리 갱신 
            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
                //만약 내땅이 아니면 일단 패스 
                if (tile.GetStat(TileStat.Nation).Equals(m_nationNumber) == false)
                    continue;

                //용도가 노말이 아니면 패스 
                if (tile.GetTileType().Equals(TileType.Nomal) == false)
                    continue;

                //내땅중에 용도가 nomal인 땅을 찾았다면
                m_planToken = tile;
                m_planIndex = Random.Range((int)TileType.WoodLand, (int)TileType.Capital); //벌목장부터 광산까지 중 뽑기 
                return;
            }

        }
        if (m_planToken == null)
        {
            ResetPolicy();
          //  Debug.Log("운영할 영토를 찾지 못해 정책 초기화");
        }
    }

    private void SelectTechTree()
    {
        //다음 연구할 기술을 선택. 
        TechTreeSelector treeManager = new(); //매니저 생성하고 
        m_planIndex = treeManager.GetTechPidByNotDone(m_doneTech);
       // Debug.Log("다음 연구 테크pid는" + m_planIndex + "로 결정");
    }

    #endregion

    #region 정책 수행
    private void ExcutePolicy()
    {
        //수행할 정책 없으면 종료
        if (HavePolicy() == false)
        {
          //  Debug.Log("수립된 정책 없음");
            return;
        }
            
        //수행할 계획 없으면 종료
        if (HavePlanToken() == false)
        {
           // Debug.Log("구체적 계획 없음");
            return;
        }

        DoPlan();
    }

    private void DoPlan()
    {
        switch (m_curMainPolicy)
        {
            case MainPolicy.ExpandLand:
                ExpandTerritory();
                break;
            case MainPolicy.ManageLand:
                ManageTerritory();
                break;
            case MainPolicy.NationLevelUP:
                LevelUp();
                break;
            case MainPolicy.TechTree:
                Research();
                break;
        }
    }

    private void ExpandTerritory()
    {
        TokenTile planTile = (TokenTile)m_planToken;
        if (AbleExpand(planTile) == false)
        {
          //  Debug.Log("확장 불가능 상태");
            return;
        }

        //확장 가능한 상태라면
      //  Debug.Log("영토 확장 정책 수행 완료");
        CalResourceAmount(Capital.Wood, -300);
        AddTerritory(planTile); //계획 토큰을 타일로 전환후 영토 집행
        ShowTerritory();
        ResetPolicy();
    }

    private void ManageTerritory()
    {
        TokenTile planTile = (TokenTile)m_planToken;
        if (AbleManageLand(planTile) == false)
        {
         //   Debug.Log("운영 불가능 상태");
            return;
        }

        //변경 가능한 상태라면
        //  Debug.Log("영토 운영 정책 수행 완료");
        OrderCostData changeCost = MgMasterData.GetInstance().GetTileData(m_planIndex).BuildCostData;
        PayCostData(changeCost);
        planTile.ChangeTileType((TileType)m_planIndex); //플랜 idx 타입으로 토지변경
        ResetPolicy();
    }
    
    private void LevelUp()
    {
        if(AbleLevelUp() == false)
        { 
         //   Debug.Log("레벨업 조건 미충족");
            return;
        }

        Announcer.Instance.AnnounceState("국가 레벨 상승 :" + m_nationLevel+"Lv");
        int needPerson = m_nationLevel * 40;
        int needFood = needPerson;
        CalResourceAmount(Capital.Food, -needFood);
        m_nationLevel += 1;
        ResetPolicy();
    }

    private void Research()
    {
        if(AbleResearch(m_planIndex) == false)
        {
            return;
        }

        Debug.Log(m_planIndex + "번 테크 연구 완료");
        //1.비용내고
        OrderCostData costData = MgMasterData.GetInstance().GetTechData(m_planIndex).ResearchCostData;
        PayCostData(costData);
        //2.완료 기록하고
        CompleteTech(m_planIndex);
        //3.정책 초기화
        ResetPolicy();
    }



    private bool AbleExpand(TokenTile _tile)
    {
        //만약 현재 타일상태가 누군가의 점유로 바꼈으면 확장 불가 
        if (_tile.GetStat(TileStat.Nation) != FixedValue.NO_NATION_NUMBER)
        {
            //   Debug.Log("미점유상태 타일이 아님");
            return false;
        }


        if (GetResourceAmount(Capital.Wood) < 300)
        {
            //  Debug.Log("확장 비용 부족");
            return false;
        }


        return true;
    }

    private bool AbleManageLand(TokenTile _tile)
    {
        //만약 현재 타일상태가 누군가의 점유로 바꼈으면 확장 불가 
        if (_tile.GetStat(TileStat.Nation) != m_nationNumber)
        {
            //  Debug.Log("국가 귀속 타일이 아님");
            return false;
        }
        if (_tile.GetTileType() != TileType.Nomal)
        {
            //  Debug.Log("토지 변경 불가능한 상태");
            return false;
        }
        OrderCostData changeCost = MgMasterData.GetInstance().GetTileData(m_planIndex).BuildCostData;
        //  Debug.Log((TileType)m_planIndex + "로 변경하려는 중");
        if (CheckInventory(changeCost) == false)
        {
            // Debug.Log("국가 단위 변경 비용 부족");
            return false;
        }

        return true;
    }

    private bool AbleLevelUp()
    {
        int needPerson = m_nationLevel * 40;
        int needFood = needPerson;
        if (GetResourceAmount(Capital.Person) < needPerson)
        {
         //   Debug.Log("등급상승 만족 인구 부족");
            return false;
        }

        if (GetResourceAmount(Capital.Food) < needFood)
        {
          //  Debug.Log("등급상승 식량 부족");
            return false;
        }
        return true;
    }

    private bool AbleResearch(int _techPid)
    {
        OrderCostData costData = MgMasterData.GetInstance().GetTechData(_techPid).ResearchCostData;

        return CheckInventory(costData);
    }

    #endregion

    #region 정책 초기화
    private void ResetPolicy()
    {
        m_curMainPolicy = MainPolicy.None;
        m_planToken = null;
        m_planIndex = FixedValue.No_INDEX_NUMBER;
        m_holdPolicyCount = 0;
    }
    #endregion

    private void RemindPolicy()
    {
        //집행되지 못한 정책의 경우 바꿀지 말지
        if(m_holdPolicyCount >= 3)
        {
          //  Debug.Log("정책 유지 3회 이유로 기존 정책 초기화");
            ResetPolicy();
        }
    }

    #region 국가 자산 관리

    private void IncomeTerritoryResource()
    {
        //영토 자원 수집
        for (int i = 0; i < m_territorryList.Count; i++)
        {
            TokenTile tile = m_territorryList[i];
            RuleBook.TMineTileResult mineResult = GamePlayMaster.GetInstance().RuleBook.MineResource(tile);
            CalMineResult(mineResult);
        }
    }

    private void CalMineResult(RuleBook.TMineTileResult _mineResult)
    {
        //채집 결과물을 가지고 자원 획득
        List<(Capital, int)> mineResult = _mineResult.GetResourceAmount();
        for (int i = 0; i < mineResult.Count; i++)
        {
            CalResourceAmount(mineResult[i].Item1, mineResult[i].Item2);
        }
    }

    public int GetResourceAmount(Capital _capital)
    {
        return m_resources[(int)_capital];
    }

    public void CalResourceAmount(Capital _capital, int _value)
    {
        m_resources[(int)_capital] += _value;
    }

    public bool CheckInventory(OrderCostData _costData)
    {
        List<TOrderItem> BuildCostList = _costData.GetCostList();
      
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            TokenType costType = BuildCostList[i].MainIdx;
            int subIndx = BuildCostList[i].SubIdx;
            int value = BuildCostList[i].Value;
            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                   //Debug.LogFormat("{0}자원 보유: {1}, 요구:{2}", (Capital)BuildCostList[i].SubIdx, GetResourceAmount((Capital)BuildCostList[i].SubIdx), BuildCostList[i].Value);
                    if (GetResourceAmount((Capital)subIndx) < value == true)
                    {
                       return false;
                    }
                    break;
                case TokenType.Content:
                    //Debug.Log("컨텐츠" + subIndx + "번을 " + value + "만큼 클리어했는지 확인");
                    if (MGContent.GetInstance().IsContentDone(subIndx) == false)
                    {
                       // Debug.Log("해당 컨텐츠 클리어 한적 없음");
                        return false;
                    }
                    break;
                case TokenType.NationTech:
                    //Debug.Log("국가기술" + subIndx + "번을 " + value + "레벨만큼 학습했는지 확인"+IsDoneTech(subIndx));
                    if (IsDoneTech(subIndx) == false)
                        return false;
                    break;
                default:
                  //  Debug.Log("국가적 고려 파트 아닌 부분");
                    break;
            }
        }

        return true;
    }

    public void PayCostData(OrderCostData _costData)
    {
        List<TOrderItem> BuildCostList = _costData.GetCostList();
  
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            TokenType costType = BuildCostList[i].MainIdx;
            int subIdx = BuildCostList[i].SubIdx;
            int value = BuildCostList[i].Value;
            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                   // Debug.LogFormat("{0}자원 지불: {1}", (Capital)subIdx, -value);
                    CalResourceAmount((Capital)subIdx, -value);
                    break;
                default:
                  //  Debug.Log("국가적 고려 파트 아닌 부분");
                    break;
            }
        }
    }
    #endregion

    #region 국가 테크 
    public void CompleteTech(int _techPid)
    {
        if(IsDoneTech(_techPid) == false) //배우지 않은 녀석이면
            m_doneTech.Add(_techPid);
    }

    public List<int> GetDoneTech()
    {
        return m_doneTech;
    }

    public bool IsDoneTech(int _techPid)
    {
        if (m_doneTech.IndexOf(_techPid) >= 0)
            return true;

        return false;
    }

 
    #endregion
}
