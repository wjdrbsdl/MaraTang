using System.Collections.Generic;
using UnityEngine;

public class ContentMasterData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public List<TOrderItem> ActConditionList; //컨텐츠 발동조건 리스트
    public Dictionary<int, StageMasterData> StageDic = new (); //컨텐츠 각 단계 내용

    public ContentMasterData(string[] _parsingData)
    {
        ContentPid = int.Parse(_parsingData[0]);
        //ConditionType = (EOrderType)int.Parse(_parsingData[1]);
        ActConditionList = new(); //발동조건
        int condtionIdx = 1;
        GameUtil.ParseOrderItemList(ActConditionList, _parsingData, condtionIdx, true);
        //1. 컨디션의 매인 아이템 리스트 파싱

        int stepIdx = condtionIdx + 1;
        if (_parsingData.Length <= stepIdx)
            return;
        int totalStep = _parsingData[stepIdx].Split(FixedValue.PARSING_LINE_DIVIDE).Length; //단계 수 구함 - 각 단계마다 stage 인포 정해져있음

        int ableSelectIdx = stepIdx + 1;
        int situAdaptCountIdx = ableSelectIdx + 1; //조건 적용할 수 - 이게 왜 필요했지
        int situationIdx = situAdaptCountIdx + 1; //상황 조성
        int succesNeedCountIdx = situationIdx + 1; //성공에 필요한 수
        int completeAutoIdx = succesNeedCountIdx + 1;
        int succesConditionIdx = completeAutoIdx + 1; //성공 조건들
        int rewardIdx = succesConditionIdx + 1; //성공시 갈 스텝
        //실패 조건들 미구현
        int failNeedCountIdx = rewardIdx + 1; //성공에 필요한 수
        int failConditionIdx = failNeedCountIdx + 1; //성공 조건들
        int penaltyIdx = failConditionIdx + 1; //실패시 갈 스텝

        bool ableSelect = int.Parse(_parsingData[ableSelectIdx])!=0;
        string[] situAdapValues = _parsingData[situAdaptCountIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] situationDivides = _parsingData[situationIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] successNeedCount = _parsingData[succesNeedCountIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] completeAutoBool = _parsingData[completeAutoIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] succesConditiones = _parsingData[succesConditionIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] rewardDivdes = _parsingData[rewardIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] failNeedCount = _parsingData[failNeedCountIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] failConditiones = _parsingData[failConditionIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] penaltyDivdes = _parsingData[penaltyIdx].Split(FixedValue.PARSING_LINE_DIVIDE);

        for (int curStep = 1; curStep <= totalStep; curStep++)
        {
            int arrayIndex = curStep - 1;
            StageMasterData stageInfo = new StageMasterData(curStep, ableSelect, situAdapValues[arrayIndex], situationDivides[arrayIndex],
                                                        successNeedCount[arrayIndex], completeAutoBool[arrayIndex], succesConditiones[arrayIndex],
                                                        failNeedCount[arrayIndex], failConditiones[arrayIndex],
                                                        rewardDivdes[arrayIndex], penaltyDivdes[arrayIndex]);
            StageDic.Add(curStep, stageInfo);
        }

        //foreach (KeyValuePair<int, StageInfo> a in StageDic)
        //{
        //    StageInfo stage = a.Value;
        //    List<TOrderItem> itemList = stage.SituationList;
        //    for (int i = 0; i < itemList.Count; i++)
        //    {
        //        TOrderItem item = itemList[i];
        //        Debug.LogFormat("{0}타입의 {1}서브 {2} 밸류로 상황 진행", item.Tokentype, item.SubIdx, item.Value);
        //    }
        //    Debug.Log("\n");
        //}

    }

    public StageMasterData GetStageData(int _stageStep)
    {
        if (StageDic.ContainsKey(_stageStep))
            return StageDic[_stageStep];

        return default(StageMasterData);
    }
}

public class StageMasterData
{
    public bool AbleSelect; //조건 중 선택이 가능한가
    public int SituAdapCount;
    public List<TOrderItem> SituationList; //컨텐츠 수행 상황 리스트
    public int SuccedNeedCount;
    public List<TOrderItem> SuccesConList;
    public int FailNeedCount;
    public List<TOrderItem> FailConList;
    public int SuccesStep; //성공시 이동할 Stage 기록
    public int PenaltyStep; //실패시 이동할 Stage 기록
    public int StageNum;
    public bool AutoClear = true;
    public StageMasterData(int _stageNum, bool _ableSelect, string _situAdapStr, string _sitautionStrData, 
                           string _succesNeedCountStr, string _autoCompleteBool, string _succesConStr,
                           string _failNeedCountStr, string _failConStr,
                           string _rewardStrData, string _penaltyStrData)
    {
        StageNum = _stageNum;
        AbleSelect = _ableSelect;
        bool noneDataAdd = true;
        SituAdapCount = int.Parse(_situAdapStr); //상황조성 조건 중 적용할 수
        SituationList = new();
        GameUtil.ParseOrderItemList(SituationList, _sitautionStrData, noneDataAdd);
        SuccedNeedCount = int.Parse(_succesNeedCountStr);
        if (!_autoCompleteBool.Equals("A"))
        {
            AutoClear = false;
        }

        SuccesConList = new();
        GameUtil.ParseOrderItemList(SuccesConList, _succesConStr);
        FailNeedCount = int.Parse(_failNeedCountStr);
        FailConList = new();
        GameUtil.ParseOrderItemList(FailConList, _failConStr);
        
         if (int.TryParse(_rewardStrData, out int rewardStep))
        {
            SuccesStep = rewardStep;
        }
        if (int.TryParse(_penaltyStrData, out int penaltyStep))
        {
            PenaltyStep = penaltyStep;
        }
        
    }
}

