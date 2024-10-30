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
   IncomeCapital, ManagePopulation, SelectPolicy, SettleTurnEnd, RemindPolicy, NationTurnEnd, TurnEndSettle
}

public enum NationStatEnum
{
     Happy, CleanFlat, CleanRatio, 성실, 안정
}

public class Nation : ITradeCustomer
{
    private int m_nationNumber;
    private int m_nationLevel = 1;
    private int m_range = 1; //현재 확장된 거리
    private GodClassEnum m_godClass;
    private TokenTile m_capitalCity;
    private List<TokenTile> m_territorryList;
    private int[] m_resources;
    private int[] nationStatValues ;
    public NationTechPart TechPart;
    private Color[] nationColor = { Color.red, Color.yellow, Color.blue };
    private List<NationPolicy> m_policyList = new(); //진행할 정책들이 아니라 작업리스트로 수정필요 
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
        nation.AddTerritory(_capitalCity);
        //주변 1칸은 기본적으로 해당 국가 영토로 편입
        List<TokenTile> boundaryTile = GameUtil.GetTileTokenListInRange(1, _capitalCity.GetXIndex(), _capitalCity.GetYIndex(),1);
        for (int i = 0; i < boundaryTile.Count; i++)
        {
            nation.AddTerritory(boundaryTile[i]);
        }
        nation.m_resources = new int[GameUtil.EnumLength(Capital.Food)];
        nation.InitiStat();
        nation.TechPart = new NationTechPart();
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

    public void InitiStat()
    {
        nationStatValues = GameUtil.EnumLengthArray(typeof(NationStatEnum));
        SetStatValue(NationStatEnum.성실, 100);
        SetStatValue(NationStatEnum.Happy, 100);
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
    private void EndNationTurn()
    {
        MgNation.GetInstance().EndNationTurn();
    }

    private void EndTurnEndSettle()
    {
        MgNation.GetInstance().EndTurnEndSettle();
    }

    public void ReportToGameMaster(NationManageStepEnum _step)
    {
        //플레이 마스터에서 플레이어의 확인이나 결정을 대기했다가 DoneReport로 재진행 
        GamePlayMaster.GetInstance().ReportNationStep(_step, this);
    }

    //해당 작업 완료했을때 - 다음 작업을 정해서 DoJob 시키기 
    public void DoneJob(NationManageStepEnum _step)
    {
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
                DoJob(NationManageStepEnum.NationTurnEnd);
                break;

            case NationManageStepEnum.SettleTurnEnd:
                DoJob(NationManageStepEnum.TurnEndSettle); //정책 결정한거 보고하고
                break;
                
        }
    }

    public void DoJob(NationManageStepEnum _step)
    {
        switch (_step)
        {
            case NationManageStepEnum.IncomeCapital:
                IncomeTerritoryResource();
                ReportToGameMaster(_step); 
                break;
            case NationManageStepEnum.ManagePopulation:
                ManagePopular();
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.SettleTurnEnd:
               // Debug.Log(m_nationNumber + "정책 집행 진행");
               // 해당 턴 완료시 본래 국가에서 정책리스트를 통해 WorkOrder를 진행했지만, 해당 부분을 겜플레이마스터로 빼면서 아래 함수가 필요없어짐
               // 그래도 턴 완료 파트 구조가 필요할지 몰라서 구조는 남겨둠. 
                //DoWork(); 
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.RemindPolicy:
                RemindPolicy();
                ReportToGameMaster(_step);
                break;
            case NationManageStepEnum.SelectPolicy:
                SelectPolicy();
                ReportToGameMaster(_step); 
                break;
            case NationManageStepEnum.NationTurnEnd:
                EndNationTurn(); 
                break;
            case NationManageStepEnum.TurnEndSettle:
                EndTurnEndSettle();
                break;
        }
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
        //기본 재료 다넣을수있는지 체크
        if (workOrder.PushResource(this) == false)
            return;

        m_workList.Add(workOrder);
        
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


    #region 작업 재검토
    private void RemindPolicy()
    {
        List<NationPolicy> removeList = new();
        for (int i = 0; i < m_policyList.Count; i++)
        {
            NationPolicy policy = m_policyList[i];
            if (policy.IsDone())
            {
                Debug.Log("완료된 작업 리스트 추가");
                removeList.Add(policy);
            }
                
        }
        RemovePolicy(removeList);
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
                    if (MGContent.GetInstance().IsContentDone(subIndx) == false)
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
    public void DeadPeople(int _count)
    {
        CalResourceAmount(Capital.Person, -_count);
    }

    private void ManagePopular()
    {
        //1. 인구 관련 클래스에서 경영 진행
        NationPopular popManager = new NationPopular(this);
        popManager.ManagePopular(); 
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
                RemovePolicy(_policy);
                break;
        }
    }
    #endregion

    #region GetSet

    public GodClassEnum GetGodClass()
    {
        return m_godClass;
    }

    #region 스텟 배열 적용하는 부분
    public int GetStat(NationStatEnum _nationStat, int _value)
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

    public List<NationPolicy> GetNationPolicyList()
    {
        return m_policyList;
    }


    public int GetNationLevel()
    {
        return m_nationLevel;
    }

    public int GetNationNum()
    {
        return m_nationNumber;
    }
    #endregion

}
