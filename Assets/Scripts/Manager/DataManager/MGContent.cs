﻿using System.Collections;
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
    private int[,] m_chunkPos;

    public int m_curSerialNum = 0; //컨텐츠등을 만들때마다 생성 
    public DevilProgress curDevilLevel = DevilProgress.Enforce3;
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
        DropItemManager.GetInstance().ReferenceSet();
        m_chunkList = maker.MakeChunk(maker.DivideChunk(MgToken.GetInstance().m_chunkLength));
        MakeChunkPos();
        GetChunkByRange(4, 1);
        GetChunkByRange(4, 2);
        GetChunkByRange(4, 9);
        GetChunkByRange(4, 10);
        MakeNationDevilRegion();
        MakeChunkCore();
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
        CountChunkTurn(); //구역컨텐츠 턴 감소
        m_devilIncubator.ChangeWorldTurn();
        WriteQuest();
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

            //1.구역에서 만들 타일 위치 뽑기
            int chunkTileCount = chunk.GetTileCount();
            int randomTile = Random.Range(0, chunkTileCount);
            TokenTile capitalTile = chunk.GetTileByIndex(randomTile);
            //Debug.Log(chunkNum + "번 구역의 타일수는 " + chunkTileCount + "그 중 " + randomTile + "번째 타일을 수도화");
            //2.해당 구역 수도로 변환
            capitalTile.ChangePlace(TileType.Capital);
            //3.새로운 국가 생성
            MgNation.GetInstance().MakeNation(capitalTile, i, (GodClassEnum)(i+1));
            //4. 국가 구역으로 지정
            chunk.IsNation = true;
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

        //이번 게임에 등장할 악마 뽑기 호출
        m_devilIncubator.DiceDevilList(_count);
        //악마가 봉인된 구역과 타일을 전달하여 해당 타일을 봉인지역으로 바꾸기
        m_devilIncubator.SetBirthRegion(chunkNumList, tileList);
        
    }

    private void MakeChunkCore()
    {
        for (int i = 0; i < m_chunkList.Count; i++)
        {
            Chunk chunk = m_chunkList[i];
            if(chunk.IsNation == true)
            {
             //   Debug.Log("국가 지역 핵 생성 안함");
                continue;
            }

            chunk.MakeCore();
        }
    }
    #endregion

    #region 구역 컨텐츠 

    private void CountChunkTurn()
    {
        for (int i = 0; i < m_chunkList.Count; i++)
        {
            m_chunkList[i].CountContentTurn();
        }
    }

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

    public TOrderItem GetComplaintChunkItem(out int chunkNum)
    {
        //가능한 청크를 반환
        List<int> randomIdx = GameUtil.GetRandomNum(m_chunkList.Count, m_chunkList.Count);

        for (int i = 0; i < m_chunkList.Count; i++)
        {
           // Debug.Log(randomIdx[i] + "구역에 민원 아이템 확인");
            Chunk chunk = m_chunkList[randomIdx[i]];
            if (chunk.PreContent == null)
                continue;
            List<TokenBase> madeList = chunk.PreContent.MadeList;
            //해당 구역이 무슨 내용인지는 모르겠는데 무튼 생성된 재료가
            //타일이면 거기서 나는 채집 아이템을
            //몬스터면 해당 몬스터 사냥을 목표로 함
            for (int x = 0; x < madeList.Count; x++)
            {
                TokenBase item = madeList[x];
                if (item.GetTokenType().Equals(TokenType.Tile))
                {
                    //해당 타일에서 나는 재료를 훑어 보면서 특정 capital인 경우 그 아이템을 요구
                    List<TOrderItem> tileItemList = MgMasterData.GetInstance().GetTileData(item.GetPid()).EffectData.GetItemList();
                    for (int itemIdx = 0; itemIdx < tileItemList.Count; itemIdx++)
                    {
                        if (tileItemList[itemIdx].Tokentype.Equals(TokenType.Capital))
                        {
                           // Debug.Log(GameUtil.GetTokenEnumName(tileItemList[itemIdx])+"자원 요구 하도록 구역 컴플레인 세팅");
                            chunkNum = randomIdx[i];
                            return new TOrderItem(TokenType.Capital, tileItemList[itemIdx].SubIdx, 1);
                        }
                    }
                }
                else if (item.GetTokenType().Equals(TokenType.Char))
                {
                    //헌트 아이템으로
                  //  Debug.Log(item.GetPid() + "번 몬스터 사냥 하도록 구역 컴플레인 세팅");
                    chunkNum = randomIdx[i];
                    return new TOrderItem(TokenType.Char, item.GetPid(), 1);
                }
            } 
        }
        //가능한 내용물이 없으면 none 반환
        chunkNum = NO_CHUNK_NUM;
        return new TOrderItem(TokenType.None,1,1);
    }

    public Chunk GetChunk(int _chunkNum)
    {
        //청크리스트가 널이거나 idx넘버가 범위 밖이라면 null 반환
        if (m_chunkList == null || _chunkNum < 0 || m_chunkList.Count <= _chunkNum)
            return null;

        return m_chunkList[_chunkNum];
    }

    private void MakeChunkPos()
    {
        int x = GameUtil.GetMapLength(true) / MgToken.GetInstance().m_chunkLength;
        if (GameUtil.GetMapLength(true) % MgToken.GetInstance().m_chunkLength != 0)
            x += 1;

        int y = GameUtil.GetMapLength(false) / MgToken.GetInstance().m_chunkLength;
        if (GameUtil.GetMapLength(false) % MgToken.GetInstance().m_chunkLength != 0)
            y += 1;

        //Debug.LogFormat("구역총수는{0} x로는{1} y로는{2}개 존재", (x*y),x,y);
        m_chunkPos = new int[x, y]; //2차 배열로 생성
        int chunkNum = 0;
        for (int i = 0; i < x; i++)
        {
            for (int l = 0; l < y; l++)
            {
                m_chunkPos[i, l] = chunkNum;
               // Debug.LogFormat("{0},{1} 좌표 구역의 넘버는 {2}", i,l,chunkNum);
                chunkNum += 1;
            }
        }
    }

    private int[] GetChunkPos(int _chunkNum)
    {
        //구역이 좌표상으론 몇번인지 - 현재 청크 넘버는 0,0에서 y축으로 먼저 쌓은뒤 x가 올라가는 방식이므로
        //x좌표는 높이만큼 나눈 몫
        //y좌표는 높이만큼 나눈 나머지
        int yLength = m_chunkPos.GetLength(1); //나눌 기준 행렬입장에선 행의 수 -> 곧 y 높이.
        int x = _chunkNum / yLength;
        int y = _chunkNum % yLength;
        //Debug.LogFormat("2차 좌표의 높이는{0} {1}구역의 좌표는{2},{3}, 구역 넘버는{4}", yLength, _chunkNum, x, y, _posChunkNum);
        return new int[] { x, y };
    }

    private bool GetChunkNumByPos(int[] _pos, out int _num)
    {
        _num = 0;
        int _x = _pos[0];
        int _y = _pos[1];
        if (_x < 0 || _y < 0)
        {
           // Debug.Log("범위 밖");
            return false;
        }
            

        if(_x>= m_chunkPos.GetLength(0) || _y>= m_chunkPos.GetLength(1))
        {
           // Debug.Log("범위 밖");
            return false;
        }

        _num = m_chunkPos[_x, _y];
        return true;
    }

    int[,] rangeBase = { { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
    private void GetChunkByRange(int _centerChunkNum, int _range)
    {
        int[] find = GetChunkPos(_centerChunkNum);
        int findx = find[0];
        int findy = find[1];
        //Debug.LogFormat("찾는 {0}의 좌표는 {1},{2}, 거리는{3}",_centerChunkNum, findx, findy, _range);

       find[0] -= _range; //시작점을 _range만큼 x감소 해서 시작
        for (int i = 0; i < 4; i++)
        {
            //4방향 모두 
            for (int plus = 0; plus < _range; plus++)
            {
                //거리만큼 더하기 진행
                //거리 배열 i 번째꺼중 [0]x ,[1]y 를 더하며 좌표를 산출
                find[0] += rangeBase[i,0]; 
                find[1] += rangeBase[i,1];
                if(GetChunkNumByPos(find, out int chunkNum))
                {
                    //Debug.LogFormat("{3}사거리에 있는 구역의 좌표는 {4},{5} 구역넘버는 {6}, ",_centerChunkNum, findx, findy, _range, find[0], find[1], chunkNum);
                }
            }
        }
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

        //몬스터 사냥인 경우에만 컴플레인 따로 진행 
        if (_orderItem.Tokentype.Equals(TokenType.Char))
        {
            Complain[] curComplain = GamePlayMaster.GetInstance().m_globalComplainList.ToArray();
            for (int i = 0; i < curComplain.Length; i++)
            {
                //컴플레인용 적용
                Complain complain = curComplain[i];
                for (int x = 0; x < complain.NeedItems.Count; x++)
                {
                    TOrderItem complainItem = complain.NeedItems[x];
                    //민원 내용이 hunt가 아니면 패스
                    if (complainItem.Tokentype.Equals(TokenType.Char) == false)
                        continue;

                    //사냥 대상이 전 몬스터 혹은 동일 몬스터면 요구 사냥 횟수를 1 차감
                    if(complainItem.SubIdx.Equals(int.MaxValue) || complainItem.SubIdx.Equals(_orderItem.SubIdx))
                    {
                        TOrderItem renewValue = complainItem;
                        renewValue.Value -= 1;
                        complain.NeedItems[x] = renewValue;
                        complain.SuccesCheck(); //대응 호출해서 클리어했는지 체크
                    }
                }
                
            }
        }
        
    }
}


