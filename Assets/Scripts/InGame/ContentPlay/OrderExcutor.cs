﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderExcutor
{
    public void ExcuteOrder(TTokenOrder _order)
    {
        int orderCount = _order.orderItemList.Count;
        if (orderCount == _order.AdaptItemCount || _order.AdaptItemCount == 0) //0은 마스터데이터에서 임시로 모두 적용할거라고 임시로 약속한 값 
        {
            //주문수와 적용 수가 같으면 모두 그대로 적용
            for (int i = 0; i < orderCount; i++)
            {
                ExcuteOrderItem(_order.orderItemList[i]);
            }
            return;
        }
        //숫자가 다른경우는 선택형으로 뽑기 UI 출력 
        int minSelect = 1; //최소 선택수 일단 1
        int maxSelect = _order.AdaptItemCount; //최대 선택수는 적용하려는 수치
        SelectItemInfo selectInfo = new SelectItemInfo(_order.orderItemList, true, minSelect, maxSelect); //선택받기 위해서 선택정보 생성
        Action confirmAction = delegate
        {
            ExcuteSelectItem(_order, selectInfo);
        };
        selectInfo.SetAction(confirmAction);
        MgUI.GetInstance().ShowItemList(selectInfo);

    }

    public void ExcuteSelectItem(TTokenOrder _order, SelectItemInfo _selectInfo)
    {
        // Debug.Log("컨펌 누르고 진행");
        List<int> selectList = _selectInfo.SelectedIndex;
        for (int i = 0; i < selectList.Count; i++)
        {
            ExcuteOrderItem(_order.orderItemList[selectList[i]]);
        }
    }

    public void ExcuteOrderItem(TOrderItem _oritem)
    {
        TOrderItem orderItem = _oritem;
        TokenType tokenGroup = orderItem.Tokentype;
        int orderSubIdx = orderItem.SubIdx;
        int orderValue = orderItem.Value;
        //선택한 아이템이 다시 이벤트 생성 , 몬스터 소환같은거면 어떡함?
        switch (tokenGroup)
        {
            //개별적으로 CallBack을 보내는 경우는 return.
            case TokenType.Tile:
                ChangeQuestPlace(orderSubIdx, orderValue);
                break;
            case TokenType.MonsterNationSpawn:
                // Debug.Log("몬스터 소환");
                SpawnMonster(orderItem);
                break;
            case TokenType.CharStat:
                PlayerManager.GetInstance().GetMainChar().CalStat((CharStat)orderSubIdx, orderValue);
                break;
            case TokenType.Capital:
                Capital capitalType = (Capital)orderSubIdx;
                PlayerCapitalData.g_instance.CalCapital(capitalType, orderValue);
                break;
            case TokenType.Action:
                break;
            case TokenType.Conversation:
                // Debug.Log("대화 요청");
                MGConversation.GetInstance().ShowCheckScript(orderItem);
                break;
            case TokenType.Nation:
                if (orderSubIdx == (int)NationEnum.Move)
                {
                    GamePlayMaster.GetInstance().CharMoveToCapital(orderValue);
                }
                break;
            case TokenType.None:
                Debug.LogWarning("아무것도 하지 않는 주문");
                break;
            default:
                Debug.LogWarning("정의 되지 않은 토큰타입 " + tokenGroup);
                break;
        }
    }

    private void SpawnMonster( TOrderItem _monterOrder)
    {
        TokenType spawnType = _monterOrder.Tokentype;
        //1. 스폰할 몬스터
        int tokenPid = _monterOrder.SubIdx;
        //2. 스폰할 갯수 
        int spawnCount = _monterOrder.Value;
        //3. 스폰 장소 - 국경중 하나로
        //Order의 토큰타입으로 어디서 어떻게 스폰할지 다 정해놓기 일단 모든 스폰은 국경에서 스폰하는걸로 
        //if(_monterOrder.Tokentype == TokenType.MonsterNationSpawn)
        //{

        //}
        TileReturner retuner = new TileReturner();
        int[] spawnPos = retuner.NationBoundaryTile().GetMapIndex();

        //4. 스폰 진행
        for (int i = 0; i < spawnCount; i++)
        {
            MgToken.GetInstance().SpawnCharactor(spawnPos, tokenPid); //월드 좌표로 pid 토큰 스폰 
        }

    }

    public void ChangeQuestPlace(int _subIdx, int _value)
    {
        //국가경계를 기준으로 주변 타일 하나를
        //정해진 타일로 변경하는 주문 
        TileReturner tileReturner = new();
        //국가 영토중 외곽 타일 하나를 집는다. 
        TokenTile targetTile = tileReturner.NationBoundaryTile();
        targetTile.ChangePlace(TileType.WoodLand); //확인위해 변경
        //그 타일부터 사거리 3~5 중에 영토 주인 없는걸로 진행
        for (int range = 3; range <= 5; range++)
        {
            List<TokenTile> roundTile = GameUtil.GetTileTokenListInRange(range, targetTile.GetMapIndex(), range);
            for (int r = 0; r < roundTile.Count; r++)
            {
                //주변 타일의 넘버가 같은 국가가 하나라도 국가가 아니면 얘는 외각
                int tileInRange = roundTile[r].GetStat(TileStat.Nation);
                if (tileInRange == FixedValue.NO_NATION_NUMBER)
                {
                    Debug.Log("해당 영지 변경");
                    roundTile[r].ChangePlace((TileType)16);
                    return;
                }
            }
        }

    }

}

public enum ESpawnPosType
{
    //무언갈 스폰할때 타입 
    Random, CharRound
}

public struct TTokenOrder
{
    //토큰 생성, 선택등의 작업을 진행하는곳

    public List<TOrderItem> orderItemList; //토큰 타입, 서브pid, 수량(밸류)로 묶은 orderItem.
    public int ChunkNum; //아무 지정이 아니면 - 1
    public int OrderExcuteCount; //집행한 수 - 단계로변경 필요?
    public int OrderSerialNumber; //주문서 일련번호 - 한 고객에게 여러 콜백이 들어갈경우, 어떤 퀘스트나, 컨텐츠 에서 나온건지 확인하기 위해서. 
    public int AdaptItemCount; //적용할 아이템 수 - 0이면 모두, 그 외에는 선택 요청해서 진행 

    #region 주문서 생성
    public TTokenOrder(List<TOrderItem> _orderList, int _selectCount, int _serialNum = 0)
    {
        orderItemList = _orderList;
        if (orderItemList == null)
            orderItemList = new();
        ChunkNum = 1;
        OrderExcuteCount = 0;
        OrderSerialNumber = _serialNum;
        AdaptItemCount = _selectCount;
        CheckAllValue();
    }

    //마스터 데이터에서 실시간 조건이 필요한 경우 Value에 All을 넣어서 파악
    private void CheckAllValue()
    {
        for (int i = 0; i < orderItemList.Count; i++)
        {
            TOrderItem curItem = orderItemList[i];
            if (curItem.Value == FixedValue.ALL)
            {
                int curIdx = i;
                TokenType type = curItem.Tokentype;
                if (type.Equals(TokenType.Nation))
                {
                    int nationNumber = MgNation.GetInstance().GetNationList().Count;
                    orderItemList.RemoveAt(curIdx); //All 벨류 아이템을 제거
                    for (int nationIdx = 0; nationIdx < nationNumber; nationIdx++)
                    {
                        TOrderItem nationItem = new TOrderItem(TokenType.Nation, curItem.SubIdx, nationIdx); //해당 국가를 아이템으로 생성 
                        orderItemList.Insert(curIdx, nationItem); //해당 idx부터 벨류가 정해진 아이템을 추가 
                    }
                }
            }
        }
    }
    #endregion

}

public struct TOrderItem
{
    //주문서 내부의 개별 아이템 항목 정보
    public TokenType Tokentype;
    public int SubIdx;
    public int Value;
    public int SerialNum;

    public TOrderItem(TokenType _tokenGroup, int _subIdx, int _value)
    {
        Tokentype = _tokenGroup;
        SubIdx = _subIdx;
        Value = _value;
        SerialNum = 0;
    }

    public TOrderItem(ConversationEnum _theme, ConversationData _covnersation)
    {
        Tokentype = TokenType.Conversation;
        SubIdx = (int)_theme;
        Value = _covnersation.GetPid();
        SerialNum = 0;
    }

    public void SetSerialNum(int _serialNum)
    {
     //   Debug.Log("시리얼 넘버로 세팅중" + _serialNum);
        SerialNum = _serialNum;
    }

    public void SetValue(int _value)
    {
        Value = _value;
   
    }

    public bool IsVaridTokenType()
    {
        return Tokentype.Equals(TokenType.None)==false;
    }
}
