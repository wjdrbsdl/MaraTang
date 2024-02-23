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
        //���� �ӽ÷� 3
        for (int i = 0; i < 3; i++)
        {
            int tempPid = i;
            int tempValue = i + 1;

            //0. ������ �� ã��
            int x = 0;
            int y = i;
            TokenTile tile = tiles[x, y];

            MgToken.GetInstance().SpawnEvent(tile, tempPid);
        }
    }

    public void MakeMonsterToken()
    {
        //��Ͽ��� �ұ�?
        QuestCondition condition = Quest.condition;
        for (int i = 0; i < condition.monsterCount; i++)
        {
            //��ġ ��� �ڵ� �ʿ� �ϴ��� �ӽ÷� 
            int tempSpawnX = 0;
            int tempSpawnY = i % 5;
            int[] spawnCoord = tiles[tempSpawnX, tempSpawnY].GetMapIndex();
            TokenChar questMonster = MgToken.GetInstance().SpawnMonster(spawnCoord, condition.monsterPID); //������ ��� ����ÿ� ��ġ
            questMonster.QuestCard = Quest;
            Quest.TempQuestTokens.Add(questMonster);
        }
    }

}