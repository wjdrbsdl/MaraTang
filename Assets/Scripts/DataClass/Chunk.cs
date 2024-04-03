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
        MgMasterData masterData = MgMasterData.GetInstance();
        QuestCondition condition = m_Quest.Condition;
        List<int> random = GameUtil.GetRandomNum(25, 5);
        for (int i = 0; i < condition.monsterCount; i++)
        {
            //��ġ ��� �ڵ� �ʿ� �ϴ��� �ӽ÷� 
            int ranPos = random[i]; //Ÿ�Ϲ�ȣ�� �̰� �ش� ��ȣ�� Ÿ����ǥ�� ��ȯ�ؾ���
            int[] tilePos = GameUtil.GetXYPosFromIndex(tiles.GetLength(0), ranPos);
            int[] spawnCoord = tiles[tilePos[0], tilePos[1]].GetMapIndex();
            if(!masterData.CheckPID(EMasterData.CharData, condition.monsterPID))
            {
                //���� ���� �Ǿ��̵� ���°Ŷ�� ���ɷ� ����
                condition.monsterPID = 2;
            }
            TokenChar questMonster = MgToken.GetInstance().SpawnCharactor(spawnCoord, condition.monsterPID); //������ ��� ����ÿ� ��ġ
            questMonster.SetQuest(m_Quest);
            questMonster.QuestPid = m_Quest.QuestPid;
            m_Quest.TempQuestTokens.Add(questMonster);
        }
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
}