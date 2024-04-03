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
        QuestCondition condition = m_Quest.Condition;
        List<int> random = GameUtil.GetRandomNum(25, 5);
        for (int i = 0; i < condition.monsterCount; i++)
        {
            //위치 잡는 코드 필요 일단은 임시로 
            int ranPos = random[i]; //타일번호를 뽑고 해당 번호를 타일좌표로 반환해야함
            int[] tilePos = GameUtil.GetXYPosFromIndex(tiles.GetLength(0), ranPos);
            int[] spawnCoord = tiles[tilePos[0], tilePos[1]].GetMapIndex();
            if(!masterData.CheckPID(EMasterData.CharData, condition.monsterPID))
            {
                //만약 몬스터 피아이디가 없는거라면 딴걸로 변경
                condition.monsterPID = 2;
            }
            TokenChar questMonster = MgToken.GetInstance().SpawnCharactor(spawnCoord, condition.monsterPID); //몬스터의 경우 사망시에 설치
            questMonster.SetQuest(m_Quest);
            questMonster.QuestPid = m_Quest.QuestPid;
            m_Quest.TempQuestTokens.Add(questMonster);
        }
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
}