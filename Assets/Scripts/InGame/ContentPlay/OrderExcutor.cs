﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderExcutor
{
    public void ExcuteOrder(TTokenOrder _order)
    {
        bool ableSelect = _order.AbleSelect;
        
        //집행에 선택이 가능한 경우는 선택지를 호출하고
        if(ableSelect == true)
        {
            MakeSelectUI(_order);
            return;
        }

        //불가능한 경우는 
        //적용 수 만큼 진행인데 지금은 주문서 모두 일괄적용중 
        for (int i = 0; i < _order.AdaptItemCount; i++)
        {
            AdaptItem(_order.orderItemList[i]);
        }
        return;

    }

    public void MakeSelectUI(TTokenOrder _order)
    {
        List<TOrderItem> ShowList = _order.orderItemList;

        // Debug.Log("선택류 리스트로 선택 정보 생성");
        int ableSelectCount = _order.AdaptItemCount; //임시로 1개만 고름 가능
        OneBySelectInfo oneBySelectInfo = new OneBySelectInfo(ShowList, ableSelectCount);
        oneBySelectInfo.OpenSelectUI();

    }

    public bool AdaptItem(TOrderItem _item)
    {
        //  Debug.Log("적용");
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        switch (_item.Tokentype)
        {
            case TokenType.Capital:
                //    Debug.Log("자원올린다");
                PlayerCapitalData.g_instance.PayCostData(new TItemListData(_item), false);
                return true; //따로 받은게 없음
            case TokenType.Equipt:
                //      Debug.Log("장비획득한다");
                EquiptItem equiptCopy = new EquiptItem(MgMasterData.GetInstance().GetEquiptData(_item.SubIdx));
                return mainChar.AquireEquipt(equiptCopy);
            case TokenType.CharStat:
                //    Debug.Log("스텟올린다");
                mainChar.CalStat((CharStat)_item.SubIdx, _item.Value);//형변환 안해두되는데 아쉽군. 
                return true;
            case TokenType.Tile:
                return ChangeQuestPlace(_item.SubIdx, _item.Value);
            case TokenType.MonsterNationSpawn:
                // Debug.Log("몬스터 소환");
                SpawnMonster(_item);
                return true;
            case TokenType.Conversation:
                // Debug.Log("대화 요청");
                MGConversation.GetInstance().ShowCheckScript(_item);
                return true;
            case TokenType.Nation:
                if (_item.SubIdx == (int)NationEnum.Move)
                {
                    GamePlayMaster.GetInstance().CharMoveToCapital(_item.Value);
                }
                return true;
            case TokenType.None:
                Debug.LogWarning("아무것도 하지 않는 주문");
                break;
        }
        //적용 항목이 없는 경우엔 적용 안된걸로
        return false;
    }

    #region 따로 정의가 필요한 아이템 타입들
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

    public bool ChangeQuestPlace(int _subIdx, int _value)
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
                    return true;
                }
            }
        }

        return false;
    }
    #endregion

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
    public bool AbleSelect; //리스트 중 선택이 가능한가 
    #region 주문서 생성
    public TTokenOrder(List<TOrderItem> _orderList, bool _ableSelect, int _selectCount, int _serialNum = 0)
    {
        orderItemList = _orderList;
        if (orderItemList == null)
            orderItemList = new();
        ChunkNum = 1;
        OrderExcuteCount = 0;
        OrderSerialNumber = _serialNum;
        AdaptItemCount = _selectCount;
        AbleSelect = _ableSelect;
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
