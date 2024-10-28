using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgMasterData : Mg<MgMasterData>
{
    private Dictionary<int, TileTypeData> m_tileTypeDataDic;
    private Dictionary<int, TokenChar> m_charDataDic;
    private Dictionary<int, TokenAction> m_tileActionDataDic;
    private Dictionary<int, TokenAction> m_charActionDataDic;
    private List<int> m_charActionList;
    private Dictionary<int, TokenEvent> m_eventDataDic;
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
        SetTileActionData();
        SetCharActionData();
        SetCharData();
        SetEventData();
        SetContentData();
        SetNationTechData();
        SetConversationData();
        SetGodData();
        SetBlessData();
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

    public TokenAction GetTileActions(int _actionPID)
    {
        return GetDicData<TokenAction>(m_tileActionDataDic, _actionPID);
    }

    public TokenEvent GetEventData(int _eventPID)
    {
        return GetDicData<TokenEvent>(m_eventDataDic, _eventPID);
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

    public Dictionary<int, God> GetGodDic()
    {
        return m_godDic;
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
            case EMasterData.TileActionData:
                isHaveData = m_tileActionDataDic.ContainsKey(pid);
                break;
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
            TileTypeData newTileData = new(parseData.DbValueList[i]);
            m_tileTypeDataDic.Add(newTileData.TypePID, newTileData);
        }
    //    Debug.Log("완료");
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
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.CharData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TokenChar masterChar = new(parseData.MatchCode, parseData.DbValueList[i]);
            m_charDataDic.Add(masterChar.GetPid(), masterChar);
        }
     //   Debug.Log("캐릭터 마스터 데이터 완료");
    }

    private void SetEventData()
    {
        m_eventDataDic = new();
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.EventData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TokenEvent masterEvent = new(parseData.MatchCode, parseData.DbValueList[i]);
            m_eventDataDic.Add(masterEvent.GetPid(), masterEvent);
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

    private void SetTileActionData()
    {
        ParseData parseContainer = MgParsing.GetInstance().GetMasterData(EMasterData.TileActionData);
        m_tileActionDataDic = new();
        for (int i = 0; i < parseContainer.DbValueList.Count; i++)
        {
            TokenAction tileAction = new TokenAction(parseContainer.MatchCode, parseContainer.DbValueList[i]);
            m_tileActionDataDic.Add(tileAction.GetPid(), tileAction);
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
            m_blessDic.Add(parseGodBless.PID, parseGodBless);
        }
    }

    #endregion
}

public class TileTypeData {
    public int TypePID;
    public string PlaceName;
    public int[] AbleTileActionPID;
    public TItemListData BuildCostData;
    public int[] Places;
    public TileTypeData(string[] _parsingData)
    {
        TypePID = int.Parse(_parsingData[0]);
        PlaceName = _parsingData[1];
        string ables = _parsingData[2]; //가능한 작업이 나열되어있음
        string[] divideAble = ables.Trim().Split(FixedValue.PARSING_LIST_DIVIDE);
        AbleTileActionPID = new int[divideAble.Length];
        for (int i = 0; i < divideAble.Length; i++)
        {
            AbleTileActionPID[i] = int.Parse(divideAble[i]);
        }

        int buildCostIdx = 3;
        if (_parsingData.Length > buildCostIdx)
        {
            // CostData =  토큰그룹_pid_수량 으로 구성
            BuildCostData = GameUtil.ParseCostDataArray(_parsingData, buildCostIdx);
        }

        int subSpaceIdx = 4;
        Places = new int[] { };
        if (_parsingData.Length > subSpaceIdx)
        {
            string subables = _parsingData[subSpaceIdx]; //가능한 작업이 나열되어있음
            string[] subAble = subables.Trim().Split(FixedValue.PARSING_LIST_DIVIDE);
            Places = new int[subAble.Length];
            for (int i = 0; i < subAble.Length; i++)
            {
                Places[i] = int.Parse(subAble[i]);
               // Debug.Log("가능한 서브 건물 " + Places[i]);
            }
        }

    }
}

