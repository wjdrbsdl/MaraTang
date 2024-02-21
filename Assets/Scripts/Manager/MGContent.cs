using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MGContent 
{
    #region 변수
    public static MGContent g_instance;
    private List<Quest> m_QuestReports = new List<Quest>();
    List<(int, bool)> m_QuestRecorde = new(); //과거 퀘스트의 기록
    private int m_mainCharChunk = 0;

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
        //진행도나 무언가에 따라서 컨텐츠 발생

        /*컨텐츠는 
         * - 해당 지역 몬스터 제거하기 
         * - 해당 지역 점거 하기 
         * - 해당 지역 자원 캐기 등 특정한 이벤트를 발생시키고, 그에 따라 보상을 약속하는 시스템 
         * 
        */

        //테스트용, 3턴 마다 특정 청크 n개에 몬스터 발생 
        //기존 발생되었던 녀석은 어떻게? 
        //1. 기존께 강화
        //2. 기존께 소멸
        //3. 기존꺼 유지 
        Debug.Log("컨텐츠 활성화");
        CountQuestTurn(); //기존에 있던 퀘스트들 턴 감소
        SelectContent(); //새로 추가할 컨텐츠 있는지 체크 
    }

    public void AlarmAction(TokenChar _doChar, TokenAction _doAction)
    {
        //캐릭이 액션을 수행할때마다 알림 받음 
        if (_doChar.isMainChar == false)
            return;

        if (_doAction.GetActionType().Equals(ActionType.Move))
        {
            int moveChunk = GameUtil.GetChunkNum(_doChar.GetMapIndex());
            if (m_mainCharChunk.Equals(moveChunk) == false)
            {
                //다른 청크로 이동한거면
             //   Debug.Log(moveChunk + "번 청크 퀘스트 수행");
                
            }
            m_mainCharChunk = moveChunk;
        }
    }

    private void CountQuestTurn()
    {
        for (int i = 0; i < m_QuestReports.Count; i++)
        {
            m_QuestReports[i].RemoveTurn();
        }
    }

    private void SelectContent()
    {
        //미리 세팅해둔 컨텐츠 테이블에 따라 수행할것

        //임시로 3턴 마다 몬스터 퀘스트 수행하도록 
        GamePlayData data = GamePlayMaster.GetInstance().GetPlayData();
        int playTime = data.PlayTime;

        if(playTime == 1)
        {
            TutorialQuest(1);
            return;
        }

        if (data.PlayTime % 3 == 0)
            MonsterQuest(1);
    }

    private void TutorialQuest(int _monsterPID)
    {
        //스폰 시킨몬스터를 퀘스트 몬스터로
        Quest newQuest = new(); //퀘스트 문서 생성 
        m_QuestReports.Add(newQuest); //리스트에 추가 
                                      //  Debug.Log("몬스터 소환 컨텐츠");

        m_mainCharChunk = GameUtil.GetChunkNum(PlayerManager.GetInstance().GetMainChar().GetMapIndex());
        Chunk chunk = MgToken.GetInstance().ChunkList[m_mainCharChunk]; //플레이어가 있는 청크로 결정
        int width = chunk.tiles.GetLength(0);
        int height = chunk.tiles.GetLength(1);
        int tileCount =  width * height;
        for (int i = 0; i < 5; i++)
        {
            int[] spawnCoord = chunk.tiles[0, i].GetMapIndex();
            TokenChar questMonster = MgToken.GetInstance().SpawnMonster(spawnCoord, _monsterPID); //몬스터의 경우 사망시에 설치
            questMonster.QuestCard = newQuest;
            newQuest.TempQuestTokens.Add(questMonster);
            chunk.Quest = newQuest;
        }
        //컨텐츠 이벤트 등록은 여기서 따로 시행
    }

    private void MonsterQuest(int _monsterPID)
    {
        //스폰 시킨몬스터를 퀘스트 몬스터로
        Quest newQuest = new(); //퀘스트 문서 생성 
        m_QuestReports.Add(newQuest); //리스트에 추가 
      //  Debug.Log("몬스터 소환 컨텐츠");

        m_mainCharChunk = GameUtil.GetChunkNum(PlayerManager.GetInstance().GetMainChar().GetMapIndex());

        for (int i = 0; i < MgToken.GetInstance().ChunkList.Count; i++)
        {
            //플레이어와 같은 청크는 패스. 
            if (i.Equals(m_mainCharChunk))
                continue;

            Chunk chunk = MgToken.GetInstance().ChunkList[i];
            //각 청크마다 몬스터 소환
            int[] spawnCoord = chunk.tiles[0, 0].GetMapIndex();
            TokenChar questMonster = MgToken.GetInstance().SpawnMonster(spawnCoord, _monsterPID); //몬스터의 경우 사망시에 설치
            questMonster.QuestCard = newQuest;
            chunk.Quest = newQuest;
        }
        //컨텐츠 이벤트 등록은 여기서 따로 시행

    }

    private void RecordQuest(Quest _quest)
    {
        int questPid = _quest.QuestPid;
        bool questSucces = true;
        m_QuestRecorde.Add((questPid, questSucces)); 
    }

    private void QuestReward()
    {
        //1.최근에 A 퀘스트를 완료했다. 
        //2. A가 들어가는 퀘스트의 조건을 확인한다. 
        //3. 보상을 지급한다 
        //4. 퀘스트의 조합을 관리하는 퀘스트? 
    }
}

