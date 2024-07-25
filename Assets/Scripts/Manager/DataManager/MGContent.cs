using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MGContent : Mg<MGContent>
{
    #region 변수
    private List<Quest> m_QuestList = new List<Quest>();
    List<(int, bool)> m_QuestRecorde = new(); //과거 퀘스트의 기록
    private int m_mainCharChunkNum = 0;
    private List<Chunk> m_chunkList = new List<Chunk>();
    public int m_questCount = 0;
    public const int NO_CHUNK_NUM = -1;

    public enum ContentEnum
    {
        진행턴, 발생컨텐츠
    }
    #endregion

    #region 초기화
    public MGContent()
    {
        g_instance = this;
        MGConversation converSation = new MGConversation();
    }
    public override void ReferenceSet()
    {
        MgParsing.GetInstance().GetMasterData(EMasterData.ContentData);
        TileMaker maker = MgToken.GetInstance().m_tileMaker;
        m_chunkList = maker.MakeChunk(maker.DivideChunk(MgToken.GetInstance().m_chunkLength));
        MakeNation();
    }
    #endregion

    #region 컨텐츠 연구
    public void WriteContentWhenNextTurn()
    {
        // 턴이 지났음
        CountQuestTurn(); //기존에 있던 퀘스트들 턴 감소
        List<Chunk> questChunk = SelectChunkList(3);
        Quest newQuest = SelectContent(); //새로 추가할 컨텐츠 있는지 체크 
        RealizeQuest(newQuest);
        RefreshQuestList();
    }

    public void WriteContentWhenCharAction(TokenChar _doChar, TokenAction _doAction)
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

    #endregion

    #region 퀘스트 생성
    private List<Chunk> SelectChunkList(int _newQuestCount)
    {
        //퀘스트를 수행할 청크 뽑기
        List<int> ranChunkIdx = GameUtil.GetRandomNum(m_chunkList.Count, _newQuestCount);
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
        ContentData contentMasterData = MgMasterData.GetInstance().GetContentData(playTime);
        if(contentMasterData == null)
        {
          //  Debug.Log("준비된 퀘스트 없음");
            return null;
        }
            
        if(playTime == 1)
        {
            //어디 청크에서 발현시킬지는 따로 산출
            m_mainCharChunkNum = GameUtil.GetMainCharChunkNum();
           // return MakeQuest(EQuestType.SpawnMonster, ERewardType.CharStat, m_mainCharChunkNum);
            return MakeQuest(contentMasterData, m_mainCharChunkNum); //이벤트 만들던 기존 루트
        }

        if (data.PlayTime % 3 == 0)
        {
            int ranChunkNum = Random.Range(0, m_chunkList.Count);
            return MakeQuest( contentMasterData, ranChunkNum);
            //  return MakeQuest(ranChunkNum, 6, 1, EOrderType.CharStat); //몬스터 만들기 기존 루트
        }

        return null;
    }

    private Quest MakeQuest(ContentData _contentMasterData, int _chunkNum)
    {
        Quest newQuest = new Quest(_contentMasterData, _chunkNum);
        m_questCount += 1;
        m_QuestList.Add(newQuest); //리스트에 추가 
  
        //발현시킬 구역 청크

        Chunk chunk = m_chunkList[_chunkNum];
        chunk.m_Quest = newQuest;

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
        chunk.MakePin();

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
        OrderExcutor orderExcutor = new();
      //  orderExcutor.ExcuteOrder();

    }
    private void GivePenalty(Quest _quest)
    {

    }
    private void RecordeQuest(Quest _quest, bool _result)
    {
        m_QuestRecorde.Add((_quest.QuestPid, _result));
    }

    private void CountQuestTurn()
    {
        for (int i = 0; i < m_QuestList.Count; i++)
        {
            m_QuestList[i].FlowTurn();
        }
    }

    public void RemoveQuest(Quest _quest)
    {
        //퀘스트 관련 마지막 정리 
        //1. 퀘스트 발휘된 구역이 있으면 구역 정리
        if (GetChunk(_quest.ChunkNum) != null)
            GetChunk(_quest.ChunkNum).ResetQuest();
        //2. 퀘스트 관련 object들 정리
        _quest.CleanQuest();
        m_QuestList.Remove(_quest);
        RefreshQuestList();
    }

    public List<Quest> GetQuestList()
    {
        return m_QuestList;
    }

    private void RefreshQuestList()
    {
        MgUI.GetInstance().RefreshQuestList();
    }
    #endregion

    public bool IsContentDone(int _contentPId)
    {
        if (m_QuestRecorde.IndexOf((_contentPId, true)) < 0)
            return false;

        return true;
    }

    private void MakeNation()
    {
        // 국가 매니저 초기화
        MgNation mgNation = new();

        //1. 생성할 국가 수를 뽑는다
        int nationCount = 3;
        //2. 국가 수 만큼 청크를 뽑는다.
        List<int> randomIdx = GameUtil.GetRandomNum(m_chunkList.Count, nationCount);
        //3. 청크 내에서 적당한 타일을 수도 타일로 바꾼다. 
        for (int i = 0; i < nationCount; i++)
        {
            //만들 구역 넘버
            int chunkNum = randomIdx[i];
            Chunk chunk = GetChunk(chunkNum);
            int chunkTileCount = chunk.GetTileCount();
            //구역에서 만들 타일 위치 뽑기
            int randomTile = Random.Range(0, chunkTileCount);
            TokenTile capitalTile = chunk.GetTileByIndex(randomTile);
            //Debug.Log(chunkNum + "번 구역의 타일수는 " + chunkTileCount + "그 중 " + randomTile + "번째 타일을 수도화");
            //해당 구역 수도로 변환
            capitalTile.ChangeTileType(TileType.Capital);
            //새로운 국가 생성
            MgNation.GetInstance().MakeNation(capitalTile, i);
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


