using System;
using System.Collections.Generic;
using UnityEngine;

public enum WorkType
{
    InterBuild, ChangeBuild, ExpandLand, NationLvUp, Research, Inherence
}

public enum WorkStateCode
{
    Complete, Cancle, Stop
}

public class WorkOrder
{
    //작업 진행도

    //1. 필요한 자원
    //2. 최대 작업 토큰
    //3. 노동 토큰당 효율
    //4. 필요한 작업량
    public WorkType workType = WorkType.ChangeBuild; //어떤 타입 공사
    public int WorkPid = -1; //해당 작업에서 pid - 특정 공사인지 알기위한 부분
    public int[] WorkPlacePos; //작업장소 
    public bool DoneWrite = false;

    private List<TOrderItem> m_needList = new List<TOrderItem>();
    private List<TOrderItem> m_curList = new List<TOrderItem>();
    public int OriginWorkGauge;
    public int RestWorkGauge;
    private bool m_enoughWorkGague = false;
    public int RestTurn;
    private bool m_enoughTurn = false;

    private int m_baseworkEfficiency = 100; //토큰 1개일때 노동 효율
    private int m_overWorkEfficiency = 100; //추가되는 토큰에 따른 노동 효율

    private bool m_isComplete = false; //효과 까지 진행된경우 true
    public bool IsCancle = false;
    

    public WorkOrder(List<TOrderItem> _needList, int _needTurn, int _needWorkGague, TokenTile _placeTile, int _workPid, WorkType _workType)
    {
        if (_needList == null)
        {
            _needList = new();
        }
            

        //작업주문서 작성
        m_needList = _needList;
        OriginWorkGauge = _needWorkGague;
        RestWorkGauge = OriginWorkGauge; //임시로 필요 작업량 할당
        RestTurn = _needTurn;
        workType = _workType;
        WorkPid = _workPid;
       // string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem curItem = m_needList[i];
            curItem.Value = 0; //현재 재료 0 으로 
            m_curList.Add(curItem);
           // debugStr += m_curList[i].Tokentype + " :" + m_curList[i].Value + "필요";
        }

        //작업할 장소가 있다면 장소에 등록해보고 안되면 취소
        if(_placeTile != null)
        {
            if(_placeTile.RegisterWork(this) == false)
            {
                return;
            }
            SetOrderPlacePos(_placeTile.GetMapIndex());
        }

        GamePlayMaster.GetInstance().RegistorWork(this);
        //여기까지 진행하면
        DoneWrite = true; //잘 생성된거 - WorkOrder 반환받는곳에선 해당 여부를 판단하여 null 로 체크 
        // Debug.Log(debugStr);
    }

    #region 조건 적용
    public bool PutResource(ITradeCustomer _customer)
    {
        //전체 필요한 양을 다 넣어야함. 
        //다만 중간에 상실하는 경우도 있으니 차이만큼 진행
        List<TOrderItem> requestList = new List<TOrderItem>(); //요청 목록 제작
      //  string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량
            //자원류가 아닌것들은 투입대상 아님
            if (needItem.Tokentype != TokenType.Capital)
                continue;
            TOrderItem cur = m_curList[i]; //현재 보유량
            TOrderItem need = cur; //요구량 생성
            need.Value = needItem.Value - cur.Value; //요구 수치 차액 진행
            requestList.Add(need);
          
        }
      
        TItemListData itemListData = new TItemListData(requestList);
        if (_customer.CheckInventory(itemListData) == false)
        {
            Debug.Log("재료 부족");
            return false;
        }
      
        _customer.PayCostData(itemListData); //재료 지불 시키고

        for (int i = 0; i < m_curList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; //보유량을 필요수치로 모두 전환
            m_curList[i] = needItem;
        }

        return true;
    }

    public void NoticeNeedResource()
    {
        
        string ar = "";                                                 
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량
            if (needItem.Tokentype != TokenType.Capital)
                AdaptItemCondition(needItem, i);
            TOrderItem cur = m_curList[i]; //현재 보유량

            ar += needItem.Tokentype+" "+ cur.Value.ToString() + "/" + needItem.Value.ToString() + "\n";
        }

        Debug.Log(ar);
    }

    public void AdaptItemCondition(TOrderItem _item, int _conditionIndex)
    {
        //자원 이외의 경우 따로 조건을 체크하는 부분
        switch (_item.Tokentype)
        {
            case TokenType.NationTech:
                AdaptNationTechValue(_item, _conditionIndex);
                break;
        }
    }

    private void AdaptNationTechValue(TOrderItem _item, int _conditionIndex)
    {
        //작업 토지가 없는데 국가 테크를 조건으로 달면 애초 이상한 조건
        if (WorkPlacePos == null)
            return;

        //나라가 없는 땅이면 마찬가지로 이상한 조건
        Nation nation = GameUtil.GetTileTokenFromMap(WorkPlacePos).GetNation();
        if (nation == null)
            return;

        bool isDoneValue = nation.TechPart.IsDoneTech(_item.SubIdx);
        if(isDoneValue == true)
        {
            TOrderItem curCondtion = _item;
            curCondtion.Value = 1; //배운거면 1로 전환
            m_curList[_conditionIndex] = curCondtion; //새로운 상태로 갱신 
        }
    }
    #endregion

    #region 작업진행
    //작업량 까는거
    public void DoWork()
    {
        if(IsDoneWork())
        {
            //이미 완료된 작업
            DoEffect();
            return;
        }

        //일 시킨다
        if(CheckWorkCondtion() == false)
        {
            Debug.LogWarning("조건이 안맞아서 작업 수행 불가" + workType);
            return;
        }

        CountTurn();
        CountWorkGague();
       
        if(IsDoneWork())
           DoEffect();
    }

    //할당된 효과 발동
    public void DoEffect()
    {
        if (m_isComplete)
        {
            Debug.LogError("효과 까지 발휘된 상태 작업 이중 효과 호출 됨");
            return;
        }

        if(IsCancle == true)
        {
            return;
        }

        if(CheckEffectCondition() == false)
        {
            return;
        }

        m_isComplete = true;
        
        RemoveRegist(WorkStateCode.Complete);
        EffectByCase();


    }

    private void CountTurn()
    {
        if (m_enoughTurn == true)
            return;

        RestTurn -= 1;
        if(RestTurn <= 0)
        {
            m_enoughTurn = true;
            RestTurn = 0;
        }
    }

    private void CountWorkGague()
    {
        if (m_enoughWorkGague == true)
            return;

#if UNITY_EDITOR
        if (GamePlayMaster.GetInstance().m_cheatWorkFree)
        {
            RestWorkGauge = 0;
            m_enoughWorkGague = true;
            return;
        }
            
#endif

        int laborCoin = GameUtil.GetTileTokenFromMap(WorkPlacePos).GetLaborCoinCount();
        // Debug.Log(laborCoin + "으로 작업진행");
        if (laborCoin == 0)
        {
            Debug.LogWarning("작업자수 부족 수행 불가");

        }

        //작업량 구하기
        int workAmount = m_baseworkEfficiency * Math.Min(laborCoin, 1); //기본 토큰1은 기본효율로
        int overToken = laborCoin - 1; //초과 토큰 수 계산
        int overWorkAmount = 0;
        if (1 <= overToken)
        {
            overWorkAmount = overToken * m_overWorkEfficiency;
        }
        //작업진행
        RestWorkGauge -= (workAmount + overWorkAmount);
        //  Debug.Log("남은 작업량 " + m_restWorkGauge);
        if (RestWorkGauge <= 0)
        {
           // Debug.Log("작업완료");
            RestWorkGauge = 0;
            m_enoughWorkGague = true;
        }
    }

    private bool CheckEffectCondition()
    {
        WorkOrderExcutor excutor = new();
        return excutor.CheckCondition(this);
    }

    private void EffectByCase()
    {
        WorkOrderExcutor excutor = new();
        excutor.Excute(this);
    }

    public void Cancle()
    {
        IsCancle = true; //취소로 바꿈
        //이후 해당 작업서를 가진 곳에서 취소 체크후 작업리스트에서 제거하는 식
        RemoveRegist(WorkStateCode.Cancle);
    }

    public void RemoveRegist(WorkStateCode _code)
    {
        //해당 장소가 매턴 작업여부를 확인하지 않는다면 갱신이 안되기 때문에 제거 되어야하는 경우 작업장소로 제거할것을 요청. 
        if(WorkPlacePos != null)
        {
            TokenTile tile = GameUtil.GetTileTokenFromMap(WorkPlacePos);
            tile.SendWorkStep(_code);
        }
        GamePlayMaster.GetInstance().RemoveWork(this);
    }
    #endregion

    #region 상태 체크
    public bool CheckWorkCondtion()
    {
#if UNITY_EDITOR
        if (GamePlayMaster.GetInstance().m_cheatWorkFree)
            return true;
#endif

        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량

            //체크하려는 종류가 자원종류가 아니면 따로 적용하고 
            if (needItem.Tokentype != TokenType.Capital)
                AdaptItemCondition(needItem, i);

            TOrderItem cur = m_curList[i]; //현재 보유량
            if (cur.Value < needItem.Value)
            {
                //Debug.Log(cur.Tokentype + "이 딸려서 불가");
                return false;
            }
       
        }
        return true;
    }

    public bool IsDoneWork()
    {
        if (m_enoughTurn && m_enoughWorkGague)
            return true;

        return false;
    }

    public bool IsCompleteWork()
    {
        return m_isComplete;
    }

    #endregion

    public void SetOrderPlacePos(int[] _mapIndex)
    {
        WorkPlacePos = _mapIndex;
    }

    public List<TOrderItem> GetNeedList()
    {
        return m_needList;
    }

    public int GetWorkGauge()
    {
        return OriginWorkGauge;
    }
}
