using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public TokenTile[,] tiles;
    public int ChunkNum;
    public Quest m_Quest;
    public NaviPin m_Pin;

    public Chunk() { }

    public Chunk(TokenTile[,] _tiles, int _num)
    {
        tiles = _tiles;
        ChunkNum = _num;
    }

    public void MakePin()
    {
        //1. 핀 요구
        m_Pin = MgNaviPin.GetInstance().RequestNaviPin();
        //2. 핀 받았으면 설정
        if(m_Pin != null)
            m_Pin.SetPinInfo(tiles[2, 2].GetObject().transform.position, ChunkNum);
    }

    public void RemovePin()
    {
        //1.핀 관리자에게 핀 제거 요구
        MgNaviPin.GetInstance().RemovePin(m_Pin);
        m_Pin = null;
    }

    public void ResetQuest()
    {
        m_Quest = null;
        RemovePin();
    }

    public void SwitchPin(bool _on)
    {
        m_Pin.SwitchPin(_on);
    }

    public void OnEnterChunk()
    {
        //해당 청크에 들어온 순간
    }

    public void OnExitChunk()
    {
        //해당 청크에서 나간 순간 
    }

    public void Dye(Color _color)
    {
        for (int i = 0; i < tiles.GetLength(0); i++)
        {
            for (int x = 0; x < tiles.GetLength(1); x++)
            {
                tiles[i, x].GetObject().GetComponent<SpriteRenderer>().color = _color;
            }
        }
    }

    public int GetTileCount()
    {
        int x = tiles.GetLength(0);
        int y = tiles.GetLength(1);
        return x * y;
    }

    public TokenTile GetTileByIndex(int _index)
    {
        //몇행 몇열인지를 빼기. 
        int xLength = tiles.GetLength(0);
        int height = _index / xLength;
        int width = _index % xLength;
        return tiles[width,height];
    }
}