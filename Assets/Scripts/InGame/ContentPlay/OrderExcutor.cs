using System;
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
            AdaptItem(_order.orderItemList[i], _order.OrderSerialNumber);
        }
        return;

    }

    public void MakeSelectUI(TTokenOrder _order)
    {
        List<TOrderItem> ShowList = _order.orderItemList;

        // Debug.Log("선택류 리스트로 선택 정보 생성");
        int ableSelectCount = _order.AdaptItemCount; //임시로 1개만 고름 가능
        OneBySelectInfo oneBySelectInfo = new OneBySelectInfo(ShowList, ableSelectCount, _order.OrderSerialNumber);
        oneBySelectInfo.OpenSelectUI();

    }

    public bool AdaptItem(TOrderItem _item, int _serialNum = FixedValue.No_VALUE)
    {
        //  Debug.Log("적용");
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        switch (_item.Tokentype)
        {
            case TokenType.Capital:
                PlayerCapitalData.g_instance.PayCostData(new TItemListData(_item), false);
                return true; 
            case TokenType.Bless:
                GodBless bless = new GodBless(MgMasterData.GetInstance().GetGodBless(_item.SubIdx));
                return mainChar.AquireBless(bless);
            case TokenType.Equipt:
                EquiptItem equiptCopy = new EquiptItem(MgMasterData.GetInstance().GetEquiptData(_item.SubIdx));
                return mainChar.AquireEquipt(equiptCopy);
            case TokenType.CharStat:
                mainChar.CalStat((CharStat)_item.SubIdx, _item.Value);//형변환 안해두되는데 아쉽군. 
                return true;
            case TokenType.EventPlaceNationSpawn:
                return ChangeQuestPlace(_item.SubIdx, _item.Value);
            case TokenType.EventPlaceChunkSpawn:
                return ChangeChunkPlace(_item.SubIdx, _item.Value);
            case TokenType.MonsterNationSpawn:
                SpawnMonster(_item);
                return true;
            case TokenType.Conversation:
                MGConversation.GetInstance().ShowScriptItem(_item, _serialNum);
                return true;
            case TokenType.Nation:
                if (_item.SubIdx == (int)NationEnum.Move)
                {
                    GamePlayMaster.GetInstance().CharMoveToCapital(_item.Value);
                }
                return true;
            case TokenType.ChunkContent:
                Chunk chunk = MGContent.GetInstance().GetChunk(5); //임시로 5번째 청크
                ChunkContent chunkContent = new ChunkContent(MgMasterData.GetInstance().GetChunkContent(_item.SubIdx)); //마스터 데이터에서 복사 
                return chunk.RealizeContent(chunkContent);
            case TokenType.Content:
                DoContentAdapt(_item.SubIdx, _item.Value);
                break;
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

    public bool ChangeChunkPlace(int _tileType, int _value)
    {
        Chunk chunk = MGContent.GetInstance().GetChunk(_value);
        //잘못된 입력으로 null 반환시 0 번째 청크로 임시 진행 
        if(chunk == null)
        {
            Debug.LogWarning("잘못된 입력으로 null 반환시 0 번째 청크로 임시 진행 ");
            chunk = MGContent.GetInstance().GetChunk(0);
        }
            
        chunk.GetRandomTile().ChangePlace((TileType)_tileType);
        return false;

    }

    public bool ChangeQuestPlace(int _tileType, int _value)
    {
        //국가경계를 기준으로 주변 타일 하나를
        //정해진 타일로 변경하는 주문 
        TileReturner tileReturner = new();
        //국가 영토중 외곽 타일 하나를 집는다. 
        TokenTile targetTile = tileReturner.NationBoundaryTile();
        Debug.Log("주변 타일 바꿀때 기준이 되는 타일 확인위해서 WoodLand로 변경");
        targetTile.ChangePlace(TileType.Police); //확인위해 변경
        //그 타일부터 사거리 3~5 중에 영토 주인 없는걸로 진행
        for (int range = 3; range <= 5; range++)
        {
            List<TokenTile> roundTile = GameUtil.GetTileTokenListInRange(range, targetTile.GetMapIndex(), range);
            for (int r = 0; r < roundTile.Count; r++)
            {
                int tileNationNum = roundTile[r].GetStat(ETileStat.Nation);
                //주변 영토중 주인없는 거 찾기
                if (tileNationNum == FixedValue.NO_NATION_NUMBER)
                {
                    //찾았으면 변경
                 //   Debug.Log("해당 영지 변경");
                    roundTile[r].ChangePlace((TileType)_tileType);
                    return true;
                }
            }
        }

        return false;
    }

    private void DoContentAdapt(int _contentEnum, int _value)
    {
        switch ((ContentEnum)_contentEnum)
        {
            case ContentEnum.GameOver:
                UnityEngine.SceneManagement.SceneManager.LoadScene("0.IntroScene");
                break;
        }
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

    public TOrderItem(ConversationThemeEnum _theme, ConversationData _covnersation)
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
