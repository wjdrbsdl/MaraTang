using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public int ChunkNum;
    public NaviPin m_Pin;
    public ChunkContent PreContent;
    public List<int[]> TilePosList;
    private bool m_aliveCore = false;
    public bool IsNation = false; //해당 구역에 나라가 세워졌는가 

    public Chunk() { }

    public Chunk(List<int[]> _tilePosLIst, int _chunkNum)
    {
        TilePosList = _tilePosLIst;
        ChunkNum = _chunkNum;
    }

    public void MakePin()
    {
        //1. 핀 요구
        m_Pin = MgNaviPin.GetInstance().RequestNaviPin();
        //2. 핀 받았으면 설정
        if(m_Pin != null)
            m_Pin.SetPinInfo(GetTileByIndex(13).GetObject().transform.position, ChunkNum);
    }

    public void RemovePin()
    {
        //1.핀 관리자에게 핀 제거 요구
        MgNaviPin.GetInstance().RemovePin(m_Pin);
        m_Pin = null;
    }

    public void ResetQuest()
    {
       RemovePin();
    }

    public void SwitchPin(bool _on)
    {
        m_Pin.SwitchPin(_on);
    }

    public void OnEnterChunk()
    {
        //해당 청크에 들어온 순간
     //   Debug.Log(ChunkNum + "구역에 들어옴");
        if(PreContent != null)
        PreContent.WakeMonster();
    }

    public void OnExitChunk()
    {
        //해당 청크에서 나간 순간 
    //    Debug.Log(ChunkNum + "구역에서 나감");
        if (PreContent != null)
            PreContent.SleepMonster();
    }

    #region 구역컨텐츠
    public void CountContentTurn()
    {
        if(PreContent == null)
        {
            MakeContent();
            return;
        }

        PreContent.RestTurn -= 1;
        if(PreContent.RestTurn == 0)
        {
          //  Debug.Log("컨텐츠 유지시간 다되서 리셋 다음 컨텐츠 생성 요구");
            ResetContent();
            MakeContent();
        }
    }

    public void MakeContent()
    {
        if (GamePlayMaster.GetInstance().m_actChunkContent == false)
            return;
        if (GetCoreLive() == false)
        {
          //  Debug.Log("핵이 파괴된 구역, 컨텐츠 구현 패싱 " + ChunkNum);
            return;
        }
      //  Debug.Log("구역에서 컨텐츠 생성");
        int testTier = 3;
        //3. 티어대로 구역 컨텐츠 생성
        ChunkContent content = new ChunkContent(testTier);
        ////3. 컨텐츠 불러온거
        //chunkcontent content = new chunkcontent(mgmasterdata.getinstance().getchunkcontent(1));
        //5. 일단 그냥 컨텐츠 구현
        RealizeContent(content);
        content.RestTurn = 1; 
    }


    public void ResetContent()
    { 
        if(FixedValue.SAY_CHUNKRESET == false)
        {
            Debug.LogWarning("구역 컨텐츠 새로 지을때 지울거 지우고");
            FixedValue.SAY_CHUNKRESET = true;
        }
        
        if (PreContent == null)
            return;

        PreContent.ResetItems();

        //기존 컨텐츠에 따라 지울거 지움
        PreContent = null;
    }

    public bool RealizeContent(ChunkContent _chunkContent)
    {
        ResetContent();
        PreContent = _chunkContent;
        bool doneRealize = _chunkContent.Realize(this);
        //구현 실패 했으면 그 컨텐츠 리셋해버리고
        if(doneRealize == false)
        {
            ResetContent();
            //실패 반환
            return false;
        }

        return true;

    }
    #endregion

    #region 코어 관리
    public void MakeCore()
    {
        //내 타일중 하나를 코어로 바꿈 
        List<int> tileRan = GameUtil.GetRandomNum(TilePosList.Count, TilePosList.Count);
        for (int i = 0; i < tileRan.Count; i++)
        {
            //0번째부터 최종까지 nomal인 맵 찾기
            TokenTile tile = GameUtil.GetTileTokenFromMap(TilePosList[tileRan[i]]);
            if(tile.GetTileType() == TileType.Nomal)
            {
                //Debug.LogFormat("{0}번 구역의 {1}번째 타일을 코어로 정의 해당 타일의 구역은{2}",ChunkNum, tileRan[i], tile.ChunkNum);
                tile.ChangePlace(TileType.ChunkCore);
                SetCoreLive(true);
                break;
            }
        }
    }

    public void SetCoreLive(bool _live)
    {
        m_aliveCore = _live;
    }

    public bool GetCoreLive()
    {
        return m_aliveCore;
    }
    #endregion

    #region 타일 빼오기
    public int GetTileCount()
    {
        return TilePosList.Count;

        //int x = tiles.GetLength(0);
        //int y = tiles.GetLength(1);
        //return x * y;
    }

    public TokenTile GetTileByIndex(int _index)
    {
        return GameUtil.GetTileTokenFromMap(TilePosList[_index]);
        //몇행 몇열인지를 빼기. 
        //int xLength = tiles.GetLength(0);
        //int height = _index / xLength;
        //int width = _index % xLength;
        //return tiles[width,height];
    }

    public TokenTile GetRandomTile()
    {
        int randomTile = Random.Range(0, GetTileCount());
        return GetTileByIndex(randomTile);
    }
    #endregion

}