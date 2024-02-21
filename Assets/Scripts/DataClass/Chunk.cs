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

            //0. ������ �� ã��
            int x = 0;
            int y = i;
            TokenTile tile = tiles[x, y];

            //1.�̺�Ʈ ��ū �� ������Ʈ �����
            TokenEvent eventToken = new TokenEvent(tempPid, tempValue);
            eventToken.SetMapIndex(tile.GetXIndex(),tile.GetYIndex()); //�̺�Ʈ ��ū ��ġ ���
            ObjectTokenBase objectToken = MonoBehaviour.Instantiate(GamePlayMaster.GetInstance().testEventGO).GetComponent<ObjectTokenBase>();
            objectToken.SetToken(eventToken, TokenType.Event);
            objectToken.SyncObjectPosition(); //������Ʈ ��ü �̵� ��Ű�� 

            //2. Ÿ�Ͽ� �̺�Ʈ �ɱ�
            tile.SetEnteraceEvent(eventToken);

        }
    }

}