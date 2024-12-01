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

    public List<TOrderItem> GetDropItem(int _mosterPid)
    {
        return GetDicData<List<TOrderItem>>(m_charDropItemDataDic, _mosterPid);
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

    public ConversationGroup GetThemConversation(ConversationEnum _theme)
    {
        return GetDicData<ConversationGroup>(m_conversationGroupDic, (int)_theme);
    }

    public ConversationData GetConversationData(ConversationEnum _theme, int _pid)
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

    public Dictionary<int, NationTechData> GetTechDic()
    {
        return m_nationTechDataDic;
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
            m_tileTypeDataDic.Add(newTileData.TypePID, newTileData);
        }

        //파싱한 자료를 근거로 해당 타일에서 지을 수 있는 건물을 재정리
        foreach(KeyValuePair<int, TileTypeData> item in m_tileTypeDataDic)
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
            if(m_equipDataDic.ContainsKey(equipt.GetPid())== false)
            m_equipDataDic.Add(equipt.GetPid(), equipt);
        }
    }

    private void SetContentData()
    {
        m_contentDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.ContentData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            ContentMasterData masterContent = new ContentMasterData(parseData.DbValueList[i]);
            //중복 
            if(m_contentDataDic.ContainsKey(masterContent.ContentPid) == false)
            m_contentDataDic.Add(masterContent.ContentPid, masterContent);
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
            if(System.Enum.TryParse<ConversationEnum>(themeStr, out ConversationEnum theme) == false)
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

    #endregion
}

public enum TileEffectEnum
{
  None, Money, Stat, Tool, OnOff
}

public class TileTypeData {
    public int TypePID;
    public string PlaceName;
    public TileEffectEnum effectType = TileEffectEnum.None;
    public TItemListData EffectData;
    public bool IsAuto = false;
    public int NeedLaborTurn; 
    public int NeedLaborAmount;
    public int[] NeedTiles;
    public bool IsInterior = false; //해당 장소는 인테리어 타입인지
    public int[] TileStat; //해당 장소의 내구도 같은것들
    public List<int> AbleBuildPid; //해당 장소에서 지을 수 있는 건물 모음. 
    public List<int> AbleInteriorPid; //해당 장소에서 지을 수 있는 내부 건물 모음.
    public TItemListData BuildCostData;
    public int BuildNeedLaborValue;

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

        int autoActIndex = effectIndex += 1;
        if (_parsingData[autoActIndex] == "T")
        {
            IsAuto = true;
        }

        int needLaborTurn = autoActIndex + 1;
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
            
        AbleBuildPid = new();
        AbleInteriorPid = new();

        int buildCostIdx = needTileTypeIdx + 1;
        if (_parsingData.Length > buildCostIdx)
        {
            // CostData =  토큰그룹_pid_수량 으로 구성
            BuildCostData = GameUtil.ParseCostDataArray(_parsingData, buildCostIdx);
        }

        int laborValueIdx = buildCostIdx += 1;
        if (_parsingData.Length > laborValueIdx)
        {
            // CostData =  토큰그룹_pid_수량 으로 구성
            BuildNeedLaborValue = int.Parse(_parsingData[laborValueIdx]);
           // Debug.Log("짓는데 필요한 노동량 " + BuildNeedLaborValue);
        }

    }
}

