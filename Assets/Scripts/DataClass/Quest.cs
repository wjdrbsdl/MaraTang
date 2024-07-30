using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : IOrderCustomer
{
    //과제
    //클리어조건
    //보상을 명기한 컨텐트
    public int ContentPid = 0; //content pid
    public int SerialNum = 0;
    public int RestWoldTurn = 3; //유지되는 기간 
    public int ChunkNum = 0;
    public int CurStep = 1;
    public List<TokenBase> QuestTokens = new(); //퀘스트에 관련된 토큰들 
    public CurStageData CurStageData;

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

    public void FlowTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    private enum QuestCode
    {
        MonsterDie, EventActive
    }

    public void SendQuestCallBack(TokenBase _token)
    {
        //각 토큰에서 개별적으로 자신의 상태를 콜백함. 
        QuestCode resultCode = QuestCode.MonsterDie;
        TokenType type = _token.GetTokenType();

        //1. 토큰 타입에 따라 현재 토큰의 상태에 따라 콜백 코드를 생성
        if (type.Equals(TokenType.Char))
        {
            //몬스터의 경우 토큰의 상태에 따라 코드를 만들어서 전달 - 즉 죽었을 경우, 어떤 상태의 경우등에 따라 코드를 정의 해놔야함.
            resultCode = QuestCode.MonsterDie;
        }
        else if (type.Equals(TokenType.Event))
        {
            Debug.Log("이벤트 입장 했음 알림" + QuestTokens.IndexOf(_token));
            resultCode = QuestCode.EventActive;
        }
        //2. 생성된 코드를 토큰과 함께 전달
        FindCallBackCode(_token, resultCode);
    }

    public void ClearStage()
    {
        Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 클리어 됨", SerialNum, CurStep);
        ResetSituation();
        int nextStep = CurStageData.SuccesStep;
        CurStep = nextStep;
        RealizeStage();
    }

    public void ResetSituation()
    {
        Debug.LogFormat("시리얼 넘버{0} 퀘 {1}스테이지 리셋", SerialNum, CurStep);
        StageMasterData stageInfo = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        TOrderItem doneItem = stageInfo.SituationList[0];
    }

    public void CleanQuest()
    {
        //퀘스트 부산물 정리하는 부분
        //1. 사냥 퀘스트의 경우 남은 몬스터에 대한 정리 
        for (int i = 0; i < QuestTokens.Count; i++)
        {
            QuestTokens[i].CleanToken();
        }
    }

    private void FindCallBackCode(TokenBase _token, QuestCode _concludeCode)
    {
        //전달받은 코드와 토큰으로 해당 퀘스트에 어떻게 적용할지 정의
   
        //현재는 몬스터 사망으로 잡아야할 몬스터 리스트에서 제거하는 중
        if(_concludeCode.Equals(QuestCode.MonsterDie))
           QuestTokens.Remove(_token);
        else if (_concludeCode.Equals(QuestCode.EventActive))
        {
            //이벤트 퀘스트의 경우 하나라도 수행되었으면 성공처리
            MGContent.GetInstance().SuccessQuest(this); //
            return;
        }

        bool isComplete = CheckQuestComplete();
        //퀘스트 평가중 성공했다면
        if (isComplete)
        {
            MGContent.GetInstance().SuccessQuest(this);
        }
            
    }

    private bool CheckQuestComplete()
    {
        //토큰의 호출시 마다 결과 코드를 기록하고 퀘스트 완료 여부를 체크한다. 
        if (QuestTokens.Count == 0)
        {
            return true;
        }
        return false;
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
                Debug.LogFormat("{0}타입이 다름", _adaptItem.Tokentype);
                continue;
            }
                
            //2. 수행된 토큰타입에 따라 개별 적용 진행
            switch (adaptType)
            {
                case TokenType.Char: //몬스터의 경우 목표 몬스터 처치시 현재 상태 value 1 상승
                    AdaptHunt(_adaptItem, curCondtion, i);
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
                    isPass = IsEnoughHunt(i);
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
    private bool IsEnoughHunt(int _index)
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