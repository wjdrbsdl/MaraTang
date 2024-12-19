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
    public bool IsNation = false; //�ش� ������ ���� �������°� 

    public Chunk() { }

    public Chunk(List<int[]> _tilePosLIst, int _chunkNum)
    {
        TilePosList = _tilePosLIst;
        ChunkNum = _chunkNum;
    }

    public void MakePin()
    {
        //1. �� �䱸
        m_Pin = MgNaviPin.GetInstance().RequestNaviPin();
        //2. �� �޾����� ����
        if(m_Pin != null)
            m_Pin.SetPinInfo(GetTileByIndex(13).GetObject().transform.position, ChunkNum);
    }

    public void RemovePin()
    {
        //1.�� �����ڿ��� �� ���� �䱸
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
        //�ش� ûũ�� ���� ����
    }

    public void OnExitChunk()
    {
        //�ش� ûũ���� ���� ���� 
    }

    public void ResetContent()
    { 
        if(FixedValue.SAY_CHUNKRESET == false)
        {
            Debug.LogWarning("���� ������ ���� ������ ����� �����");
            FixedValue.SAY_CHUNKRESET = true;
        }
        
        if (PreContent == null)
            return;

        //���� �������� ���� ����� ����
        PreContent = null;
    }

    public bool RealizeContent(ChunkContent _chunkContent)
    {
        ResetContent();
        PreContent = _chunkContent;
        bool doneRealize = _chunkContent.Realize(this);
        //���� ���� ������ �� ������ �����ع�����
        if(doneRealize == false)
        {
            ResetContent();
            //���� ��ȯ
            return false;
        }

        return true;

    }

    public void MakeCore()
    {
        //�� Ÿ���� �ϳ��� �ھ�� �ٲ� 
        List<int> tileRan = GameUtil.GetRandomNum(TilePosList.Count, TilePosList.Count);
        for (int i = 0; i < tileRan.Count; i++)
        {
            //0��°���� �������� nomal�� �� ã��
            TokenTile tile = GameUtil.GetTileTokenFromMap(TilePosList[tileRan[i]]);
            if(tile.GetTileType() == TileType.Nomal)
            {
                //Debug.LogFormat("{0}�� ������ {1}��° Ÿ���� �ھ�� ���� �ش� Ÿ���� ������{2}",ChunkNum, tileRan[i], tile.ChunkNum);
                tile.ChangePlace(TileType.ChunkCore);
                SetCoreLive(true);
                break;
            }
        }
    }

    #region Ÿ�� ������
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
        //���� ������� ����. 
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

    public void SetCoreLive(bool _live)
    {
        m_aliveCore = _live;
    }

    public bool GetCoreLive()
    {
        return m_aliveCore;
    }
}