using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgMasterData : Mg<MgMasterData>
{
    private Dictionary<int, TileTypeData> m_tileTypeDataDic;
    private Dictionary<int, TokenChar> m_charDataDic;
    private Dictionary<int, List<TOrderItem>> m_charDropItemDataDic;
    private Dictionary<int, BlessSynerge> m_synergeDataDic;
    private Dictionary<int, TokenAction> m_charActionDataDic;
    private List<int> m_charActionList;
    private Dictionary<int, EquiptItem> m_equipDataDic;
    private Dictionary<int, ContentMasterData> m_contentDataDic;
    private Dictionary<int, NationTechData> m_nationTechDataDic;
    private Dictionary<int, ConversationGroup> m_conversationGroupDic;
    private Dictionary<int, God> m_godDic;
    private Dictionary<int, GodBless> m_blessDic;
    private Dictionary<int, ChunkContent> m_chunkContentDic;
    private Dictionary<int, CapitalData> m_capitalDataDic;
    #region 생성자
    public MgMasterData()
    {
        InitiSet();
    }
    #endregion

    public override void InitiSet()
    {
        g_instance = this;

        SetTileTypeData();
        SetBlessSynergeData();
        SetCharActionData();
        SetCharData();
        SetEquiptData();
        SetContentData();
        SetNationTechData();
        SetConversationData();
        SetBlessData();
        SetGodData();
        SetChunkData();
        SetCapitalData();
        SetExtraValue();
    }

    public override void ReferenceSet()
    {
        // Debug.Log("마스터데이터 레퍼런스 시작");

    }
    #region Get
    public TileTypeData GetTileData(int _tileTypeID)
    {
        //return m_tileTypeDataDic[_tileTypeID];
        return GetDicData<TileTypeData>(m_tileTypeDataDic, _tileTypeID);
    }

    public Dictionary<int, TokenChar> GetCharDic()
    {
        return m_charDataDic;
    }

    public EquiptItem GetEquiptData(int _equiptPid)
    {
        return GetDicData<EquiptItem>(m_equipDataDic, _equiptPid);
    }

    public List<TOrderItem> GetDropItem(int _monsterPid)
    {
        return GetDicData<List<TOrderItem>>(m_charDropItemDataDic, _monsterPid);
    }

    public TokenChar GetCharData(int _charPID)
    {
        return GetDicData<TokenChar>(m_charDataDic, _charPID);
    }

    public TokenAction GetMasterCharAction(int _actionPID)
    {
        return GetDicData<TokenAction>(m_charActionDataDic, _actionPID);
    }

    public List<int> GetCharActionList()
    {
        return m_charActionList;
    }

    public ContentMasterData GetContentData(int _contentPID)
    {
        return GetDicData<ContentMasterData>(m_contentDataDic, _contentPID);
    }

    public StageMasterData GetStageData(int _contentPID, int _stageStep)
    {
        ContentMasterData contentData = GetDicData<ContentMasterData>(m_contentDataDic, _contentPID);
        return contentData.GetStageData(_stageStep);
    }

    public Dictionary<int, ContentMasterData> GetContentDataDic()
    {
        return m_contentDataDic;
    }

    public NationTechData GetTechData(int _techPID)
    {
        return GetDicData<NationTechData>(m_nationTechDataDic, _techPID);
    }

    public ConversationGroup GetThemConversation(ConversationThemeEnum _theme)
    {
        return GetDicData<ConversationGroup>(m_conversationGroupDic, (int)_theme);
    }

    public ConversationData GetConversationData(ConversationThemeEnum _theme, int _pid)
    {
        return GetDicData<ConversationGroup>(m_conversationGroupDic, (int)_theme).GetConversationData(_pid);
    }

    public BlessSynerge GetBlessSynergeData(int _pid)
    {
        return GetDicData<BlessSynerge>(m_synergeDataDic, _pid);
    }

    public Dictionary<int, God> GetGodDic()
    {
        return m_godDic;
    }

    public GodBless GetGodBless(int _blessPID)
    {
        return GetDicData<GodBless>(m_blessDic, _blessPID);
    }

    public ChunkContent GetChunkContent(int _contentPID)
    {
        return GetDicData<ChunkContent>(m_chunkContentDic, _contentPID);
    }

    public Dictionary<int, NationTechData> GetTechDic()
    {
        return m_nationTechDataDic;
    }

    public CapitalData GetCapitalData(Capital _capitalPid)
    {
        return GetDicData<CapitalData>(m_capitalDataDic, (int)_capitalPid);
    }

    private T1 GetDicData<T1>(Dictionary<int, T1> _dic, int _pid)
    {
        if (_dic.ContainsKey(_pid))
            return _dic[_pid];

        return default(T1);
    }

    public bool CheckPID(EMasterData _dataType, int pid)
    {
        bool isHaveData = false;
        switch (_dataType)
        {
            case EMasterData.CharData:
                isHaveData = m_charDataDic.ContainsKey(pid);
                break;
            case EMasterData.TileType:
                isHaveData = m_tileTypeDataDic.ContainsKey(pid);
                break;
        }
        return isHaveData;
    }
    #endregion

    #region 마스터 데이터 생성
    private void SetTileTypeData()
    {
        m_tileTypeDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.TileType);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TileTypeData newTileData = new(parseData.DbValueList[i], parseData.MatchCode);
            if (m_tileTypeDataDic.ContainsKey(newTileData.TypePID))
            {
                Debug.LogError("중첩 pid" + newTileData.TypePID + " " + newTileData.PlaceName);
                continue;
            }
            m_tileTypeDataDic.Add(newTileData.TypePID, newTileData);
        }

        //파싱한 자료를 근거로 해당 타일에서 지을 수 있는 건물을 재정리
        foreach (KeyValuePair<int, TileTypeData> item in m_tileTypeDataDic)
        {
            int[] needPlaces = item.Value.NeedTiles;
            for (int x = 0; x < needPlaces.Length; x++)
            {
                int inNeed = needPlaces[x];
                if (m_tileTypeDataDic.ContainsKey(inNeed))
                {
                    //반대로 해당 장소에서 지을 수 있는 목록에 자신을 추가
                    if (item.Value.IsInterior)
                    {
                        //인테리어 타입이면 인테리어 건설측에
                        m_tileTypeDataDic[inNeed].AbleInteriorPid.Add(item.Value.TypePID);
                    }
                    else
                    {
                        //아니면 그냥 건설측에
                        m_tileTypeDataDic[inNeed].AbleBuildPid.Add(item.Value.TypePID);
                    }

                }
            }
        }
        //    Debug.Log("완료");
    }

    private void SetBlessSynergeData()
    {
        m_synergeDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.BlessSynerge);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            BlessSynerge newSynergeData = new(parseData.DbValueList[i]);
            m_synergeDataDic.Add(newSynergeData.PID, newSynergeData);
        }
    }

    private void SetCharActionData()
    {
        m_charActionDataDic = new();
        m_charActionList = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.CharActionData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TokenAction masterAction = new(parseData.MatchCode, parseData.DbValueList[i]);
            m_charActionDataDic.Add(masterAction.GetPid(), masterAction);
            m_charActionList.Add(masterAction.GetPid());
        }
    }

    private void SetCharData()
    {
        m_charDataDic = new();
        m_charDropItemDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.CharData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TokenChar masterChar = new(parseData.MatchCode, parseData.DbValueList[i]);
            List<TOrderItem> dropItemList = new();
            GameUtil.ParseOrderItemList(dropItemList, parseData.DbValueList[i], 4);
            m_charDataDic.Add(masterChar.GetPid(), masterChar);
            m_charDropItemDataDic.Add(masterChar.GetPid(), dropItemList);

        }
        //   Debug.Log("캐릭터 마스터 데이터 완료");
    }

    private void SetEquiptData()
    {
        m_equipDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.Equipment);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            EquiptItem equipt = new EquiptItem(parseData.MatchCode, parseData.DbValueList[i]);
            if (m_equipDataDic.ContainsKey(equipt.GetPid()) == false)
                m_equipDataDic.Add(equipt.GetPid(), equipt);
        }
    }

    private void SetContentData()
    {
        m_contentDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.ContentData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            int ContentPid = int.Parse(parseData.DbValueList[i][0]);
            //동일한 pid에서 최초 한번 컨텐츠 데이터를 만들고 
            if (m_contentDataDic.ContainsKey(ContentPid) == false)
            {
                ContentMasterData masterContent = new ContentMasterData(parseData.DbValueList[i]);
                m_contentDataDic.Add(masterContent.ContentPid, masterContent);
            }
            //그뒤는 스테이지 정보만 파싱
            StageMasterData stageData = new StageMasterData(parseData.DbValueList[i]);
            //위 컨텐츠 데이터 딕션에 스테이지 넘버를 키로 스테이지 정보를 추가
            m_contentDataDic[ContentPid].StageDic.Add(stageData.StageNum, stageData);
        }
    }

    private void SetNationTechData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.NationTechTree);
        m_nationTechDataDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            NationTechData nationTechData = new NationTechData(parseContainer.MatchCode, parseContainer.DbValueList[i]);
            m_nationTechDataDic.Add(nationTechData.GetPid(), nationTechData);
        }
    }

    private void SetConversationData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.Conversation);
        m_conversationGroupDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            string[] conversationParsingLine = parseContainer.DbValueList[i];
            string themeStr = conversationParsingLine[0]; //db상 0 번째에 테마 작성
            if (System.Enum.TryParse(themeStr, out ConversationThemeEnum theme) == false)
            {
                //정의 되지 않은 Theme 이면 생성하지말고 넘김
                continue;
            }
            if (m_conversationGroupDic.ContainsKey((int)theme) == false)
            {
                //해당 테마 대화 그룹이 없으면 새로 생성
                ConversationGroup group = new ConversationGroup(theme);
                m_conversationGroupDic.Add((int)theme, group);
            }
            //이후 세부 대화 데이터를 만들어서 그그룹에 추가 
            ConversationData conversationData = new ConversationData(parseContainer.DbValueList[i]);
            m_conversationGroupDic[(int)theme].AddConversationData(conversationData);
        }
    }

    private void SetGodData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.God);

        m_godDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            //이후 세부 대화 데이터를 만들어서 그그룹에 추가 
            God parseGod = new God(parseContainer.DbValueList[i]);
            m_godDic.Add(parseGod.PID, parseGod);
        }
    }

    private void SetBlessData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.GodBless);

        m_blessDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            //이후 세부 대화 데이터를 만들어서 그그룹에 추가 
            GodBless parseGodBless = new GodBless(parseContainer.DbValueList[i]);
            m_blessDic.Add(parseGodBless.GetPid(), parseGodBless);
        }
    }

    private void SetChunkData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.ChunkContent);
        m_chunkContentDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            //이후 세부 대화 데이터를 만들어서 그그룹에 추가 
            ChunkContent chunkContent = new ChunkContent(parseContainer.MatchCode, parseContainer.DbValueList[i]);
            m_chunkContentDic.Add(chunkContent.PID, chunkContent);
        }
    }

    private void SetCapitalData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.CapitalData);
        m_capitalDataDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            //이후 세부 대화 데이터를 만들어서 그그룹에 추가 
            CapitalData capitalMasterData = new CapitalData(parseContainer.MatchCode, parseContainer.DbValueList[i]);
            if (m_capitalDataDic.ContainsKey(capitalMasterData.capital) == false)
            {
                // Debug.Log((Capital)capitalMasterData.capital + " 데이터 추가");
                m_capitalDataDic.Add(capitalMasterData.capital, capitalMasterData);
            }

        }
    }

    private void SetExtraValue()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.ExtraValue);
        new ExtraValues(parseContainer.DbValueList[0]);
    }

    #endregion
}

public enum TileEffectEnum
{
  None, Money, Stat, Tool, UIOpen
}

public class TileTypeData {
    public int TypePID;
    public string PlaceName;
    public TileEffectEnum effectType = TileEffectEnum.None;
    public TItemListData EffectData;
    public bool IsAutoReady = false;
    public bool IsAutoEffect = false;
    //건설
    public int NeedLaborTurn; 
    public int NeedLaborAmount;
    public int[] NeedTiles;
    public TItemListData BuildCostData;
    public int BuildNeedLaborValue;
    public bool IsInterior = false; //해당 장소는 인테리어 타입인지
    //파괴
    public int NeedDestroyTurn = 1; //보통 1
    public TItemListData DestroyCostData;

    public int[] TileStat; //해당 장소의 내구도 같은것들
    public List<int> AbleBuildPid = new(); //진화 가능한 외부 건물
    public List<int> AbleInteriorPid = new(); //지을 수 있는 내부 건물
    

    public TileTypeData(string[] _parsingData, List<int[]> _matchCode)
    {
        //TypePID = int.Parse(_parsingData[0]);
        int tileTypeIndex = 0;
        if (System.Enum.TryParse(typeof(TileType), _parsingData[tileTypeIndex], out object parseTileType))
            TypePID = (int)parseTileType;
        PlaceName = _parsingData[1];

        //스텟들 파싱
        TileStat = new int[System.Enum.GetValues(typeof(ETileStat)).Length];
        GameUtil.InputMatchValue(ref TileStat, _matchCode, _parsingData);
        //추가 설정이 필요한것들 진행
        TileStat[(int)ETileStat.CurDurability] = TileStat[(int)ETileStat.MaxDurability];
     
        int effectTypeIndex = 2;
        if( System.Enum.TryParse(typeof(TileEffectEnum), _parsingData[effectTypeIndex], out object ffectType))
            effectType = (TileEffectEnum)ffectType;
      //  Debug.Log(ffectType + " " + effectType);

        int effectIndex = effectTypeIndex + 1;
        if (_parsingData.Length > effectIndex)
        {
            EffectData = GameUtil.ParseCostDataArray(_parsingData, effectIndex);
        }

        int autoActReadyIndex = effectIndex + 1;
        if (_parsingData[autoActReadyIndex] == "T")
        {
            IsAutoReady = true;
        }

        int autoEffectIndex = autoActReadyIndex += 1;
        if (_parsingData[autoEffectIndex] == "T")
        {
            IsAutoEffect = true;
        }

        int needLaborTurn = autoEffectIndex + 1;
        NeedLaborTurn = int.Parse(_parsingData[needLaborTurn]);

        int needLaborIndex = needLaborTurn += 1;
        NeedLaborAmount = int.Parse(_parsingData[needLaborIndex]);

        int interiorTypeIdx = needLaborIndex+1;
        if (_parsingData[interiorTypeIdx] == "T")
        {
            IsInterior = true;
        }
    
        int needTileTypeIdx = interiorTypeIdx +1; //해당 건물(장소)를 짓는데 필요한 장소
        string[] needTiles = _parsingData[needTileTypeIdx].Split(",");
        NeedTiles = new int[needTiles.Length];
        for (int i = 0; i < needTiles.Length; i++)
        {
            if (System.Enum.TryParse(typeof(TileType), needTiles[i], out object parseNeedTileType))
                NeedTiles[i] = (int)parseNeedTileType;
            else
            {
                Debug.Log("정의 되지 않은 재료는 " + (TileType)NeedTiles[i]);
            }
           // Debug.Log(parseNeedTileType);
        }
            
        

        int buildCostIdx = needTileTypeIdx + 1;
        BuildCostData = GameUtil.ParseCostDataArray(_parsingData, buildCostIdx);
      
        int laborValueIdx = buildCostIdx += 1;
        if (_parsingData.Length > laborValueIdx)
        {
            // CostData =  토큰그룹_pid_수량 으로 구성
            BuildNeedLaborValue = int.Parse(_parsingData[laborValueIdx]);
           // Debug.Log("짓는데 필요한 노동량 " + BuildNeedLaborValue);
        }

        int destroyTurnIdx = laborValueIdx + 1;
        if (int.TryParse(_parsingData[destroyTurnIdx], out int destroyTurn)){
            NeedDestroyTurn = destroyTurn;
        }

        int destroyCostIdx = destroyTurnIdx + 1;
        // CostData =  토큰그룹_pid_수량 으로 구성
        DestroyCostData = GameUtil.ParseCostDataArray(_parsingData, destroyCostIdx);
      
    }
}

