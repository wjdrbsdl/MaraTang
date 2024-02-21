using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public TokenTile[,] tiles;
    public int ChunkNum;
    public Quest Quest;

    public Chunk() { }

    public Chunk(TokenTile[,] _tiles, int _num)
    {
        tiles = _tiles;
        ChunkNum = _num;
    }

    public void MakeEventToken()
    {
        for (int i = 0; i < 3; i++)
        {
            int tempPid = i;
            int tempValue = i + 1;

            //0. 생성할 땅 찾고
            int x = 0;
            int y = i;
            TokenTile tile = tiles[x, y];

            //1.이벤트 토큰 및 오브젝트 만들고
            TokenEvent eventToken = new TokenEvent(tempPid, tempValue);
            eventToken.SetMapIndex(tile.GetXIndex(),tile.GetYIndex()); //이벤트 토큰 위치 잡고
            ObjectTokenBase objectToken = MonoBehaviour.Instantiate(GamePlayMaster.GetInstance().testEventGO).GetComponent<ObjectTokenBase>();
            objectToken.SetToken(eventToken, TokenType.Event);
            objectToken.SyncObjectPosition(); //오브젝트 자체 이동 시키고 

            //2. 타일에 이벤트 심기
            tile.SetEnteraceEvent(eventToken);

        }
    }

}