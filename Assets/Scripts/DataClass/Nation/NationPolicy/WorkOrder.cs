using System;
using System.Collections.Generic;
using UnityEngine;

public enum WorkType
{
    InterBuild, ChangeBuild
}

public class WorkOrder
{
    //작업 진행도

    //1. 필요한 자원
    //2. 최대 작업 토큰
    //3. 노동 토큰당 효율
    //4. 필요한 작업량
    public WorkType m_workType = WorkType.ChangeBuild; //어떤 타입 공사
    public int m_workPid = -1; //해당 작업에서 pid - 특정 공사인지 알기위한 부분
    private List<TOrderItem> m_needList = new List<TOrderItem>();
    private List<TOrderItem> m_curList = new List<TOrderItem>();
    private int m_originWorkGauge;
    private int m_restWorkGauge;
    
    private int m_baseworkEfficiency = 100; //토큰 1개일때 노동 효율
    private int m_overWorkEfficiency = 10; //추가되는 토큰에 따른 노동 효율

    private int m_workTokenNum; //할당된 노동 토큰 수
    private int m_maxWorkTokenNum = 3; //최대 할당 가능한 수 

    public WorkOrder(List<TOrderItem> _needList, int _needWorkGague, int _workPid = -1, WorkType _workType = WorkType.ChangeBuild)
    {
        //작업주문서 작성
        m_needList = _needList;
        m_workTokenNum = 1; //토큰 1개 임시 할당
        m_originWorkGauge = _needWorkGague;
        //m_restWorkGauge = m_originWorkGauge;
        m_restWorkGauge = 70; //임시로 필요 작업량 할당
        m_workType = _workType;
        m_workPid = _workPid;
       // string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem curItem = m_needList[i];
            curItem.Value = 0; //현재 재료 0 으로 
            m_curList.Add(curItem);
           // debugStr += m_curList[i].Tokentype + " :" + m_curList[i].Value + "필요";
        }
       // Debug.Log(debugStr);
    }

    public bool PushResource(ITradeCustomer _customer)
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

    public bool PushWorkToken()
    {
        if (m_workTokenNum == m_maxWorkTokenNum)
        {
            return false;
        }
        m_workTokenNum += 1;
        return true;
    }

    public bool TakeOutWorkToken()
    {
        if(m_workTokenNum == 0)
        {
            return false;
        }
        m_workTokenNum -= 1;
        return true;
    }

    public void DoWork()
    {
        if(IsDoneWork() == true)
        {
            //이미 완료된 작업
            return;
        }


        //일 시킨다
        if(IsReadyResource() == false)
        {
            Debug.Log("재료가 부족하여 작업 수행 불가");
            return;
        }

        if(m_workTokenNum == 0)
        {
            Debug.Log("작업자수 부족 수행 불가");
            return;
        }

        //작업량 구하기
        int workAmount = m_baseworkEfficiency * 1; //기본 토큰1은 기본효율로
        int overToken = m_workTokenNum - 1; //초과 토큰 수 계산
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
        }
    }

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
}
