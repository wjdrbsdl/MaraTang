using System.Collections.Generic;
using UnityEngine;

public class ContentData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public List<TOrderItem> ActConditionList; //컨텐츠 발동조건 리스트
    public Dictionary<int, StageInfo> StageDic; //컨텐츠 각 단계 내용

    public ContentData(string[] _parsingData)
    {
        ContentPid = int.Parse(_parsingData[0]);
        //ConditionType = (EOrderType)int.Parse(_parsingData[1]);
        ActConditionList = new(); //발동조건
        int condtionIdx = 1;
        GameUtil.ParseOrderItemList(ActConditionList, _parsingData, condtionIdx);
        //1. 컨디션의 매인 아이템 리스트 파싱

        int stepIdx = condtionIdx + 1;
        if (_parsingData.Length <= stepIdx)
            return;
        int totalStep = _parsingData[stepIdx].Split(FixedValue.PARSING_LINE_DIVIDE).Length; //단계 수 구함 - 각 단계마다 stage 인포 정해져있음
        int situAdaptCountIdx = stepIdx + 1;
        int situationIdx = situAdaptCountIdx + 1;
        int succesConditionIdx = situationIdx + 1;
        int rewardIdx = succesConditionIdx + 1;
        int penaltyIdx = rewardIdx + 2;

        string[] situAdapValues = _parsingData[situAdaptCountIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] situationDivides = _parsingData[situationIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] succesDivides = _parsingData[succesConditionIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] rewardDivdes = _parsingData[rewardIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
        string[] penaltyDivdes = _parsingData[penaltyIdx].Split(FixedValue.PARSING_LINE_DIVIDE);


        StageDic = new();
        for (int curStep = 1; curStep <= totalStep; curStep++)
        {
            int arrayIndex = curStep - 1;
            StageInfo stageInfo = new StageInfo(curStep, situAdapValues[arrayIndex], situationDivides[arrayIndex], succesDivides[arrayIndex],
                                                        rewardDivdes[arrayIndex], penaltyDivdes[arrayIndex]);
            StageDic.Add(curStep, stageInfo);
        }

        //foreach (KeyValuePair<int, StageInfo> a in StageDic)
        //{
        //    StageInfo stage = a.Value;
        //    List<TOrderItem> itemList = stage.SuccesConList;
        //    for (int i = 0; i < itemList.Count; i++)
        //    {
        //        TOrderItem item = itemList[i];
        //        Debug.LogFormat("{0}타입의 {1}서브 {2} 밸류로 상황 진행", item.Tokentype, item.SubIdx, item.Value);
        //    }
        //    Debug.Log("\n");
        //}

    }
}

public class StageInfo
{
    public int SituAdapCount;
    public List<TOrderItem> SituationList; //컨텐츠 수행 상황 리스트
    public List<TOrderItem> SuccesConList;
    public List<TOrderItem> RewardList;
    public List<TOrderItem> PenaltyList;
    public int StageNum;
    public StageInfo(int _stageNum, string _situAdapStr, string _sitautionStrData, string _succesConStr, string _rewardStrData, string _penaltyStrData)
    {
        StageNum = _stageNum;
        bool nonDataAdd = true;

        SituAdapCount = int.Parse(_situAdapStr); //상황조성 조건 중 적용할 수
        SituationList = new();
        GameUtil.ParseOrderItemList(SituationList, _sitautionStrData, nonDataAdd);
        SuccesConList = new();
        GameUtil.ParseOrderItemList(SuccesConList, _succesConStr, nonDataAdd);
        RewardList = new();
        GameUtil.ParseOrderItemList(RewardList, _rewardStrData, nonDataAdd);
        PenaltyList = new();
        GameUtil.ParseOrderItemList(PenaltyList, _penaltyStrData, nonDataAdd);
    }
}