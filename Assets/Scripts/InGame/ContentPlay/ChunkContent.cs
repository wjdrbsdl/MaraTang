using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkType
{
    None, Monster, Capital, Special
}

public class ChunkContent
{
    public ChunkType chunkType = ChunkType.None;
    public List<TOrderItem> ItemList = new();
    public int PID;
    public ChunkContent()
    {

    }

    public ChunkContent(List<int[]> matchCode, string[] valueCode)
    {
        //m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        //GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        PID = int.Parse( valueCode[0]);
        ItemList = GameUtil.ParseCostDataArray(valueCode, 2).GetItemList();
        Debug.Log(ItemList[0].Tokentype + "파싱");
    }

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
    #endregion

}
