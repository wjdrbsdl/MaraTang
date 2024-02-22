using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MGContent
{
    /*
     * 게임에 컨텐츠를 풀어 놓는 곳 
     * 1. 어느 컨텐츠를 풀지 정하고 
     * 2. 그 컨텐츠 타입에 맞는 퀘스트를 생성하고 
     * 3. 어디에 풀지 구역을 정한뒤 
     * 4. 퀘스트를 수행
     * -------------------------------------------------------------
     * 1. 어느 컨텐츠를 할지 정하고 - 몇개, 어느 타입
     * 2. 퀘스트 수행할 청크를 그 수만큼 뽑고 
     * 3. 해당 청크 내부에서 전달받은 타입으로 퀘스트를 생성하고
     * 4. 퀘스트를 수행 
     * --------------------------------------------------------------
     *  //진행도나 무언가에 따라서 컨텐츠 발생

        /*컨텐츠는 
         * - 해당 지역 몬스터 제거하기 
         * - 해당 지역 점거 하기 
         * - 해당 지역 자원 캐기 등 특정한 이벤트를 발생시키고, 그에 따라 보상을 약속하는 시스템 
         * 
     

        //기존 발생되었던 녀석은 어떻게? 
        //1. 기존께 강화
        //2. 기존께 소멸
        //3. 기존꺼 유지 
     */
    #region 변수
    public static MGContent g_instance;
    private List<Quest> m_QuestReports = new List<Quest>();
    List<(int, bool)> m_QuestRecorde = new(); //과거 퀘스트의 기록
    private int m_mainCharChunkNum = 0;

    public enum ContentEnum
    {
        진행턴, 발생컨텐츠
    }
    #endregion

    #region 초기화
    public MGContent()
    {
        g_instance = this;
    }
    public void ReferenceSet()
    {
        MgParsing.GetInstance().GetMasterData(EMasterData.ContentData);
   
    }
    #endregion

    public void MakeContent()
    {
       
       // Debug.Log("컨텐츠 활성화");
        CountQuestTurn(); //기존에 있던 퀘스트들 턴 감소
        Quest newQuest = SelectContent(); //새로 추가할 컨텐츠 있는지 체크 
        RealizeQuest(newQuest);
    }

    public void AlarmAction(TokenChar _doChar, TokenAction _doAction)
    {
        //캐릭이 액션을 수행할때마다 알림 받음 
        if (_doChar.isMainChar == false)
            return;

        if (_doAction.GetActionType().Equals(ActionType.Move))
        {
            int moveChunk = GameUtil.GetChunkNum(_doChar.GetMapIndex());
            if (m_mainCharChunkNum.Equals(moveChunk) == false)
            {
                //다른 청크로 이동한거면
             //   Debug.Log(moveChunk + "번 청크 퀘스트 수행");
                
            }
            m_mainCharChunkNum = moveChunk;
        }
    }

    private void CountQuestTurn()
    {
        for (int i = 0; i < m_QuestReports.Count; i++)
        {
            m_QuestReports[i].RemoveTurn();
        }
    }

    private Quest SelectContent()
    {
        //미리 세팅해둔 컨텐츠 테이블에 따라 수행할것

        //임시로 3턴 마다 몬스터 퀘스트 수행하도록 
        GamePlayData data = GamePlayMaster.GetInstance().GetPlayData();
        int playTime = data.PlayTime;

        if(playTime == 1)
        {
            //어디 청크에서 발현시킬지는 따로 산출
            m_mainCharChunkNum = GameUtil.GetChunkNum(PlayerManager.GetInstance().GetMainChar().GetMapIndex());
            return MakeQuest(m_mainCharChunkNum, 1, 5, RewardType.TileEvent);
        }

        if (data.PlayTime % 3 == 0)
        {
            return MakeQuest(m_mainCharChunkNum, 1, 1, RewardType.None);
        }

        return null;
    }

    private Quest MakeQuest(int _chunkNum, int _monsterPID, int _monsterCount, RewardType _rewardType)
    {
        Quest newQuest = new(_monsterPID, _monsterCount, RewardType.TileEvent); //퀘스트 문서 생성 
        newQuest.ChunkNum = _chunkNum;
        m_QuestReports.Add(newQuest); //리스트에 추가 
                                      //  Debug.Log("몬스터 소환 컨텐츠");

        //발현시킬 구역 청크
        m_mainCharChunkNum = GameUtil.GetChunkNum(PlayerManager.GetInstance().GetMainChar().GetMapIndex());
        Chunk chunk = MgToken.GetInstance().ChunkList[_chunkNum]; //플레이어가 있는 청크로 결정
        chunk.Quest = newQuest;

        return newQuest;
    }

    private void RealizeQuest(Quest _quest)
    {
        if (_quest == null)
            return;

        MgUI.GetInstance().ShowQuest(_quest);

        //룰북에서 할까?
        QuestCondition condition = _quest.condition;
        Chunk chunk = MgToken.GetInstance().ChunkList[_quest.ChunkNum];
        //몬스터 카운트가 있으면 몬스터 생성
        for (int i = 0; i < condition.monsterCount; i++)
        {
            //위치 잡는 코드 필요 일단은 임시로 
            int tempSpawnX = 0;
            int tempSpawnY = i%5;
            int[] spawnCoord = chunk.tiles[tempSpawnX, tempSpawnY].GetMapIndex();
            TokenChar questMonster = MgToken.GetInstance().SpawnMonster(spawnCoord, condition.monsterPID); //몬스터의 경우 사망시에 설치
            questMonster.QuestCard = _quest;
            _quest.TempQuestTokens.Add(questMonster);
        }
        //그외 조건 값들이 더있으면 또 수행 
    }

}

