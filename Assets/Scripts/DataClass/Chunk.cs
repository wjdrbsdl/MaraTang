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
        //수량 임시로 3
        for (int i = 0; i < 3; i++)
        {
            int tempPid = i;
            int tempValue = i + 1;

            //0. 생성할 땅 찾고
            int x = 0;
            int y = i;
            TokenTile tile = tiles[x, y];

            MgToken.GetInstance().SpawnEvent(tile, tempPid);
        }
    }

    public void MakeMonsterToken()
    {
        //룰북에서 할까?
        MgMasterData masterData = MgMasterData.GetInstance();
        QuestCondition condition = Quest.Condition;
        for (int i = 0; i < condition.monsterCount; i++)
        {
            //위치 잡는 코드 필요 일단은 임시로 
            int tempSpawnX = 0;
            int tempSpawnY = i % 5;
            int[] spawnCoord = tiles[tempSpawnX, tempSpawnY].GetMapIndex();
            if(!masterData.CheckPID(EMasterData.CharData, condition.monsterPID))
            {
                //만약 몬스터 피아이디가 없는거라면 딴걸로 변경
                condition.monsterPID = 2;
            }
            TokenChar questMonster = MgToken.GetInstance().SpawnMonster(spawnCoord, condition.monsterPID); //몬스터의 경우 사망시에 설치
            questMonster.QuestCard = Quest;
            Quest.TempQuestTokens.Add(questMonster);
        }
    }

    public void MakePin()
    {
        NaviPin naviPin = MgNaviPin.GetInstance().GetNaviPin();
        naviPin.PutPin(tiles[2, 2].GetObject().transform.position);
    }

    public void OnEnterChunk()
    {
        //해당 청크에 들어온 순간
        Debug.Log(ChunkNum + "에 입장");
    }

    public void OnExitChunk()
    {
        //해당 청크에서 나간 순간 
        Debug.Log(ChunkNum + "에서 나감");
    }
}