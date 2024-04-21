using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventStat
{
    ETokenType, ItemPID, ItemValue, GiveMethod
}

public class TokenEvent : TokenBase, IOrderCustomer
{
    public int m_selectCount = 0; //선택지 수
    public TTokenOrder TokenOrder;

    #region 이벤트 토큰 생성
    public TokenEvent()
    {
        m_tokenType = TokenType.Event;
    }

    //마스터데이터 생성
    public TokenEvent(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]); //시트 데이터상 0번째는 pid
        m_itemName = valueCode[1]; //1은 이름
        m_tokenType = TokenType.Event;
        m_tokenIValues = new int[System.Enum.GetValues(typeof(EventStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
    }

    //복사본 생성

    public TokenEvent(TokenEvent _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = _masterToken.m_tokenType;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //마스터 데이터 깊은 복사로 객체 고유 배열 값 생성. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //스텟값 복사
    }

    public static TokenEvent CopyToken(TokenEvent _origin)
    {
        return new TokenEvent(_origin);
    }
    #endregion

    public void ActiveEvent()
    {
        Debug.Log(m_tokenPid + "피아이디 발동");
        OrderExcutor orderExcutor = new();
        orderExcutor.ExcuteOrder(this);
        SendQuestCallBack();
    }

    public void RemoveEvent()
    {
        //0. 오브젝트 정리
        if (m_object != null)
            m_object.DestroyObject();

        //1. 사망시 처리
        SendQuestCallBack();

        //2. 데이터 참조 제거
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.DeleteEnterEvent();
        MgToken.GetInstance().RemoveCharToken(this);
    }

    public override void CleanToken()
    {
        base.CleanToken();
        //0. 오브젝트 정리
        if (m_object != null)
            m_object.DestroyObject();
        //2. 데이터 참조 제거
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.DeleteEnterEvent();
        MgToken.GetInstance().RemoveCharToken(this);
    }

    public void MakeEventContent(EOrderType _orderType, List<TOrderItem> _itemList)
    {
        //현재 이벤트의 PID와 지정된 값에 따라서 TTokenOrder 생성
       
        if (_orderType.Equals(EOrderType.SpawnMonster))
        {
            //취급하는 그룹이 캐릭터인경우 - 캐릭터 스폰 주문서로 생성
            TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, _itemList, ESpawnPosType.Random, GameUtil.GetMainCharChunkNum());
        }
        
        TokenOrder.SetOrderCustomer(this);
    }

    public void OnOrderCallBack(OrderReceipt _orderReceipt) //이벤트 토큰 고객
    {
        Debug.Log("이벤 토큰의 콜백 집행 횟수 "+_orderReceipt.Order.OrderExcuteCount);
        EOrderType orderType = _orderReceipt.Order.OrderType; //진행했던 오더 타입
        //1. 몬스터 생성 주문서 진행시 받은 몬스터에 자신이 속한 퀘스트를 또 연계할 수도있음. 
    }
}
