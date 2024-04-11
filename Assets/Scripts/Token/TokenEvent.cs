using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenEvent : TokenBase, IOrderCustomer
{
    public int m_selectCount = 0; //선택지 수
    public TTokenOrder TokenOrder;
    #region 이벤트 토큰 생성
    public TokenEvent()
    {

    }

    //마스터데이터 생성
    public TokenEvent(int pid, int value2)
    {
        m_tokenPid = pid;
    }
    
    //복사본 생성

    public TokenEvent(TokenEvent _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
    }

    public static TokenEvent CopyToken(TokenEvent _origin)
    {
        return new TokenEvent(_origin);
    }
    #endregion

    /*
     * 이벤트 토큰 - 입장하면 자동 발생하는 녀석 
     * 선택지가 주어지고 선택하는 방식 
     * -> 자동 선택형 - 주어진 선택지중, 하나를 자동적으로 선택한 효과 
     * -> 그외 - 주어진 선택지중 선택하거나, 취소 -> 취소시 해당 이벤트는 취소 값을 반환 
     * 받는 결과값 - 선택, 성공, 실패, 취소 
     * 
     * 선택으로 인해 수행 - 조건 변화, 보상 획득, 몬스터 소환 등 
     */

    public void ActiveEvent()
    {
        Debug.Log(m_tokenPid + "피아이디 발동");
        OrderExcutor.ExcuteOrder(this);
    }

    public void SelectEvent()
    {

    }

    public void MakeEventContent()
    {
        //각자 이벤트 토큰에서 이벤트 만들기? 
        int tempPid = 2;
        int tempCount = 3;
        ESpawnPosType tempSpawnPos = ESpawnPosType.Random;
        TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, tempPid, tempCount, tempSpawnPos, GameUtil.GetMainCharChunkNum());
        TokenOrder.SetOrderCustomer(this);
    }

    public void OrderCallBack(OrderReceipt _orderReceipt) //이벤트 토큰 고객
    {
        Debug.Log("이벤 토큰에서 주문완료 콜백받음");
        //1. 몬스터 생성 주문서 진행시 받은 몬스터에 자신이 속한 퀘스트를 또 연계할 수도있음. 
    }
}
