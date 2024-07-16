using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum MainPolicy
{
    None, NationLevelUP, ExpandLand, ManageLand, TechTree, Support
}

public enum NationManageStep
{
   ManageStart, Income, SelectPolicy, ExcutePolicy, RemindPolicy
}

public enum NationStat
{
     Happy
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
    private Color[] nationColor = { Color.red, Color.yellow, Color.blue };
    private List<NationPolicy> m_policyList = new(); //진행할 정책들

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

    #region 국가 행동 진행
    private void EndTurn()
    {
        MgNation.GetInstance().EndNationTurn();
    }

    public void ReportToGameMaster(NationManageStep _step)
    {
        //플레이 마스터에서 플레이어의 확인이나 결정을 대기했다가 DoneReport로 재진행 
        GamePlayMaster.GetInstance().ReportNationStep(_step, this);
    }

    public void DoneReport(NationManageStep _step)
    {
        switch (_step)
        {
            case NationManageStep.ManageStart:
              //  Debug.Log(m_nationNumber + "번 국가 수입정산 진행");
                IncomeTerritoryResource();
                DoneReport(NationManageStep.Income); //보고필요가 없는건 바로 다음 단계 보고 끝난걸로 진행 
                break;
            case NationManageStep.Income:
              //  Debug.Log(m_nationNumber + "번 결정하고 보고");
                SelectPolicy(); //정책결정하고
                ReportToGameMaster(NationManageStep.SelectPolicy); //정책 결정한거 보고하고
                break;
            case NationManageStep.SelectPolicy:
             //   Debug.Log(m_nationNumber + "번 집행하고 보고");
                ExcutePolicy(); //집행하고
                ReportToGameMaster(NationManageStep.ExcutePolicy); //집행한거 보고하고
                break;
            case NationManageStep.ExcutePolicy:
             //   Debug.Log(m_nationNumber + "번 상기하고 턴종료");
                RemindPolicy(); //상기하고
                EndTurn(); //턴종료
                break;
        }
    }
    #endregion

    #region 정책수립
    private void SelectPolicy()
    {
        //정책 정함
     
        int randomPolicy = Random.Range(1, (int)MainPolicy.Support);

        //어떤류 할지 정하고
        MainPolicy mainTheme = (MainPolicy)randomPolicy;
        //Announcer.Instance.AnnounceState(m_nationNumber + "국가에서 메인 정책 결정 " + m_curMainPolicy);
        NationPolicy policy = new NationPolicy(mainTheme, m_nationNumber); //주요정책안으로 정책 형성
        //그에 대한 타겟을 정한다
        //MakePlan(m_curMainPolicy);
        MakePlan(policy); //세부 계획 수립
    }

    private void MakePlan(NationPolicy _policy)
    {
        MainPolicy mainPolicy = _policy.GetMainPolicy();
        switch (mainPolicy)
        {
            case MainPolicy.ExpandLand:
                FindExpandLand(_policy);
                break;
            case MainPolicy.ManageLand:
                FindManageLand(_policy);
                break;
            case MainPolicy.NationLevelUP:
                TokenBase planToken = GetCapital();
                _policy.SetPlanToken(planToken);
                break;
            case MainPolicy.TechTree:
                SelectTechTree(_policy);
                break;
        }
        //정책 세부 계획 수립후
        if(_policy.GetPlanIndex() != FixedValue.No_INDEX_NUMBER || _policy.GetPlanToken() != null)
        {
            //계획이 설정되었으면
            AddPolicy(_policy);
            ShowPolicyPin(_policy);
        }
    }

    private void FindExpandLand(NationPolicy _policy)
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
                //무국적 땅이 아니면 넘김
                if (tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER) == false)
                    continue;

                //정책 대상이면 넘김
                if (tile.GetPolicy() != null)
                    continue;

                TokenBase planToken = tile; //확장가능한 땅이면 타겟 지정
                _policy.SetPlanToken(planToken); //정책 대상으로 넣고
                tile.SetPolicy(_policy);
                findExpandCount -= 1;
                if (findExpandCount.Equals(0))
                {
                    break;
                }

            }

            if (findExpandCount.Equals(0))
            {
                break;
            }
        }

    }

    private void FindManageLand(NationPolicy _policy)
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

                //이미 정책 대상 토지면 패스
                if (tile.GetPolicy() != null)
                    continue;

                //용도가 노말이 아니면 패스 
                if (tile.GetTileType().Equals(TileType.Nomal) == false)
                    continue;

                //내땅중에 용도가 nomal인 땅을 찾았다면
                TokenBase planToken = tile;
                int planIndex = Random.Range((int)TileType.WoodLand, (int)TileType.Capital); //벌목장부터 광산까지 중 뽑기 
                _policy.SetPlanToken(planToken);
                _policy.SetPlanIndex(planIndex);
                tile.SetPolicy(_policy);
                return;
            }

        }

    }

    private void SelectTechTree(NationPolicy _policy)
    {
        //다음 연구할 기술을 선택. 
        TechTreeSelector treeManager = new(); //매니저 생성하고 
        int planIndex = treeManager.GetTechPidByNotDone(m_doneTech);
        _policy.SetPlanIndex(planIndex);
        // Debug.Log("다음 연구 테크pid는" + m_planIndex + "로 결정");
    }
    #endregion

    #region 정책 추가 제거
    private void AddPolicy(NationPolicy _policy)
    {
        m_policyList.Add(_policy);
    }

    private void RemovePolicy(List<NationPolicy> _ploicyList)
    {
        for (int i = 0; i < _ploicyList.Count; i++)
        {
            RemovePolicy(_ploicyList[i]);
        }
    }

    private void RemovePolicy(NationPolicy _policy)
    {
        _policy.Reset(); //정책 내부적으로 리셋 - tokenBase에 할당된 policy null등 진행 
        RemovePolicyPin(_policy);
        m_policyList.Remove(_policy);
    }
    #endregion

    #region 정책 표기 핀
    private void ShowPolicyPin(NationPolicy _policy)
    {
        MgNaviPin.GetInstance().ShowPolicyPin(_policy);   
    }

    private void RemovePolicyPin(NationPolicy _policy)
    {
        MgNaviPin.GetInstance().RemovePolicyPinList(_policy);
    }
    #endregion

    #region 정책 수행
    private void ExcutePolicy()
    {
        for (int i = 0; i < m_policyList.Count; i++)
        {
            NationPolicy policy = m_policyList[i];
            DoPlan(policy);
        }
    }

    private void DoPlan(NationPolicy _policy)
    {
        TokenBase planToken = _policy.GetPlanToken();
        int planIndex = _policy.GetPlanIndex();
        bool isComplete = false;
        MainPolicy policy = _policy.GetMainPolicy();
        switch (policy)
        {
            case MainPolicy.ExpandLand:
                isComplete = ExpandTerritory(planToken);
                break;
            case MainPolicy.ManageLand:
                isComplete = ManageTerritory(planToken, planIndex);
                break;
            case MainPolicy.NationLevelUP:
                isComplete = LevelUp();
                break;
            case MainPolicy.TechTree:
                isComplete = Research(planIndex);
                break;
        }
        if (isComplete)
        {
            _policy.Done();
        }
    }

    private bool ExpandTerritory(TokenBase _planToken)
    {
        TokenTile planTile = (TokenTile)_planToken;
        if (AbleExpand(planTile) == false)
        {
          //  Debug.Log("확장 불가능 상태");
            return false;
        }

        //확장 가능한 상태라면
        //Debug.Log("영토 확장 정책 수행 완료");
        CalResourceAmount(Capital.Wood, -300);
        AddTerritory(planTile); //계획 토큰을 타일로 전환후 영토 집행
        ShowTerritory();
        return true;
    }

    private bool ManageTerritory(TokenBase _planToken, int _planIndex)
    {
        TokenTile planTile = (TokenTile)_planToken;
        if (AbleManageLand(planTile, _planIndex, m_nationNumber) == false)
        {
         //   Debug.Log("운영 불가능 상태");
            return false;
        }

        //변경 가능한 상태라면
        //  Debug.Log("영토 운영 정책 수행 완료");
        OrderCostData changeCost = MgMasterData.GetInstance().GetTileData(_planIndex).BuildCostData;
        PayCostData(changeCost);
        planTile.ChangeTileType((TileType)_planIndex); //플랜 idx 타입으로 토지변경
        return true;
    }
    
    private bool LevelUp()
    {
        if(AbleLevelUp() == false)
        { 
         //   Debug.Log("레벨업 조건 미충족");
            return false;
        }

        Announcer.Instance.AnnounceState("국가 레벨 상승 :" + m_nationLevel+"Lv");
        int needPerson = m_nationLevel * 40;
        int needFood = needPerson;
        CalResourceAmount(Capital.Food, -needFood);
        m_nationLevel += 1;
        return true;
    }

    private bool Research(int _planIndex)
    {
        if(AbleResearch(_planIndex) == false)
        {
            return false;
        }

     //   Debug.Log(m_planIndex + "번 테크 연구 완료");
        //1.비용내고
        OrderCostData costData = MgMasterData.GetInstance().GetTechData(_planIndex).ResearchCostData;
        PayCostData(costData);
        //2.완료 기록하고
        CompleteTech(_planIndex);
        
        return true;
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

    private bool AbleManageLand(TokenTile _tile, int _nationNumber, int _planIndex)
    {
        //만약 현재 타일상태가 누군가의 점유로 바꼈으면 확장 불가 
        if (_tile.GetStat(TileStat.Nation) != _nationNumber)
        {
            //  Debug.Log("국가 귀속 타일이 아님");
            return false;
        }
        if (_tile.GetTileType() != TileType.Nomal)
        {
            //  Debug.Log("토지 변경 불가능한 상태");
            return false;
        }
        OrderCostData changeCost = MgMasterData.GetInstance().GetTileData(_planIndex).BuildCostData;
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
        //마스터데이터에 없는 테크번호면 실패
        if(MgMasterData.GetInstance().GetTechData(_techPid) == null)
        {
            return false;
        }
        OrderCostData costData = MgMasterData.GetInstance().GetTechData(_techPid).ResearchCostData;
        return CheckInventory(costData);
    }

    #endregion

    private void RemindPolicy()
    {
        List<NationPolicy> removeList = new();
        for (int i = 0; i < m_policyList.Count; i++)
        {
            NationPolicy policy = m_policyList[i];
            if (policy.IsDone())
            {
                removeList.Add(policy);
            }
                
        }
        RemovePolicy(removeList);
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
            TokenType costType = BuildCostList[i].Tokentype;
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

    public void PayCostData(OrderCostData _costData, bool _isPay = true)
    {
        List<TOrderItem> BuildCostList = _costData.GetCostList();
  
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            TokenType costType = BuildCostList[i].Tokentype;
            int subIdx = BuildCostList[i].SubIdx;
            int value = -BuildCostList[i].Value;
            if (_isPay == false)
                value *= -1;
            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                   // Debug.LogFormat("{0}자원 지불: {1}", (Capital)subIdx, -value);
                    CalResourceAmount((Capital)subIdx, value);
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

    public List<NationPolicy> GetNationPolicyList()
    {
        return m_policyList;
    }

    #region 제안받기
    public void SuggestPolicyCancle(NationPolicy _policy)
    {
        Debug.Log(_policy.GetMainPolicy() + "취소 제안");
        RemovePolicy(_policy);
    }
    #endregion
}
