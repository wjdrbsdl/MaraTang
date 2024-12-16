using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum ContentEnum
{
    WorldTurnMatch, Clear, ClearCount, GameState, GameOver
}

public enum DevilProgress
{
    Base, Enforce1, Enforce2, Enforce3, Enforce4, Corruption
}

public class MGContent : Mg<MGContent>
{
    #region 변수
    [JsonProperty] private List<Quest> m_QuestList = new List<Quest>();
    [JsonProperty] List<(int, bool)> m_QuestRecorde = new(); //과거 퀘스트의 기록
    [JsonProperty] public DevilIncubator m_devilIncubator;
    private int m_mainCharChunkNum = 0;
    private List<Chunk> m_chunkList = new List<Chunk>();
    
    public int m_curSerialNum = 0; //컨텐츠등을 만들때마다 생성 
    [JsonIgnore] public const int NO_CHUNK_NUM = -1;
    [JsonIgnore] public int m_devilStartCount = 4;
    [JsonIgnore] public int m_nationStartCount = 3;

    #endregion

    #region 초기화
    public MGContent()
    {
        g_instance = this;
        new MGConversation(); //싱글톤 초기화
        new MgGodBless(); //싱글톤 초기화
        new ComplainManager(); //싱글톤
        new DropItemManager(); //싱글톤
        m_devilIncubator = new DevilIncubator();
    }

    public override void ReferenceSet()
    {
        MgParsing.GetInstance().GetMasterData(EMasterData.ContentData);
        MgGodBless.GetInstance().ReferenceSet();
        ComplainManager.GetInstance().ReferenceSet();
        TileMaker maker = MgToken.GetInstance().m_tileMaker;
        m_chunkList = maker.MakeChunk(maker.DivideChunk(MgToken.GetInstance().m_chunkLength));
        MakeNationDevilRegion();

    }

    public void LoadGame(MgContentJson _json)
    {
        m_devilIncubator = _json.devilIncubator;
        m_QuestRecorde = _json.QuestRecorde;
        TileMaker maker = MgToken.GetInstance().m_tileMaker;
        m_chunkList = maker.MakeChunk(maker.DivideChunk(MgToken.GetInstance().m_chunkLength));
    }

    #endregion

    #region 컨텐츠 연구
    public void WriteContentWhenNextTurn()
    {
        // 턴이 지났음
        CountQuestTurn(); //기존에 있던 퀘스트들 턴 감소
        m_devilIncubator.ChangeWorldTurn();
        WriteQuest();
        WriteChunkContent();
    }

    private void WriteQuest()
    {
        List<Quest> questList = SelectContent(); //새로 추가할 컨텐츠 있는지 체크 
        for (int i = 0; i < questList.Count; i++)
        {
            RealizeQuest(questList[i]); //컨텐츠 추가시 
        }
        
        RefreshQuestList();
    }

    private int ChunkContentCount = 3; 
    private void WriteChunkContent()
    {
        return;
        //0. 컨텐츠를 새로 뽑을 타이밍인지 체크
        if (IsTimeNewChunkContent() == false)
            return;
        //1. 컨텐츠 구현할 청크갯수 를 정함 
        int newChunkCount = ChunkContentCount;
        //2. 무슨 컨텐츠를 구현할지 비율로 정함 M, C 1:1 , 보너스 특수 S 추가 획득
        List<ChunkContentType> contentType = new();
        //임시로 몬스터 3개 할당
        contentType.Add(ChunkContentType.Monster);
        contentType.Add(ChunkContentType.Monster);
        contentType.Add(ChunkContentType.Monster);

        List<int> randomNum = GameUtil.GetRandomNum(m_chunkList.Count, contentType.Count);
        for (int i = 0; i < randomNum.Count; i++)
        {
            //3. 뽑힌순서대로 구역 확인
            Chunk chunk = m_chunkList[randomNum[i]];
            //4. 컨텐츠 구현가능한지 확인
            ChunkContent content = new ChunkContent(MgMasterData.GetInstance().GetChunkContent(1));
            //5. 일단 그냥 컨텐츠 구현
            chunk.RealizeContent(content);
        } 
    }

    #endregion

    #region 퀘스트 생성
 

    public Quest RequestGuildQuest()
    {
        Dictionary<int, ContentMasterData> contentDic = MgMasterData.GetInstance().GetContentDataDic();
        int guildQuestId = 3;
        int tempChunkNum = 1;
        if (contentDic.ContainsKey(guildQuestId))
        {
            ContentMasterData guildQuest = contentDic[guildQuestId];
            return new Quest(guildQuest, tempChunkNum);
        }

        return null;
    }

    private List<Quest> SelectContent()
    {
        //존재하는 모든 컨텐츠들의 발동조건을 따져서 수행 

        List<Quest> newQuestList = new();
        Dictionary<int, ContentMasterData> contentDic = MgMasterData.GetInstance().GetContentDataDic();
        foreach(KeyValuePair<int, ContentMasterData> pair in contentDic)
        {
            ContentMasterData curContent = pair.Value;
            bool repeat = curContent.AbleRepeat;
            if (IsContentDone(curContent.ContentPid))
            {
             //   Debug.Log(curContent.ContentPid + "했던 컨텐츠는 패쓰");
                continue;
            }
            if (IsPlayingContent(curContent.ContentPid))
            {
              //  Debug.Log(curContent.ContentPid + "진행중인 컨텐츠는 패쓰");
                continue;
            }
            //모든 컨텐츠의 발동조건을 살핌. 
            if (IsSatisfyAct(curContent.ActConditionList))
            {
                //만약 발동조건이 충족한 컨텐츠가 발견되면 
                int tempChunkNum = 1;
                newQuestList.Add( new Quest(curContent, tempChunkNum));
            }
        }

        return newQuestList;
    }

    private bool IsSatisfyAct(List<TOrderItem> _conditionList)
    {
        //컨텐츠 발동 조건에 충족하는지 체크
        for (int i = 0; i < _conditionList.Count; i++)
        {
            TOrderItem conditionInfo = _conditionList[i]; 
            TokenType condtionType = conditionInfo.Tokentype;
            switch (condtionType)
            {
                //각 케이스별로 조건 체크 
                case TokenType.Content:
                    ContentEnum contentCase = (ContentEnum)conditionInfo.SubIdx;
                    bool isOk = IsSatisfyCase(contentCase, conditionInfo.Value);
                    if(isOk == false)
                    {
                        return false;
                    }
                    break;

                    //잘못된 케이스 데이터인 경우 실행되지 않도록 세팅 
                default:
                    return false;

            }

        }

        return true;
    }

    private bool IsSatisfyCase(ContentEnum _contentCase, int _value)
    {
       // Debug.Log(_contentCase + "가 " + _value.ToString() + " 한지 컨텐츠 케이스 조건 확인");
        switch (_contentCase)
        {
            case ContentEnum.WorldTurnMatch:
                GamePlayData data = GamePlayMaster.GetInstance().GetPlayData();
                int playTime = data.PlayTime;
                if(playTime == _value)
                {
                    return true;
                }
                break;
            case ContentEnum.Clear:
                //벨류는 컨텐츠pid
             //   Debug.Log(_value + "클리어 했는지 체크"+ IsContentDone(_value));
                if (IsContentClear(_value))
                {
                    return true;
                }
                break;
        }
        return false;
    }

    #endregion

    #region 퀘스트 관리
    public void RealizeQuest(Quest _quest)
    {
        if (_quest == null)
            return;
        _quest.RealizeStage();
        Chunk chunk = m_chunkList[_quest.ChunkNum];
        chunk.MakePin();
        m_QuestList.Add(_quest);
    }

    public void SuccessQuest(Quest _quest)
    {
        RecordeQuest(_quest, true);
        RemoveQuest(_quest);
        CheckNextQuest();
    }

    public void FailQuest(Quest _quest)
    {
        RecordeQuest(_quest, false);
        RemoveQuest(_quest);
        CheckNextQuest();
    }

    private void CheckNextQuest()
    {
        //성공이나 실패 컨텐츠 상태값이 바뀐경우 연계 되는 퀘스트가 있는지 보고 실행하는 용도. 
        List<Quest> questList = SelectContent(); //새로 추가할 컨텐츠 있는지 체크 
        for (int i = 0; i < questList.Count; i++)
        {
            RealizeQuest(questList[i]); //컨텐츠 추가시 
        }
        RefreshQuestList();
    }

    private void RecordeQuest(Quest _quest, bool _result)
    {
        //   Debug.Log(_quest.ContentPid + "번 컨텐츠 성공 여부 :"+_result);
        m_devilIncubator.ChangeWorldContent(_quest.ContentPid, _result);
        m_QuestRecorde.Add((_quest.ContentPid, _result));
    }

    private void CountQuestTurn()
    {
        for (int i = 0; i < m_QuestList.Count; i++)
        {
            m_QuestList[i].FlowTurn();
        }
    }

    private void RemoveQuest(Quest _quest)
    {
        //퀘스트 관련 마지막 정리 
        //1. 퀘스트 발휘된 구역이 있으면 구역 정리
        if (GetChunk(_quest.ChunkNum) != null)
            GetChunk(_quest.ChunkNum).ResetQuest();
        //2. 퀘스트 관련 object들 정리
        m_QuestList.Remove(_quest);
        RefreshQuestList();
    }

    public List<Quest> GetQuestList()
    {
        return m_QuestList;
    }

    public List<(int, bool)> GetRecord()
    {
        return m_QuestRecorde;
    }

    private void RefreshQuestList()
    {
        MgUI.GetInstance().RefreshQuestList();
    }
 
    public int GetSerialNum()
    {
        //
        int serial = m_curSerialNum;
        m_curSerialNum += 1;
        return serial;
    }

    public bool IsContentDone(int _contentPId)
    {
        //해당 컨텐츠를 해봤능가 
        for (int i = 0; i < m_QuestRecorde.Count; i++)
        {
            (int, bool) recorde = m_QuestRecorde[i];
            if (recorde.Item1 == _contentPId)
            {
                return true;
            }
        }
        //한적 없는 컨텐트는 false
        return false;
    }

    public bool IsContentClear(int _contentPid)
    {
        for (int i = 0; i < m_QuestRecorde.Count; i++)
        {
            (int, bool) recorde = m_QuestRecorde[i];
            if(recorde.Item1 == _contentPid)
            {
                return recorde.Item2;
            }
        }
        //한적 없는 컨텐트는 false
        return false;
    }

    public bool IsPlayingContent(int _cotentPid)
    {
        for (int i = 0; i < m_QuestList.Count; i++)
        {
            int cotentpid = m_QuestList[i].ContentPid;
            if(_cotentPid == cotentpid)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region 국가데빌 지역 설정
    private void MakeNationDevilRegion()
    {
        //1. 사용할 수만큼 겹치지 않도록 Chunk 인덱스 추출
        List<int> randomNationChunk = GameUtil.GetRandomNum(m_chunkList.Count, m_devilStartCount + m_nationStartCount);
        //2. 각추출된 구역을 가지고 국가, 악마 봉인지역 생성
        MakeNation(randomNationChunk, 0, m_nationStartCount);
        MakeDevilList(randomNationChunk, m_nationStartCount-1, m_devilStartCount);
    }

    private void MakeNation(List<int> _randomIdx, int _startIdx, int _count)
    {
        // 국가 매니저 초기화
        MgNation mgNation = new();

        //2. 청크 내에서 적당한 타일을 수도 타일로 바꾼다. 
        for (int i = _startIdx; i < _startIdx + _count; i++)
        {
            //만들 구역 넘버
            int chunkNum = _randomIdx[i];
            Chunk chunk = GetChunk(chunkNum);
            int chunkTileCount = chunk.GetTileCount();
            //구역에서 만들 타일 위치 뽑기
            int randomTile = Random.Range(0, chunkTileCount);
            TokenTile capitalTile = chunk.GetTileByIndex(randomTile);
            //Debug.Log(chunkNum + "번 구역의 타일수는 " + chunkTileCount + "그 중 " + randomTile + "번째 타일을 수도화");
            //해당 구역 수도로 변환
            capitalTile.ChangePlace(TileType.Capital);
            //새로운 국가 생성
            MgNation.GetInstance().MakeNation(capitalTile, i, (GodClassEnum)(i+1));
        }
    }

    private void MakeDevilList(List<int> _randomIdx, int _startIdx, int _count)
    {
        List<int> chunkNumList = new List<int>();
        List<int[]> tileList = new List<int[]>();
        for (int i = _startIdx; i < _startIdx + _count; i++)
        {
            //만들 구역 넘버
            int chunkNum = _randomIdx[i];
            Chunk chunk = GetChunk(chunkNum);
            int chunkTileCount = chunk.GetTileCount();
            //구역에서 만들 타일 위치 뽑기
            int randomTile = Random.Range(0, chunkTileCount);
            TokenTile devilStartTile = chunk.GetTileByIndex(randomTile);

            chunkNumList.Add(chunkNum);
            tileList.Add(devilStartTile.GetMapIndex());

        }
        //구역과 타일을 전달
        m_devilIncubator.SetBirthRegion(chunkNumList, tileList);
        //악마 뽑기 호출
        m_devilIncubator.DiceDevilList(_count);
    }
    #endregion

    #region 구역 컨텐츠 
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

    private int periode = 5;
    private bool IsTimeNewChunkContent()
    {
        int curTurn = GamePlayMaster.GetInstance().GetPlayData().PlayTime;
        return curTurn % periode == 0;
    }

    public Chunk GetChunk(int _chunkNum)
    {
        //청크리스트가 널이거나 idx넘버가 범위 밖이라면 null 반환
        if (m_chunkList == null || _chunkNum < 0 || m_chunkList.Count <= _chunkNum)
            return null;

        return m_chunkList[_chunkNum];
    }
    #endregion
    public void SendActionCode(TOrderItem _orderItem, int _serialNum = FixedValue.No_VALUE)
    {
        //플레이어 액션 후 결과물을 보고
        //결과물 따라서 
        //1. 새로운 컨텐츠 조건 해방으로 추가될 컨텐츠가 있는지
        //2. 수행중인 퀘스트에 어떤영향을 미치는지 판단 
        //* 현재 있는 3개 값만으로 분류가 불가해질경우, 추가 변수 설정이 필요. 
        // Debug.LogFormat("{0}번 타입 {1}서브 {2} 벨류 액션 전달", _orderItem.Tokentype, _orderItem.SubIdx, _orderItem.Value);
        //퀘스트 클리어 여부 체크
        Quest[] curQuest = m_QuestList.ToArray();
        for (int i = 0; i < curQuest.Length; i++)
        {
            Quest quest = curQuest[i];

           // Debug.Log("시리얼 넘버 " + serialNum + "현재 컨텐츠 시리얼" + quest.SerialNum);
            //시리얼 넘버가 존재하는 코드라면, 퀘스트 시리얼 넘버가 일치해야 체크
            if (_serialNum != FixedValue.No_VALUE && _serialNum != quest.SerialNum)
                continue;

            quest.CurStageData.AdaptCondtion(_orderItem); //각 퀘스트에 새로운 상태를 적용하고
            bool isCompelete = quest.CurStageData.CheckSuccess(); //해당 퀘스트의 스테이지가 클리어되었는지 확인
            bool isFail = quest.CurStageData.CheckFail();
           // Debug.LogFormat("퀘스트 고유 넘버{0}, 행동 고유넘버{5}, 퀘Pid{4}, {1}스테이지 클리어 여부 성공{2} 실패{3}", quest.SerialNum, quest.CurStep, isCompelete, isFail, quest.ContentPid, _serialNum);
            if (isCompelete)
            {
                bool isAutoClear = quest.CurStageData.AutoClear;
                if(isAutoClear)
                quest.ClearStage();
                else
                {
                    Debug.Log("플레이어 요청으로 클리어 가능");
                }
            }
            
            else if (isFail)
                quest.FailStage();
        }

    }
}


