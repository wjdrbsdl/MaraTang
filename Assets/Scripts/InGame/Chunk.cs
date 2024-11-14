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
        //1. �� �䱸
        m_Pin = MgNaviPin.GetInstance().RequestNaviPin();
        //2. �� �޾����� ����
        if(m_Pin != null)
            m_Pin.SetPinInfo(tiles[2, 2].GetObject().transform.position, ChunkNum);
    }

    public void RemovePin()
    {
        //1.�� �����ڿ��� �� ���� �䱸
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
        //�ش� ûũ�� ���� ����
    }

    public void OnExitChunk()
    {
        //�ش� ûũ���� ���� ���� 
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
        //���� ������� ����. 
        int xLength = tiles.GetLength(0);
        int height = _index / xLength;
        int width = _index % xLength;
        return tiles[width,height];
    }
}