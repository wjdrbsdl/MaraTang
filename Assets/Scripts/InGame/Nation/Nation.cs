using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum NationEnum
{
    //국가 관련 db 짤때 
    Move
}

public enum NationManageStepEnum
{
   NationEvent, IncomeCapital, ManagePopulation, SelectPolicy, CalTurnEndEffect, RemindPolicy, FinishPreTurn, FinishEndEffect
}

public enum NationStatEnum
{
     Happy, CleanFlat, CleanRatio, 성실, 안정, Sight, CommunityRange, Population, BirthRatio, TerritoryByLevel, TerritoryByPlus,
     용기, 신뢰, 자애, 건강, 근면,
     안정도, 행복도, 청결
}

public class Nation : ITradeCustomer
{
    private int m_nationNumber;
    private int m_nationLevel = 1;
    private int m_range = 1; //현재 확장된 거리
    private GodClassEnum m_godClass;
    private TokenTile m_capitalCity;
    private List<TokenTile> m_territorryList;
    public NationPopular m_popularMg; //인구 관리소
    public NationEvent m_eventMg; //사건 관리소 
    public NationStatPart m_statPart; //스텟 관리소
    public NationTechPart TechPart;
    public NationTerritoryPart TerritoryPart;
    public bool IsAlive = true;
    private int[] m_resources; //보유 자원
    private int[] nationStatValues ; //국가 스텟 - 정서, 환경 통합
    private Color[] nationColor = { Color.red, Color.yellow, Color.blue };

    private List<WorkOrder> m_workList = new(); //진행중인 작업들. 

    #region 국가 생성자
    public Nation()
    {
        m_territorryList = new();
    }

    public Nation MakeNewNation(TokenTile _capitalCity, int _nationNuber, GodClassEnum _god)
    {
        Nation nation = new();
        nation.m_godClass = _god;
        nation.m_nationNumber = _nationNuber;
        nation.SetCapitalCity(_capitalCity);
        nation.InitiStat();
        nation.AddTerritory(_capitalCity); //수도를 최초 영토로 넣고 
        //주변 1칸은 기본적으로 해당 국가 영토로 편입
        List<TokenTile> boundaryTile = GameUtil.GetTileTokenListInRange(1, _capitalCity.GetMapIndex(),1);
        for (int i = 0; i < boundaryTile.Count; i++)
        {
            nation.AddTerritory(boundaryTile[i]);
        }

        //인구파트 생성
        nation.m_popularMg = new NationPopular(nation);
        nation.m_popularMg.IncreaseLaborCoin(3); //3개 노동토큰 생성
        //스텟파트 생성
        nation.m_statPart = new NationStatPart(nation);
        //사건파트 생성
        nation.m_eventMg = new NationEvent(nation);
        //기술파트 생성
        nation.TechPart = new NationTechPart(nation);
        //자원수량 설정
        nation.m_resources = new int[GameUtil.EnumLength(Capital.Food)];
        nation.CalResourceAmount(Capital.Food, ExtraValues.g_instance.FirstSupplyMeal);
        //영지 파트 설정
        nation.TerritoryPart = new NationTerritoryPart(nation);
        return nation;
    }

    public void Destroy()
    {
        //국가 소멸시 할 작업들. 
        Debug.Log(GetNationNum() + " 국가 수도 파괴 소멸");
        IsAlive = false; //수도 파괴시 국가도 파괴 
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
        _tileToken.m_Side = SideEnum.Player;
        GamePlayMaster.GetInstance().FogContorl(_tileToken, GetStat(NationStatEnum.Sight));
    }

    public void RemoveTerritory(TokenTile _tileToken)
    {
        m_territorryList.Remove(_tileToken);
    }

    public void SetCapitalCity(TokenTile _tileToken)
    {
        m_capitalCity = _tileToken;
    }

    public void InitiStat()
    {
        nationStatValues = GameUtil.EnumLengthArray(typeof(NationStatEnum));
        SetStatValue(NationStatEnum.성실, 100);
        SetStatValue(NationStatEnum.Happy, 100);
        SetStatValue(NationStatEnum.Sight, 3);
        SetStatValue(NationStatEnum.CommunityRange, 6); //수도로부터 해당 거리에 있는 경우에 타일의 위에 있지 않아도 상호작용 가능
        SetStatValue(NationStatEnum.BirthRatio, 36);
        SetStatValue(NationStatEnum.Population, 30);
        SetStatValue(NationStatEnum.TerritoryByLevel, 6);
        SetStatValue(NationStatEnum.TerritoryByPlus, 0);
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
    public void StartNationTurn()
    {
        if(IsAlive == false)
        {
            MgNation.GetInstance().FinishNationTurn();
            return;
        }
            
        //시작은 수입정산부터
        DoJob(NationManageStepEnum.IncomeCapital);
    }

    public void SettleNationTurn()
    {
        if(IsAlive == false)
        {
            FinishTurnEndEffect();
            return;
        }
            
        DoJob(NationManageStepEnum.CalTurnEndEffect);
    }

    public void DoJob(NationManageStepEnum _step)
    {
        switch (_step)
        {
         
            //국가 턴 시작시 하는일
            case NationManageStepEnum.IncomeCapital:
               // Debug.Log("수금 진행");
                IncomeTerritoryResource();
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.ManagePopulation:
                ManagePopular();
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.RemindPolicy:
                RemindWork();
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.SelectPolicy:
                SelectPolicy();
                ReportToGameMaster(_step);
                break;
                //앞에턴 상황대로 진행될것 다 진행후, 이후 플레이어턴에 적용될 명운등을 여기서 뽑기 
            case NationManageStepEnum.NationEvent:
                m_statPart.RelateStat(); //국가 스텟에 따라서 정서수치 바꾸고
                m_eventMg.WatchEvent(); //정서 수치와 국가 스텟에 따라서 발생할 이벤트 설정
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.FinishPreTurn:
                EndNationTurn();
                break;

            //국가턴 종료후 플레이어턴 몬스터턴 진행 후 턴 정산때 하는 일 
            case NationManageStepEnum.CalTurnEndEffect:
                // Debug.Log(m_nationNumber + "정책 집행 진행");
                // 해당 턴 완료시 본래 국가에서 정책리스트를 통해 WorkOrder를 진행했지만, 해당 부분을 겜플레이마스터로 빼면서 아래 함수가 필요없어짐
                // 그래도 턴 완료 파트 구조가 필요할지 몰라서 구조는 남겨둠. 
                //DoWork(); 
                //작업서대로 진행될테고- 그대로 완성이나 효과가 적용
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.FinishEndEffect:
                FinishTurnEndEffect();
                break;
        }
    }

    public void DoneJob(NationManageStepEnum _step)
    {
        //해당 작업 완료했을때 - 일의 순서를 정의하는 곳 
        switch (_step)
        {
         
            case NationManageStepEnum.IncomeCapital:
                DoJob(NationManageStepEnum.ManagePopulation); //정책 결정한거 보고하고
                break;
            case NationManageStepEnum.ManagePopulation:
                DoJob(NationManageStepEnum.RemindPolicy);//집행한거 보고하고
                break;
            case NationManageStepEnum.RemindPolicy:
                DoJob(NationManageStepEnum.SelectPolicy);
                break;
            case NationManageStepEnum.SelectPolicy:
                DoJob(NationManageStepEnum.NationEvent);
                break;
            case NationManageStepEnum.NationEvent:
                DoJob(NationManageStepEnum.FinishPreTurn);
                break;
            case NationManageStepEnum.CalTurnEndEffect:
                DoJob(NationManageStepEnum.FinishEndEffect); //정책 결정한거 보고하고
                break;
                
        }
    }

    //겜마스터에게 보고함으로써 플레이어가 확인할 수 있는 시간 주기 
    public void ReportToGameMaster(NationManageStepEnum _step)
    {
        //플레이 마스터에서 플레이어의 확인이나 결정을 대기했다가 DoneReport로 재진행 
        GamePlayMaster.GetInstance().ReportNationStep(this, _step);
    }

    private void EndNationTurn()
    {
        MgNation.GetInstance().FinishNationTurn();
    }

    private void FinishTurnEndEffect()
    {
        MgNation.GetInstance().EndTurnEndSettle();
    }

    #endregion

    #region 정책수립
    private void SelectPolicy()
    {
        //어떤류 할지 정하고
        MainPolicyEnum mainTheme = SelectMainPolicy();
        //Announcer.Instance.AnnounceState(m_nationNumber + "국가에서 메인 정책 결정 " + m_curMainPolicy);
        FactoryPolicy factory = new();
        NationPolicy policy = factory.MakePolicy(mainTheme, this); //주요정책안으로 정책 형성
        policy.MakePlan(); //계획 세우고

        //세워진 계획이 없으면 종료 
        if (policy.m_donePlan == false)
            return;

        //작업서 쓰고
        WorkOrder workOrder = policy.WriteWorkOrder();
        if(workOrder.DoneWrite == false)
        {
            //Debug.Log("잘못 쓰인 작업서 - 해당 타일에 이미 진행중인 작업이 있음");
            return;
        }
        //기본 재료 다넣을수있는지 체크
        //재료 상관없이 작업만들수 있고, 부족분은 플레이어에게 요청 혹은 부족함을 표기함. 
        //if (workOrder.PushResource(this) == false)
        //    return;

       AddWorkorder(workOrder);
        
    }

    private MainPolicyEnum SelectMainPolicy()
    {
        //메인 정책 선택해보기 
        //0. 현재 상태 정리
        //1. 정책 타입별 우선도 체크
        //2. 군주 성향 투입
        //3. 메인 정책 Dice

        return (MainPolicyEnum)Random.Range(1, (int)MainPolicyEnum.Support);
    }

    #endregion

    #region 정책 관리
    private void AddWorkorder(WorkOrder _policy)
    {
        m_workList.Add(_policy);
    }

    private void RemoveWork(List<WorkOrder> _workList)
    {
        for (int i = 0; i < _workList.Count; i++)
        {
            RemovePolicy(_workList[i]);
        }
      //  Debug.Log(m_nationNumber + "번 국가 완료 작업 제거하고 남은 작업 수"+m_workList.Count);
    }

    private void RemovePolicy(WorkOrder _workOrder)
    {
        //RemovePolicyPin(_workOrder);
        m_workList.Remove(_workOrder);
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

    #region 작업 재검토
    private void RemindWork()
    {
        List<WorkOrder> removeList = new();
        for (int i = 0; i < m_workList.Count; i++)
        {
            WorkOrder order = m_workList[i];
            if (order.IsCompleteWork()||order.IsCancle)
            {
               // Debug.Log("완료 혹은 취소된 작업 리스트 제거추가");
                removeList.Add(order);
            }
                
        }
        RemoveWork(removeList);
    }
    #endregion

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

    public bool CheckInventory(TItemListData _costData)
    {
        List<TOrderItem> BuildCostList = _costData.GetItemList();
      
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
                    if (MGContent.GetInstance().IsContentClear(subIndx) == false)
                    {
                       // Debug.Log("해당 컨텐츠 클리어 한적 없음");
                        return false;
                    }
                    break;
                case TokenType.NationTech:
                    //Debug.Log("국가기술" + subIndx + "번을 " + value + "레벨만큼 학습했는지 확인"+IsDoneTech(subIndx));
                    if (TechPart.IsDoneTech(subIndx) == false)
                        return false;
                    break;
                default:
                  //  Debug.Log("국가적 고려 파트 아닌 부분");
                    break;
            }
        }

        return true;
    }

    public void PayCostData(TItemListData _costData, bool _isPay = true)
    {
        List<TOrderItem> BuildCostList = _costData.GetItemList();
  
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

    #region 인구 관리
    private void ManagePopular()
    {
        m_popularMg.ManagePopular(); 
    }
    #endregion
  

    public void LevelUp()
    {
        m_nationLevel += 1;
    }

    enum SuggestCode
    {
        Add, Cancle
    }

    #region 제안받기
    public void SuggestPolicyCancle(NationPolicy _policy)
    {
        Debug.Log(_policy.GetMainPolicy() + "취소 제안");
        ConsiderSuggest(_policy, SuggestCode.Cancle);
    }

    private void ConsiderSuggest(NationPolicy _policy, SuggestCode _suggestCode)
    {
        //제안 방식에 따라 정책을 평가해서 받을지 말지? 
        switch (_suggestCode)
        {
            case SuggestCode.Cancle:
                RemovePolicy(_policy.m_workOrder);
                break;
        }
    }
    #endregion

    #region GetSet

    public GodClassEnum GetGodClass()
    {
        return m_godClass;
    }

    public List<TokenTile> GetTerritorry()
    {
        return m_territorryList;
    }

    public List<TokenTile> GetPlace(TileType _type)
    {
        List<TokenTile> list = new();
        for (int i = 0; i < m_territorryList.Count; i++)
        {
            if (m_territorryList[i].GetTileType().Equals(_type))
            {
                list.Add(m_territorryList[i]);
            }
        }
        return list;
    }

    #region 스텟 배열 적용하는 부분
    public int GetStat(NationStatEnum _nationStat)
    {
        int index = (int)_nationStat;
        return nationStatValues[index];
    }
    public void SetStatValue(NationStatEnum _nationStat, int _value)
    {
        int index = (int)_nationStat;
        nationStatValues[index] = _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);

    }

    public void CalStat(NationStatEnum _nationStat, int _value)
    {
        int index = (int)_nationStat;
        nationStatValues[index] += _value;
        //Debug.Log(_nationStat + " 가" + _value + "적용");
        if (_nationStat.Equals(NationStatEnum.Happy)){
            if(nationStatValues[index] <= 0)
            {
             //   Debug.Log("행복도 마이너스 타락 진행");
            }
        }
    }
    #endregion

    public int GetNationLevel()
    {
        return m_nationLevel;
    }

    public int GetNationNum()
    {
        return m_nationNumber;
    }

    public int[] GetStatValues()
    {
        return nationStatValues;
    }
    #endregion

}
