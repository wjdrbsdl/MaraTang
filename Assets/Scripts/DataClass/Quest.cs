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
            MGContent.GetInstance().SuccessQuest(this);
            return;
        }
        RealizeStage();
    }

    public void ResetSituation()
    {
        Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 리셋", SerialNum, CurStep);
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
    public List<TOrderItem> SuccesConList; //맞추려는 조건
    public List<TOrderItem> CurConList; //현재 진행 상황
    public List<bool> CurPassRecordList; //현재 달성 상황
    public int SuccesStep; //성공시 이동할 Stage 기록
    public int PenaltyStep; //실패시 이동할 Stage 기록
    public int StageNum;

    public CurStageData(StageMasterData _stageMasterData)
    {
        SuccesConList = _stageMasterData.SuccesConList; //성공조건은 마스터 그대로
        CurConList = new List<TOrderItem>(); //현재 상황은 새로 새팅
        CurPassRecordList = new List<bool>();
        for (int i = 0; i < SuccesConList.Count; i++)
        {
            TOrderItem conditionItem = SuccesConList[i];
            TokenType conditionType = conditionItem.Tokentype;
            int intialValue = FixedValue.No_VALUE; //초기값은 기본적으로 노 벨류, 조건의 value가 0인경우도 있어서 -1을 기본 세팅. 
            if (conditionType.Equals(TokenType.Char))
                intialValue = 0; //몬스터의경우 0 => 몬스터의경우 value가 잡은 몬스터 수이므로 0 부터 시작. 
            TOrderItem curConItem = new TOrderItem(conditionType, conditionItem.SubIdx, intialValue); //조건의 value를 0으로 해서 진행

            CurConList.Add(curConItem);
            CurPassRecordList.Add(false);
        }
        SuccesStep = _stageMasterData.SuccesStep;
        PenaltyStep = _stageMasterData.PenaltyStep;
        StageNum = _stageMasterData.StageNum;
    }


    #region 액션을 현재 상태에 반영
    public void AdaptCondtion(TOrderItem _adaptItem)
    {
        //새 item을 현재 상태에 적용
        TokenType adaptType = _adaptItem.Tokentype;
        for (int i = 0; i < CurPassRecordList.Count; i++)
        {
            TOrderItem curCondtion = CurConList[i]; //현재 상태
            //1. 현재 조건 상태의 TokenType과 동일한지부터 체크 
            if(_adaptItem.Tokentype != curCondtion.Tokentype)
            {
                Debug.LogFormat("적용 타입 {0}, 조건 타입 {1}으로 타입이 다름", _adaptItem.Tokentype, curCondtion.Tokentype);
                continue;
            }
                
            //2. 수행된 토큰타입에 따라 개별 적용 진행
            switch (adaptType)
            {
                case TokenType.Char: //몬스터의 경우 목표 몬스터 처치시 현재 상태 value 1 상승
                    AdaptHunt(_adaptItem, curCondtion, i);
                    break;
                case TokenType.Action:
                    AdaptActionLv(i);
                    break;
                default:
                    AdaptValue(_adaptItem, curCondtion, i);
                    break;
            }

        }
    }
    private void AdaptHunt(TOrderItem _huntMonItem, TOrderItem _curRecord, int _index)
    {
        //잡은 몬스터와 잡아야하는 몬스터 pid가 동일하면 현재 조건에 +1
        if(_curRecord.SubIdx == _huntMonItem.SubIdx)
        {
            TOrderItem newCurCondition = _curRecord;
            newCurCondition.SetValue(_curRecord.Value + 1);
            CurConList[_index] = newCurCondition; //조건 상황이 변했으면 새로 할당
            return;
        }
     }
    private void AdaptActionLv(int _index)
    {
        TOrderItem needAction = CurConList[_index]; //조건값 복사
        List<TokenAction> actionList = PlayerManager.GetInstance().GetPlayerActionList();
        int needActionPid = needAction.SubIdx;
        Debug.Log(_index+"번째 조건인"+needActionPid+"를 보유했는지 체크 현재 보유 유무는 "+needAction.Value);
        //일단 레벨보단 보유로 진행
        for (int i = 0; i < actionList.Count; i++)
        {
            int actionPid = actionList[i].GetPid();
            
            if (needActionPid == actionPid)
            {
                needAction.SetValue(1); //동일한 스킬이 존재하면 해당 조건 벨류 1로 적용
                CurConList[_index] = needAction;
                Debug.LogFormat("필요한 스킬 pid{0}이 존재해서 현재 조건 value를 {1}로 수정",actionPid, CurConList[_index].Value);
            }
                
        }
    }
    private void AdaptValue(TOrderItem _adaptItem, TOrderItem _curRecord, int _index)
    {
        //단순히 최근 값을 현재 상태로 적용하면 되는 경우 
        Debug.LogFormat("적용하려는 Adapt 대상이 {0}타입에 sub{1}인지 체크 기존 밸류는{2}", _curRecord.Tokentype, _curRecord.SubIdx,_curRecord.Value);
        //subIdx 확인후 
        if (_curRecord.SubIdx == _adaptItem.SubIdx)
        {
            //그냥 할당
            CurConList[_index] = _adaptItem;
            Debug.LogFormat("{0}타입 적용할 sub{1}에 {2}로 값 적용\n적용후 값{3}", _adaptItem.Tokentype, _adaptItem.SubIdx, _adaptItem.Value, CurConList[_index].Value);
            return;
        }
        Debug.LogFormat("{0}타입 적용할 sub{1}가 다름",_adaptItem.Tokentype,_adaptItem.SubIdx);
    }
    #endregion

    #region 현재 상태가 성공 상태에 도달했는지 체크
    public bool CheckCondition()
    {
        //현재 상태와 목표 상태를 각 토큰타입에 따라 벨류를 따져봄
        for (int i = 0; i < CurPassRecordList.Count; i++)
        {
            TOrderItem curCondtion = CurConList[i]; //현재 상태
            TokenType conditionType = curCondtion.Tokentype;
            bool isPass = false; //개별 성공 여부
            //수행된 조건 타입에 따라 현재 조건 상태를 변화
            switch (conditionType)
            {
                case TokenType.Char: //몬스터의 경우 목표 몬스터 처치시 현재 상태 value 1 상승
                case TokenType.Action:
                    isPass = IsEnoughValue(i);
                    break;
                default:
                    isPass = IsMatch(i);
                    break;
            }
            //따진 후 성공여부 체크 
            CurPassRecordList[i] = isPass;
        }

        for (int i = 0; i < CurPassRecordList.Count; i++)
        {
            if (CurPassRecordList[i] == false)
                return false;
        }
        return true;
    }
    private bool IsEnoughValue(int _index)
    {
        if (SuccesConList[_index].Value <= CurConList[_index].Value)
            return true;

        return false;
    }
    private bool IsMatch(int _index)
    {
        //조건값과 현재값이 동일하면 되는 경우 
        if (SuccesConList[_index].Value == CurConList[_index].Value)
            return true;

        return false;
    }
    #endregion
}