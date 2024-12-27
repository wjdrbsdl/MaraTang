using System;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkContentType
{
    None, Monster, Capital, Special
}

public class ChunkContent
{
    public ChunkContentType chunkType = ChunkContentType.None;
    public List<TOrderItem> ItemList = new();
    public List<TokenBase> MadeList = new(); //만들어진 결과물
    public int PID;

    #region 생성자
    public ChunkContent()
    {

    }

    public ChunkContent(int _tier)
    {
        //티어와 그밖의 변수로 적절한 구역 컨텐츠 생성
        //티어에 따라 스폰할 몬스터와, 장소를 정의
        TOrderItem monsterItem = new TOrderItem(TokenType.Char, 2, -1);
        ItemList.Add(monsterItem);
    }

    public ChunkContent(List<int[]> matchCode, string[] valueCode)
    {
        //m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        //GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        PID = int.Parse( valueCode[0]);
        ItemList = GameUtil.ParseCostDataArray(valueCode, 2).GetItemList();
        //Debug.Log(ItemList[0].Tokentype + "파싱");
    }

    public ChunkContent(ChunkContent _origin)
    {
        PID = _origin.PID;
        chunkType = _origin.chunkType;
        //아이템 리스트 복사 
        for (int i = 0; i < _origin.ItemList.Count; i++)
        {
            TOrderItem item = _origin.ItemList[i];
            ItemList.Add(item);
        }
    }
    #endregion

    #region 아이템들 구체화
    public bool Realize(Chunk _chunk)
    {
        bool enoughTile = DoPosRevise(_chunk.GetTileCount(), _chunk);
        if (enoughTile == false)
            return false;

        for (int i = 0; i < ItemList.Count; i++)
        {
            bool doneAdapt = AdaptItem(ItemList[i], _chunk);
            //하나라도 적용못했으면 실패 반환
            if (doneAdapt == false)
                return false;
        }
        return true;
    }

    private bool DoPosRevise(int _chunkTileCount, Chunk _chunk)
    {
        //랜덤인 Pos인 경우 미리 위치를 바꿔놓을것
        List<int> except = new();
        List<int> needSelectIndexList = new(); //밸류값이 렌덤이라서 수정이 필요한 아이템의 Idx
        for (int i = 0; i < ItemList.Count; i++)
        {
            TOrderItem item = ItemList[i];
            //바꿔야하는 타입이 아니면 안바꿈
            if (item.Tokentype != TokenType.Char && item.Tokentype != TokenType.Tile)
                continue;

            //랜덤화 필요 숫자는 셈
            if(item.Value < 0 || _chunkTileCount <= item.Value)
            {
                needSelectIndexList.Add(i);
                continue;
            }

            //발생하려는 위치가 국가 타일이면 바꿈 
            if(_chunk.GetTileByIndex(item.Value).GetNation() != null)
            {
                needSelectIndexList.Add(i);
                continue;
            }

            //그밖에는 제외에 필요한 숫자
            except.Add(item.Value);
        }

        //범위는 타일 숫자 0~24개 중에서 수정이 필요한 인덱스 숫자 만큼, 제외할 인덱스를 빼고 진행 
        List<int> newRandomValue = GameUtil.GetRandomNum(_chunkTileCount, needSelectIndexList.Count, except);
        if (newRandomValue.Count != needSelectIndexList.Count)
        {
            Debug.Log("구역에 여분 타일 부족");
            return false;
        }
            
        for (int i = 0; i < needSelectIndexList.Count; i++)
        {
            TOrderItem reviseItem = ItemList[needSelectIndexList[i]]; //인덱스에 해당하는 아이템을 호출
            reviseItem.Value = newRandomValue[i]; //할당받은 랜덤값으로 수정
            ItemList[needSelectIndexList[i]] = reviseItem; //리스트 내부의 아이템을 변경 
        }
        return true;
    }

    public void MakeSelectUI(TTokenOrder _order)
    {
        List<TOrderItem> ShowList = _order.orderItemList;

        // Debug.Log("선택류 리스트로 선택 정보 생성");
        int ableSelectCount = _order.AdaptItemCount; //임시로 1개만 고름 가능
        OneBySelectInfo oneBySelectInfo = new OneBySelectInfo(ShowList, ableSelectCount);
        oneBySelectInfo.OpenSelectUI();

    }

    public bool AdaptItem(TOrderItem _item, Chunk _chunk)
    {
        //  Debug.Log("적용");
        TokenTile targetTile = _chunk.GetTileByIndex(_item.Value);//타겟 타일이 필요한경우 미리 뽑
        switch (_item.Tokentype)
        {
            case TokenType.Char:
                int monsterPid = _item.SubIdx;
             
                return SpawnMonster(monsterPid, targetTile);
            case TokenType.Tile:
                TileType tileType = (TileType)_item.SubIdx;
                ChangePlace(tileType, targetTile);
                return true;
            case TokenType.None:
                Debug.LogWarning("아무것도 하지 않는 주문");
                break;
            default:
                return true;
        }
        //적용 항목이 없는 경우엔 적용 안된걸로
        return false;
    }
    #endregion 

    public void ResetItems()
    {
        for (int i = 0; i < MadeList.Count; i++)
        {
            TokenBase token = MadeList[i];
            if(token.GetTokenType() == TokenType.Char)
            {
              ((TokenChar)token).CleanToken();
                continue;
            }
            if(token.GetTokenType() == TokenType.Tile)
            {
                ((TokenTile)token).ChangePlace(TileType.Nomal);
            }
        }
    }

    #region 따로 정의가 필요한 아이템 타입들
    private bool SpawnMonster(int _monsterPid, TokenTile _targetTile)
    {
        int[] pos = _targetTile.GetMapIndex();
        TokenChar spawnChar = MgToken.GetInstance().SpawnCharactor(pos, _monsterPid); //월드 좌표로 pid 토큰 스폰 
        if (spawnChar == null)
            return false;

        spawnChar.SetState(CharState.Sleep);//몬스터 재우기
        if (RuleBook.curChunkNum == _targetTile.ChunkNum)
        {
            //몬스터 스폰 시킨 구역에 플레이어와 이미 들어와 잇다면
         //   Debug.Log("플레이어가 있는 구역 ");
            spawnChar.SetState(CharState.Idle);//일반상태로 깨우기
        }
        MadeList.Add(spawnChar);
        return true;
    }

    private bool ChangePlace(TileType _tileType, TokenTile _targetTile)
    {
        _targetTile.ChangePlace(_tileType);
        MadeList.Add(_targetTile);
        return true;
    }
    #endregion

    public void WakeMonster()
    {
        for (int i = 0; i < MadeList.Count; i++)
        {
            TokenBase item = MadeList[i];
            if (item.GetTokenType().Equals(TokenType.Char))
            {
                ((TokenChar)item).SetState(CharState.Idle);
            }
        }
    }

    public void SleepMonster()
    {
        for (int i = 0; i < MadeList.Count; i++)
        {
            TokenBase item = MadeList[i];
            if (item.GetTokenType().Equals(TokenType.Char))
            {
                TokenChar monster = ((TokenChar)item);
                if (monster.GetState() == CharState.Idle)
                    monster.SetState(CharState.Sleep);
            }
        }
    }
}
