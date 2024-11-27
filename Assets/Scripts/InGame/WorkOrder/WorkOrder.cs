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
    private int m_originWorkGauge;
    private int m_restWorkGauge;
    
    private int m_baseworkEfficiency = 100; //토큰 1개일때 노동 효율
    private int m_overWorkEfficiency = 10; //추가되는 토큰에 따른 노동 효율

    private bool m_isComplete = false; //효과 까지 진행된경우 true
    public bool IsCancle = false;
    

    public WorkOrder(List<TOrderItem> _needList, int _needWorkGague, TokenTile _placeTile, int _workPid = -1, WorkType _workType = WorkType.ChangeBuild)
    {
        if (_needList == null)
        {
            _needList = new();
            Debug.LogWarning("임시로 작업 재료 할당");
            _needList.Add(new TOrderItem(TokenType.Capital, (int)Capital.Wood, 5));
        }
            

        //작업주문서 작성
        m_needList = _needList;
        m_originWorkGauge = _needWorkGague;
        //m_restWorkGauge = m_originWorkGauge;
        m_restWorkGauge = 0; //임시로 필요 작업량 할당
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

    #region 자원 출입
    public bool PutResource(ITradeCustomer _customer)
    {
        //전체 필요한 양을 다 넣어야함. 
        //다만 중간에 상실하는 경우도 있으니 차이만큼 진행
        List<TOrderItem> requestList = new List<TOrderItem>(); //요청 목록 제작
      //  string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량
            TOrderItem cur = m_curList[i]; //현재 보유량
            TOrderItem need = cur; //요구량 생성
            need.Value = needItem.Value - cur.Value; //요구 수치 차액 진행
            requestList.Add(need);
          //  debugStr += requestList[i].Tokentype + " :" + requestList[i].Value + "필요";
        }
      //  Debug.Log(debugStr);
        TItemListData itemListData = new TItemListData(requestList);
        if (_customer.CheckInventory(itemListData) == false)
        {
            Debug.Log("재료 부족");
            return false;
        }
      //  Debug.Log("재료 지불");
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
        List<TOrderItem> requestList = new List<TOrderItem>(); //요청 목록 제작
                                                               //  string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량
            TOrderItem cur = m_curList[i]; //현재 보유량
            TOrderItem need = cur; //요구량 생성
            need.Value = needItem.Value - cur.Value; //요구 수치 차액 진행
            requestList.Add(need);
            //  debugStr += requestList[i].Tokentype + " :" + requestList[i].Value + "필요";
        }

        string ar = "";
        for (int i = 0; i < requestList.Count; i++)
        {
            ar += requestList[i].Tokentype + " " + requestList[i].Value + "\n";
        }

        Debug.Log(ar);
    }
    #endregion

    #region 작업진행
    //작업량 까는거
    public void DoWork()
    {
        if(IsDoneWork() == true)
        {
            //이미 완료된 작업
            DoEffect();
            return;
        }


        //일 시킨다
        if(IsReadyResource() == false)
        {
            Debug.LogWarning("재료가 부족하여 작업 수행 불가" + workType);
            return;
        }

        int laborCoin = GameUtil.GetTileTokenFromMap(WorkPlacePos).GetLaborCoinCount();
       // Debug.Log(laborCoin + "으로 작업진행");
        if(laborCoin == 0)
        {
            Debug.LogWarning("작업자수 부족 수행 불가");
            
        }

        //작업량 구하기
        int workAmount = m_baseworkEfficiency * 1; //기본 토큰1은 기본효율로
        int overToken = laborCoin - 1; //초과 토큰 수 계산
        int overWorkAmount = 0;
        if (1 <= overToken)
        {
            overWorkAmount = overToken * m_overWorkEfficiency;
        }
        //작업진행
        m_restWorkGauge -= (workAmount + overWorkAmount);
      //  Debug.Log("남은 작업량 " + m_restWorkGauge);
        if(m_restWorkGauge <= 0)
        {
            Debug.Log("작업완료");
            m_restWorkGauge = 0;
            DoEffect();
        }
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

        if(CheckCondition() == false)
        {
            return;
        }

        m_isComplete = true;
        EffectByCase();
        RemoveRegist(WorkStateCode.Complete);


    }

    private bool CheckCondition()
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
    public bool IsReadyResource()
    {
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량
            TOrderItem cur = m_curList[i]; //현재 보유량
            if(cur.Value < needItem.Value)
            {
                return false;
            }
       
        }
        return true;
    }

    public bool IsDoneWork()
    {
        if (m_restWorkGauge == 0)
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
        return m_originWorkGauge;
    }
}
