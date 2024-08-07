using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : IOrderCustomer
{
    public int ContentPid = 0; //content pid
    public int SerialNum = 0;
    public int RestWoldTurn = 3; //유지되는 기간 
    public int ChunkNum = 0;
    public int CurStep = 1;
    public CurStageData CurStageData; //현재 스테이지 진행 정보, 달성 정보 수행하는 곳. 

    #region 생성
    public Quest()
    {
       
    }

    public Quest(ContentMasterData _contentData, int _chunkNum)
    {
        ContentPid = _contentData.ContentPid;
        ChunkNum = _chunkNum;
        SerialNum = MGContent.GetInstance().GetSerialNum();
    }
    #endregion

    public void FlowTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    #region 스테이지 진행
    public void RealizeStage()
    {
        Debug.Log(CurStep + "단계 구현화 진행");
        StageMasterData stage = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        CurStageData = new CurStageData(stage);
        TTokenOrder order = new TTokenOrder(stage.SituationList, stage.SituAdapCount, SerialNum, this);
        OrderExcutor excutor = new OrderExcutor();
        excutor.ExcuteOrder(order);
        Debug.LogWarning("새 퀘스트 알람 닫아놓음");
        //MgUI.GetInstance().ShowQuest(this);
      //  Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 발동 됨", SerialNum, CurStep);
    }

    public void ClearStage()
    {
        Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 클리어 됨", SerialNum, CurStep);
        ResetSituation();
        int nextStep = CurStageData.SuccesStep;
        CurStep = nextStep;
        if(CurStep == 0)
        {
            MGContent.GetInstance().SuccessQuest(this); //ClearStage()
            return;
        }
        RealizeStage();
    }

    public void FailStage()
    {
        Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 실패 됨", SerialNum, CurStep);
        ResetSituation();
        int nextStep = CurStageData.PenaltyStep;
        CurStep = nextStep;
        if (CurStep == 0)
        {
            MGContent.GetInstance().FailQuest(this); //FailStage에서 호출
            return;
        }
        RealizeStage();
    }

    public void ResetSituation()
    {
   //     Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 리셋", SerialNum, CurStep);
        StageMasterData stageInfo = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        TOrderItem doneItem = stageInfo.SituationList[0];
    }
 
    public void OnOrderCallBack(OrderReceipt _orderReceipt)
    {
        TOrderItem doneItem = _orderReceipt.DoneItem;

        if (doneItem.Tokentype.Equals(TokenType.Conversation))
        {
            SelectItemInfo selectInfo = new SelectItemInfo(null, false);
            MgUI.GetInstance().SetScriptCustomer(selectInfo);
            return;
        }
    }
    #endregion

}

public class CurStageData
{
    //스테이지 클리어를 위한 정보를 기록

    //1. 해당 건물에 입장했는가 - 단일조건
    //2. 몬스터를 잡았는가 - 다수 조건
    //3. 대화 확인을 했는가 - 단일 조건 
    //4. 재료를 몇개 모아서 건넨다
    //5. 보상을 선택한다. 
    //6. 중도 포기한다. 
    //7. 실패 조건을 충족했다. 
    public int SuccesNeedCount = 0; //필요 충족 수 
    public List<TOrderItem> SuccesConList; //맞추려는 조건
    public int FailNeedCount = 0;
    public List<TOrderItem> FailConList;
    public List<TOrderItem> CurConList; //현재 진행 상황
    public int SuccesStep; //성공시 이동할 Stage 기록
    public int PenaltyStep; //실패시 이동할 Stage 기록
    public int StageNum;

    public CurStageData(StageMasterData _stageMasterData)
    {
        SuccesNeedCount = _stageMasterData.SuccedNeedCount;
        SuccesConList = _stageMasterData.SuccesConList; //성공조건은 마스터 그대로
        FailNeedCount = _stageMasterData.FailNeedCount;
        FailConList = _stageMasterData.FailConList;
        CurConList = new List<TOrderItem>(); //현재 상황은 새로 새팅
        InitCurConditionValue(); //현재 상태 초기값 설정
       // Debug.Log("최종 추가된 조건 수" + CurConList.Count);
        InitCheck(); //현재 상태 체크
        SuccesStep = _stageMasterData.SuccesStep;
        PenaltyStep = _stageMasterData.PenaltyStep;
        StageNum = _stageMasterData.StageNum;
    }

    private void InitCurConditionValue()
    {
        //1. 성공 조건과 실패 조건의 TOrder를 투입한다
        //2. TokenType과 SubIdx가 중복되는 것들은 1개로 통합한다. - 중복 체크 필요. 
        //3. 초기값은 각 TokenType, Subidx에 따라 지정한다. 
        //성공 조건 추가
   //     Debug.Log("성공 조건 추가 " + SuccesConList.Count);
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = SuccesConList[i];
            if (CheckOverlap(conditionItem))
            {
                //이미 추가한 아이템이면 패쓰
                continue;
            }
            TokenType conditionType = conditionItem.Tokentype;
            int intialValue = FixedValue.No_VALUE; //초기값은 기본적으로 노 벨류, 조건의 value가 0인경우도 있어서 -1을 기본 세팅. 
            switch (conditionType)
            {
                case TokenType.Char:
                    intialValue = 0; //몬스터의경우 0 => 몬스터의경우 value가 잡은 몬스터 수이므로 0 부터 시작. 
                    break;
                case TokenType.Action:
                    intialValue = PlayerManager.GetInstance().GetPlayerActionLevel(conditionItem.SubIdx); 
                    break;
            }
            
            TOrderItem curConItem = new TOrderItem(conditionType, conditionItem.SubIdx, intialValue); //조건의 value를 0으로 해서 진행
            CurConList.Add(curConItem);
        }
        //실패 조건 추가
   //     Debug.Log("실패 조건 추가 " + FailConList.Count);
        for (int i = 0; i < FailConList.Count; i++)
        {
            TOrderItem conditionItem = FailConList[i];
            if (CheckOverlap(conditionItem))
            {
                //이미 추가한 아이템이면 패쓰
                continue;
            }
            TokenType conditionType = conditionItem.Tokentype;
            int intialValue = FixedValue.No_VALUE; //초기값은 기본적으로 노 벨류, 조건의 value가 0인경우도 있어서 -1을 기본 세팅. 
            switch (conditionType)
            {
                case TokenType.Char:
                    intialValue = 0; //몬스터의경우 0 => 몬스터의경우 value가 잡은 몬스터 수이므로 0 부터 시작. 
                    break;
                case TokenType.Action:
                    intialValue = PlayerManager.GetInstance().GetPlayerActionLevel(conditionItem.SubIdx);
                    break;
            }

            TOrderItem curConItem = new TOrderItem(conditionType, conditionItem.SubIdx, intialValue); //조건의 value를 0으로 해서 진행
            CurConList.Add(curConItem);
        }
    }

    private bool CheckOverlap(TOrderItem _item)
    {
        //중복 체크
        for (int i = 0; i < CurConList.Count; i++)
        {
            TOrderItem curItem = CurConList[i];
            if (curItem.Tokentype == _item.Tokentype && curItem.SubIdx == _item.SubIdx)
            {
         //       Debug.Log(curItem.Tokentype + ":" + curItem.SubIdx+"중복 "); 
                return true;
            }
                
        }
        return false;
    }

    private void InitCheck()
    {
      
    }


    #region 액션을 현재 상태에 반영
    public void AdaptCondtion(TOrderItem _adaptItem)
    {
        //새 item을 현재 상태에 적용
        TokenType adaptType = _adaptItem.Tokentype;
        for (int i = 0; i < CurConList.Count; i++)
        {
            TOrderItem curCondtion = CurConList[i]; //현재 상태
            //1. 현재 조건 상태의 TokenType과 동일한지부터 체크 
            if(_adaptItem.Tokentype != curCondtion.Tokentype)
            {
               // Debug.LogFormat("적용 타입 {0}, 조건 타입 {1}으로 타입이 다름", _adaptItem.Tokentype, curCondtion.Tokentype);
                continue;
            }

            //2. 토큰타입에 따라 개별적으로 진행
            bool isAdapt = false;
            switch (adaptType)
            {
                case TokenType.Char: //몬스터는 목표 pid(sub) 처치시 value 1 상승
                    isAdapt = RequestAdaptHunt(_adaptItem, curCondtion, i);
                    break;
                case TokenType.Action: //pid 보유시 해당 액션 레벨을 value 로 적용(adapt된 토큰타입은 상관없음)
                    isAdapt = RequestAdaptActionLv(_adaptItem, curCondtion, i);
                    break;
                //case TokenType.Conversation: //pid 보유시 해당 액션 레벨을 value 로 적용(adapt된 토큰타입은 상관없음)
                //    isAdapt = RequestAdaptResponse(_adaptItem);
                //    break;
                default: //그외 입력된 값으로 조건값을 바꾸면 되는 경우 통합
                    isAdapt = RequestAdaptValue(_adaptItem, curCondtion, i);
                    break;
            }
            //3. 만약 조건 중 어느 하나에 적용했다면 나머지 조건은 안봐도 됨. 그러려면 Adapt의 여부를 반환받아야함. 
            if (isAdapt)
                break;

        }
    }
    private bool RequestAdaptHunt(TOrderItem _huntMonItem, TOrderItem _curRecord, int _index)
    {
        //잡은 몬스터와 잡아야하는 몬스터 pid가 동일하면 현재 조건에 +1
        if(_curRecord.SubIdx == _huntMonItem.SubIdx)
        {
            TOrderItem newCurCondition = _curRecord;
            newCurCondition.SetValue(_curRecord.Value + 1);
            CurConList[_index] = newCurCondition; //조건 상황이 변했으면 새로 할당
            return true;
        }
        return false;
     }
    private bool RequestAdaptActionLv(TOrderItem _studyAction, TOrderItem _curRecord, int _index)
    {
        //확인하려는 action Pid가 맞으면 플레이어 레벨 적용 
        //1. 스킬 pid 확인
        if (_curRecord.SubIdx == _studyAction.SubIdx) 
        {
            TOrderItem newCurCondition = _curRecord; //새로 생성
            //2. 레벨 가져옴
            int actionLevel = PlayerManager.GetInstance().GetPlayerActionLevel(_studyAction.SubIdx);
            //3. 레벨 세팅
            newCurCondition.SetValue(actionLevel); 
            //4. 조건에 새로 세팅
            CurConList[_index] = newCurCondition; //조건 상황이 변했으면 새로 할당
            return true;
        }
        return false;
    }
    private bool RequestAdaptResponse(TOrderItem _response)
    {
        //확인, 취소 등의 응답이 들어왔을 때 조건 충족
        //1 해당 응답과 성공 조건을 비교해서 값이 같은 index의 현재 조건값을 변경
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            TOrderItem successItem = SuccesConList[i];
            if(successItem.SubIdx == _response.SubIdx && successItem.Value == _response.Value)
            {
                //성공 조건중 일치하는 응답이 있으면, 현재 조건의 index 값을 성공조건으로 할당 
                CurConList[i] = successItem;
                return true;
            }
        }
        return false;
    }
    private bool RequestAdaptValue(TOrderItem _adaptItem, TOrderItem _curRecord, int _index)
    {
        //단순히 최근 값을 현재 상태로 적용하면 되는 경우 
      //  Debug.LogFormat("적용하려는 Adapt 대상이 {0}타입에 sub{1}인지 체크 기존 밸류는{2}", _curRecord.Tokentype, _curRecord.SubIdx,_curRecord.Value);
        //subIdx 확인후 
        if (_curRecord.SubIdx == _adaptItem.SubIdx)
        {
            //그냥 할당
            CurConList[_index] = _adaptItem;
            Debug.LogFormat("{0}타입 적용할 sub{1}에 {2}로 값 적용\n적용후 값{3}", _adaptItem.Tokentype, _adaptItem.SubIdx, _adaptItem.Value, CurConList[_index].Value);
            return true;
        }
        return false;
      //  Debug.LogFormat("{0}타입 적용할 sub{1}가 다름",_adaptItem.Tokentype,_adaptItem.SubIdx);
    }
    #endregion

    #region 성공 실패 체크 
    public bool CheckSuccess()
    {
        //현재 상태와 목표 상태를 각 토큰타입에 따라 벨류를 따져봄
        int passCount = 0; //통과한수
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            //1. 성공 조건들 중 1개 뺌
            TOrderItem successCondition = SuccesConList[i]; 
            for (int curConIdx = 0; curConIdx < CurConList.Count; curConIdx++)
            {
                //2. 성공 조건과 현재 조건을 하나씩 비교 
                TOrderItem curCondtion = CurConList[curConIdx]; //현재 상태
                TokenType conditionType = curCondtion.Tokentype;
                if(curCondtion.Tokentype != successCondition.Tokentype || curCondtion.SubIdx != successCondition.SubIdx)
                {
                    //3. 성공 조건과 현재 조건이 비교 대상인지 체크 
                    //4. 다르면 패싱
                    continue;
                }

                //5. 비교 조건이면 성공 여부 체크 
                bool isPass = false; //개별 성공 여부
                                     //수행된 조건 타입에 따라 현재 조건 상태를 변화
                switch (conditionType)
                {
                    case TokenType.Char: //몬스터의 경우 목표 몬스터 처치시 현재 상태 value 1 상승
                    case TokenType.Action:
                        isPass = IsEnoughValue(successCondition, curCondtion);
                        break;
                    default:
                        isPass = IsMatch(successCondition, curCondtion);
                        break;
                }

                if (isPass)
                    passCount += 1;

                //6. 성공 조건에 맞는 현재 조건을 따져봤으므로 break;
                break; 
            }
            //7. 다음 성공 조건 루프 
        }
        Debug.LogFormat("필요수{0} 충족수{1} 조건수{2}", SuccesNeedCount, passCount, SuccesConList.Count);
        if (SuccesNeedCount <= passCount)
        {
            return true;
        }
        return false;
    }

    public bool CheckFail()
    {
        //현재 상태와 목표 상태를 각 토큰타입에 따라 벨류를 따져봄
        int failCount = 0; //실패한수
        for (int i = 0; i < FailConList.Count; i++)
        {
            //1. 성공 조건들 중 1개 뺌
            TOrderItem failCondition = FailConList[i];
            for (int curConIdx = 0; curConIdx < CurConList.Count; curConIdx++)
            {
                //2. 성공 조건과 현재 조건을 하나씩 비교 
                TOrderItem curCondtion = CurConList[curConIdx]; //현재 상태
                TokenType conditionType = curCondtion.Tokentype;
                if (curCondtion.Tokentype != failCondition.Tokentype || curCondtion.SubIdx != failCondition.SubIdx)
                {
                    //3. 성공 조건과 현재 조건이 비교 대상인지 체크 
                    //4. 다르면 패싱
                    continue;
                }

                //5. 비교 조건이면 성공 여부 체크 
                bool isPass = false; //개별 성공 여부
                                     //수행된 조건 타입에 따라 현재 조건 상태를 변화
                switch (conditionType)
                {
                    case TokenType.Char: //몬스터의 경우 목표 몬스터 처치시 현재 상태 value 1 상승
                    case TokenType.Action:
                        isPass = IsEnoughValue(failCondition, curCondtion);
                        break;
                    default:
                        isPass = IsMatch(failCondition, curCondtion);
                        break;
                }

                if (isPass)
                    failCount += 1;

                //6. 성공 조건에 맞는 현재 조건을 따져봤으므로 break;
                break;
            }
            //7. 다음 성공 조건 루프 
        }
        Debug.LogFormat("실패 필요수{0} 충족수{1} 조건수{2}", FailNeedCount, failCount, FailConList.Count);
        if (FailNeedCount <= failCount)
        {
            return true;
        }
        return false;
    }

    private bool IsEnoughValue(TOrderItem _target, TOrderItem _cur)
    {
        if (_target.Value <= _cur.Value)
            return true;

        return false;
    }
    private bool IsMatch(TOrderItem _target, TOrderItem _cur)
    {
        //조건값과 현재값이 동일하면 되는 경우 
        if (_target.Value == _cur.Value)
            return true;

        return false;
    }
    #endregion
}