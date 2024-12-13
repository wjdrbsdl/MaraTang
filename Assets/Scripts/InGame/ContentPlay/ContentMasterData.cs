using System.Collections.Generic;
using UnityEngine;

public class ContentMasterData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public bool AbleRepeat = false;
    public List<TOrderItem> ActConditionList; //컨텐츠 발동조건 리스트
    public Dictionary<int, StageMasterData> StageDic = new (); //컨텐츠 각 단계 내용

    public ContentMasterData(string[] _parsingData)
    {
        ContentPid = int.Parse(_parsingData[0]);
        ActConditionList = new(); //발동조건
        int condtionIdx = 1;
        GameUtil.ParseOrderItemList(ActConditionList, _parsingData, condtionIdx, true);
        int repeatIdx = condtionIdx + 1;
        if (_parsingData[repeatIdx] == "T")
        {
            AbleRepeat = true;
        }
     
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
    public bool AdatpSerial = true; //관련된 정보만 받기 - 몬스터사냥시, 해당 관련 몬스터만 카운트할지, 다른 컨텐츠 몬스터도 카운트할지 여부

    public StageMasterData(string[] _parsingData)
    {
        //스테이지 정보 파싱 
        int stepIdx = 3;
        if (_parsingData.Length <= stepIdx)
            return;
        StageNum = int.Parse(_parsingData[stepIdx]);

        int ableSelectIdx = stepIdx + 1; // 선택가능 여부
        AbleSelect = int.Parse(_parsingData[ableSelectIdx]) != 0; //0이나 그밖에 숫자 string으로 들어온 값을 파싱해서 0인지 체크 

        int situAdaptCountIdx = ableSelectIdx + 1; //조건 적용할 수 - 이게 왜 필요했지
        bool noneDataAdd = true;
        SituAdapCount = int.Parse(_parsingData[situAdaptCountIdx]); //상황조성 조건 중 적용할 수

        int situationIdx = situAdaptCountIdx + 1; //상황 조성
        SituationList = new();
        GameUtil.ParseOrderItemList(SituationList, _parsingData[situationIdx], noneDataAdd);
        if (SituAdapCount == 0)
        {
            //db에 귀찮아서 0 으로 표기한 경우 모든 선택지 가능으로 수정해주기. 
            SituAdapCount = SituationList.Count;
        }

        int serialAdapt = situationIdx + 1; //시리얼 적용여부
        if (!_parsingData[serialAdapt].Equals("F"))
        {
            AdatpSerial = false;
        }

        int succesNeedCountIdx = serialAdapt + 1; //성공에 필요한 수
        SuccedNeedCount = int.Parse(_parsingData[succesNeedCountIdx]);

        int completeAutoIdx = succesNeedCountIdx + 1; //크리어 자동여부
        if (!_parsingData[completeAutoIdx].Equals("A"))
        {
            AutoClear = false;
        }

        int succesConditionIdx = completeAutoIdx + 1; //성공 조건들
        SuccesConList = new();
        GameUtil.ParseOrderItemList(SuccesConList, _parsingData[succesConditionIdx]);

        int rewardIdx = succesConditionIdx + 1; //성공시 갈 스텝
        if (int.TryParse(_parsingData[rewardIdx], out int rewardStep))
        {
            SuccesStep = rewardStep;
        }
        //실패 조건들 미구현
        int failNeedCountIdx = rewardIdx + 1; //실패에 필요 수
        FailNeedCount = int.Parse(_parsingData[failNeedCountIdx]);

        int failConditionIdx = failNeedCountIdx + 1; //실패 조건
        FailConList = new();
        GameUtil.ParseOrderItemList(FailConList, _parsingData[failConditionIdx]);

        int penaltyIdx = failConditionIdx + 1; //실패시 갈 스텝

        if (int.TryParse(_parsingData[penaltyIdx], out int penaltyStep))
        {
            PenaltyStep = penaltyStep;
        }

        int helpIdx = penaltyIdx + 1; //도움말
     

    }
  
}

