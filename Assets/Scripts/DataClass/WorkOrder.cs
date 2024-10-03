using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkOrder
{
    //작업 진행도

    //1. 필요한 자원
    //2. 최대 작업 토큰
    //3. 노동 토큰당 효율
    //4. 필요한 작업량
    private List<TOrderItem> m_needList = new List<TOrderItem>();
    private List<TOrderItem> m_curList = new List<TOrderItem>();
    private int m_originWorkGauge;
    private int m_restWorkGauge;
    private int m_workEfficiency = 10;
    private int m_workTokenNum; //할당된 노동 토큰 수
    private int m_maxWorkTokenNum = 3; //최대 할당 가능한 수 

    public WorkOrder(int _planIndex)
    {
        TItemListData changeCost = MgMasterData.GetInstance().GetTileData(_planIndex).BuildCostData;
        m_needList = changeCost.GetItemList();
        m_originWorkGauge = 100;
        string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem curItem = m_needList[i];
            debugStr += m_needList[i].Tokentype + " :" + m_needList[i].Value + "필요\n";
            curItem.Value = 0; //현재 재료 0 으로 
            m_curList.Add(curItem);
            debugStr += m_curList[i].Tokentype + " :" + m_curList[i].Value + "보유\n";
        }
        Debug.Log(debugStr);
    }

    public WorkOrder(List<TOrderItem> _needList, int _needWorkGague)
    {
        //작업주문서 작성
        m_needList = _needList;
        m_originWorkGauge = _needWorkGague;
        string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem curItem = m_needList[i];
            curItem.Value = 0; //현재 재료 0 으로 
            m_curList.Add(curItem);
            debugStr += m_curList[i].Tokentype + " :" + m_curList[i].Value + "필요";
        }
        Debug.Log(debugStr);
    }

    public bool PushResource(ITradeCustomer _customer)
    {
        //전체 필요한 양을 다 넣어야함. 
        //다만 중간에 상실하는 경우도 있으니 차이만큼 진행
        List<TOrderItem> requestList = new List<TOrderItem>(); //요청 목록 제작
        string debugStr = "";
        for (int i = 0; i < m_needList.Count; i++)
        {
            TOrderItem needItem = m_needList[i]; // 애초 필요량
            TOrderItem cur = m_curList[i]; //현재 보유량
            TOrderItem need = cur; //요구량 생성
            need.Value = needItem.Value - cur.Value; //요구 수치 차액 진행
            requestList.Add(need);
            debugStr += requestList[i].Tokentype + " :" + requestList[i].Value + "필요";
        }
        Debug.Log(debugStr);
        TItemListData itemListData = new TItemListData(requestList);
        if (_customer.CheckInventory(itemListData))
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

}
