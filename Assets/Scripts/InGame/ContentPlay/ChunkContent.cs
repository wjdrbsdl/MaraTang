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

    public void Realize(Chunk _chunk)
    {
        PosRevise(_chunk.GetTileCount(), _chunk);
        for (int i = 0; i < ItemList.Count; i++)
        {
            AdaptItem(ItemList[i], _chunk);
        }
    }

    private void PosRevise(int _chunkTileCount, Chunk _chunk)
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
        for (int i = 0; i < needSelectIndexList.Count; i++)
        {
            TOrderItem reviseItem = ItemList[needSelectIndexList[i]]; //인덱스에 해당하는 아이템을 호출
            reviseItem.Value = newRandomValue[i]; //할당받은 랜덤값으로 수정
            ItemList[needSelectIndexList[i]] = reviseItem; //리스트 내부의 아이템을 변경 
        }
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
                int[] pos = targetTile.GetMapIndex();
                SpawnMonster(monsterPid, pos);
                return true; 
            case TokenType.Tile:
                TileType tileType = (TileType)_item.SubIdx;
                ChangePlace(tileType, targetTile);
                return true;
            case TokenType.None:
                Debug.LogWarning("아무것도 하지 않는 주문");
                break;
        }
        //적용 항목이 없는 경우엔 적용 안된걸로
        return false;
    }

    #region 따로 정의가 필요한 아이템 타입들
    private void SpawnMonster(int _monsterPid, int[] _spawnPos)
    {
        MgToken.GetInstance().SpawnCharactor(_spawnPos, _monsterPid); //월드 좌표로 pid 토큰 스폰 
    }

    public bool ChangePlace(TileType _tileType, TokenTile _targetTile)
    {
        _targetTile.ChangePlace(_tileType);
        return true;
    }
    #endregion

}
