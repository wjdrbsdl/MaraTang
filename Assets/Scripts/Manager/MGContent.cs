using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MGContent : Mg<MGContent>
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
    private List<Quest> m_QuestList = new List<Quest>();
    List<(int, bool)> m_QuestRecorde = new(); //과거 퀘스트의 기록
    private int m_mainCharChunkNum = 0;
    private List<Chunk> m_chunkList = new List<Chunk>();

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
    public override void ReferenceSet()
    {
        MgParsing.GetInstance().GetMasterData(EMasterData.ContentData);
        TileMaker maker = MgToken.GetInstance().m_tileMaker;
        m_chunkList = maker.MakeChunk(maker.DivideChunk(MgToken.GetInstance().m_chunkLength));
        RandomDye();
        MakeFence();
    }
    #endregion

    public void OnNextTurn()
    {
        // 턴이 지났음
        CountQuestTurn(); //기존에 있던 퀘스트들 턴 감소
        List<Chunk> questChunk = SelectChunk(3);
        Quest newQuest = SelectContent(); //새로 추가할 컨텐츠 있는지 체크 
        RealizeQuest(newQuest);
    }

    public void OnCharAction(TokenChar _doChar, TokenAction _doAction)
    {
        //캐릭이 액션을 수행할때마다 알림 받음 
        if (_doChar.isMainChar == false)
            return;

        if (_doAction.GetActionType().Equals(ActionType.Move))
        {
            int moveChunk = GameUtil.GetChunkNum(_doChar.GetMapIndex());
            if (m_mainCharChunkNum.Equals(moveChunk) == false)
            {
                //다른 구역으로 넘어갔을때 
                m_chunkList[m_mainCharChunkNum].OnExitChunk(); //이전건 나간거
                m_chunkList[moveChunk].OnEnterChunk(); //새로운건 들어간거
            }
            m_mainCharChunkNum = moveChunk;

        }
    }

    private void CountQuestTurn()
    {
        for (int i = 0; i < m_QuestList.Count; i++)
        {
            m_QuestList[i].RemoveTurn();
        }
    }

    #region 퀘스트 생성
    private List<Chunk> SelectChunk(int _selectCount)
    {
        //퀘스트를 수행할 청크 뽑기
        List<int> ranChunkIdx = GameUtil.GetRandomNum(m_chunkList.Count, _selectCount);
        List<Chunk> rulletChunk = new();
        for (int i = 0; i < ranChunkIdx.Count; i++)
        {
            rulletChunk.Add(m_chunkList[ranChunkIdx[i]]);
         //   Debug.Log(ranChunkIdx[i] + "번째 청크");
        }

        return rulletChunk;
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
            return MakeQuest(m_mainCharChunkNum, 12, 5, RewardType.TileEvent);
        }

        if (data.PlayTime % 3 == 0)
        {
            int ranChunkNum = Random.Range(0, m_chunkList.Count);
            return MakeQuest(ranChunkNum, 6, 1, RewardType.CharStat);
        }

        return null;
    }

    private Quest MakeQuest(int _chunkNum, int _monsterPID, int _monsterCount, RewardType _rewardType)
    {
        Quest newQuest = new(_monsterPID, _monsterCount, _rewardType); //퀘스트 문서 생성 
        newQuest.ChunkNum = _chunkNum;
        m_QuestList.Add(newQuest); //리스트에 추가 
                                      //  Debug.Log("몬스터 소환 컨텐츠");

        //발현시킬 구역 청크
        m_mainCharChunkNum = GameUtil.GetChunkNum(PlayerManager.GetInstance().GetMainChar().GetMapIndex());
        Chunk chunk = m_chunkList[_chunkNum];
        chunk.Quest = newQuest;

        return newQuest;
    }
    #endregion

    #region 퀘스트 관리
    private void RealizeQuest(Quest _quest)
    {
        if (_quest == null)
            return;

        MgUI.GetInstance().ShowQuest(_quest);
        Chunk chunk = m_chunkList[_quest.ChunkNum];

        //몬스터 카운트가 있으면 몬스터 생성
        chunk.MakeMonsterToken();
        chunk.MakePin();

        //그외 조건 값들이 더있으면 또 수행 
    }

    public void SuccessQuest(Quest _quest)
    {
        GiveReward(_quest);
        RecordeQuest(_quest, true);
        RemoveQuest(_quest);
    }

    public void FailQuest(Quest _quest)
    {
        GivePenalty(_quest);
        RecordeQuest(_quest, false);
        RemoveQuest(_quest);
    }

    private void GiveReward(Quest _quest)
    {
        RewardData _reward = _quest.Reward;
        RewardType rewardType = _reward.RewardType;
        if (rewardType.Equals(RewardType.Capital))
        {
            Capital rewardCapital = (Capital)_reward.SubType;
            PlayerCapitalData.g_instance.CalCapital(rewardCapital, _reward.RewardValue);
            return;
        }
        if (rewardType.Equals(RewardType.TileEvent))
        {
            Chunk chunk = m_chunkList[_quest.ChunkNum];
            chunk.MakeEventToken();
            return;
        }
        if (rewardType.Equals(RewardType.CharStat))
        {
            MgUI.GetInstance().ShowRewardList(_quest.Reward);
            return;
        }
    }

    private void GivePenalty(Quest _quest)
    {

    }

    private void RecordeQuest(Quest _quest, bool _result)
    {
        m_QuestRecorde.Add((_quest.QuestPid, _result));
    }

    public void RemoveQuest(Quest _quest)
    {
        //퀘스트 관련 마지막 정리 
        if (GetChunk(_quest.ChunkNum) != null)
            GetChunk(_quest.ChunkNum).ResetQuest();
        m_QuestList.Remove(_quest);
    }
    #endregion

    public List<Quest> GetQuestList()
    {
        return m_QuestList;
    }

    //선택한 보상을 적용하기 위해 각기 필요한 클래스로 전달하기 
    public void SelectReward(RewardInfo _rewardInfo)
    {
        if (_rewardInfo.RewardType.Equals(RewardType.CharStat))
        {
            PlayerManager.GetInstance().GetMainChar().CalStat((CharStat)_rewardInfo.SubIdx, _rewardInfo.Value);
        }
    }

    private void RandomDye()
    {
        Color[] a = { Color.blue, Color.cyan, Color.red, Color.yellow, Color.magenta, Color.green, Color.white };
        for (int i = 0; i < m_chunkList.Count; i++)
        {
            Color b = a[i % a.Length];
            m_chunkList[i].Dye(b);
        }
    }

    private void MakeFence()
    {
        List<int> ranNum = GameUtil.GetRandomNum(m_chunkList.Count, 3);
        Sprite fenceSprite = MgToken.GetInstance().m_tilesSprite[1];
        for (int i = 0; i < ranNum.Count; i++)
        {
            Chunk chunk = m_chunkList[ranNum[i]];
            int xLength = chunk.tiles.GetLength(0);
            int yLength = chunk.tiles.GetLength(1);

            //외곽인경우만 스프라이트 바꾸기
            
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    if(x==0 || x == xLength - 1)
                    {
                        //x축이 0이거나 맨 끝인경우 y 0~max 달리고
                        chunk.tiles[x, y].SetSprite(fenceSprite);
                    }
                    else
                    {
                        //x값이 1~어느 사이인 경우엔 y 처음과 끝만 색칠하고 해당 열은 패스 
                        chunk.tiles[x, 0].SetSprite(fenceSprite);
                        chunk.tiles[x, yLength - 1].SetSprite(fenceSprite);
                        break;
                    }
                }
            }
        }
    }

    public Chunk GetChunk(int _chunkNum)
    {
        //청크리스트가 널이거나 idx넘버가 범위 밖이라면 null 반환
        if (m_chunkList == null || _chunkNum < 0 || m_chunkList.Count <= _chunkNum)
            return null;

        return m_chunkList[_chunkNum];
    }
}

