using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgMasterData : Mg<MgMasterData>
{
    private Dictionary<int, TileTypeData> m_tileTypeDataDic;
    private Dictionary<int, TokenChar> m_charDataDic;
    private Dictionary<int, TokenAction> m_tileActionDataDic;
    private Dictionary<int, TokenAction> m_charActionDataDic;
    private Dictionary<int, TokenEvent> m_eventDataDic;
    private Dictionary<int, ContentData> m_contentDataDic;
    private Dictionary<int, NationTechTree> m_nationTechDataDic;
    private Dictionary<int, ConversationGroup> m_conversationGroupDic;
    public static char DIVIDECHAR = '_';
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
    }

    public override void ReferenceSet()
    {
       // Debug.Log("마스터데이터 레퍼런스 시작");
   
    }

    public TileTypeData GetTileData(int _tileTypeID)
    {
        //return m_tileTypeDataDic[_tileTypeID];
        return GetDicData<TileTypeData>(m_tileTypeDataDic, _tileTypeID);
    }

    public TokenChar GetCharData(int _charPID)
    {
        return GetDicData<TokenChar>(m_charDataDic, _charPID);
    }

    public TokenAction GetMasterCharAction(int _actionPID)
    {
        return GetDicData<TokenAction>(m_charActionDataDic, _actionPID);
    }

    public TokenAction GetTileActions(int _actionPID)
    {
        return GetDicData<TokenAction>(m_tileActionDataDic, _actionPID);
    }

    public TokenEvent GetEventData(int _eventPID)
    {
        return GetDicData<TokenEvent>(m_eventDataDic, _eventPID);
    }

    public ContentData GetContentData(int _contentPID)
    {
        return GetDicData<ContentData>(m_contentDataDic, _contentPID);
    }

    public NationTechTree GetTechData(int _techPID)
    {
        return GetDicData<NationTechTree>(m_nationTechDataDic, _techPID);
    }

    public ConversationGroup GetThemConversation(ConversationTheme _theme)
    {
        return GetDicData<ConversationGroup>(m_conversationGroupDic, (int)_theme);
    }

    public ConversationData GetConversationData(ConversationTheme _theme, int _pid)
    {
        return GetDicData<ConversationGroup>(m_conversationGroupDic, (int)_theme).GetConversationData(_pid);
    }

    public Dictionary<int, NationTechTree> GetTechDic()
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
        ParseData parseData = MgParsing.GetInstance().GetMasterData(EMasterData.CharActionData);
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            TokenAction masterAction = new(parseData.MatchCode, parseData.DbValueList[i]);
            m_charActionDataDic.Add(masterAction.GetPid(), masterAction);
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
            ContentData masterContent = new ContentData(parseData.DbValueList[i]);
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
            NationTechTree nationTechData = new NationTechTree(parseContainer.MatchCode, parseContainer.DbValueList[i]);
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
            if(System.Enum.TryParse<ConversationTheme>(themeStr, out ConversationTheme theme) == false)
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

    #endregion
}

public class TileTypeData {
    public int TypePID;
    public int[] AbleTileActionPID;
    public int BuildCost;
    public OrderCostData BuildCostData;
    public TileTypeData(string[] _parsingData)
    {
        TypePID = int.Parse(_parsingData[0]);
        string ables = _parsingData[2]; //가능한 작업이 나열되어있음
        string[] divideAble = ables.Trim().Split(MgMasterData.DIVIDECHAR);
        AbleTileActionPID = new int[divideAble.Length];
        for (int i = 0; i < divideAble.Length; i++)
        {
            AbleTileActionPID[i] = int.Parse(divideAble[i]);
        }
        BuildCost = int.Parse(_parsingData[3]);

        int buildCostIdx = 5;
        if (_parsingData.Length > buildCostIdx)
        {
            // CostData =  토큰그룹_pid_수량 으로 구성
            BuildCostData = GameUtil.ParseCostDataArray(_parsingData, buildCostIdx);
        }
        
    }
}

