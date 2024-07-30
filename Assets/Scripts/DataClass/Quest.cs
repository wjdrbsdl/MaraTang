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
    public ContentMasterData ContentData;
    public List<TokenBase> QuestTokens = new(); //퀘스트에 관련된 토큰들 

    #region 생성
    public Quest()
    {
       
    }

    public Quest(ContentMasterData _contentData, int _chunkNum)
    {
        ContentPid = _contentData.ContentPid;
        ContentData = _contentData;
        ChunkNum = _chunkNum;
        SerialNum = MGContent.GetInstance().GetSerialNum();

    }
    #endregion

    public void RealizeStage()
    {
        StageMasterData stage = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        TTokenOrder order = new TTokenOrder(stage.SituationList, stage.SituAdapCount, SerialNum, this);
        OrderExcutor excutor = new OrderExcutor();
        excutor.ExcuteOrder(order);
        MgUI.GetInstance().ShowQuest(this);
    
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

    public void SelectConfirmEvent(SelectItemInfo _selectItemInfo)
    {
        StageMasterData stageInfo = MgMasterData.GetInstance().GetContentData(ContentPid).StageDic[CurStep];
        if ((ConversationEnum)stageInfo.SuccesConList[0].SubIdx == ConversationEnum.Check)
        {
            Debug.Log("대화중에 그냥 확인만 누르면 성공 간주로서 통과");
            ClearStage();
        }
    }

    public void ClearStage()
    {
        ResetSituation();
        StageMasterData stageInfo = MgMasterData.GetInstance().GetStageData(ContentPid, CurStep);
        int nextStep = stageInfo.SuccesStep;
        CurStep = nextStep;
        RealizeStage();
    }

    public void ResetSituation()
    {
        StageMasterData stageInfo = ContentData.StageDic[CurStep];
        TOrderItem doneItem = stageInfo.SituationList[0];
        if (doneItem.Tokentype.Equals(TokenType.OnEvent))
        {
            Debug.Log("추가했던 이벤트 제거");
            PlayerManager.GetInstance().OnChangedPlace.RemoveListener(CharPlaceCheck);
        }
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
            System.Action confirmAction = delegate
            {
                SelectConfirmEvent(selectInfo);
            };
            selectInfo.SetAction(confirmAction);
            MgUI.GetInstance().SetScriptCustomer(selectInfo);
            return;
        }
        if (doneItem.Tokentype.Equals(TokenType.OnEvent))
        {
            Debug.Log("onEvent 조건으로 플레이변환에 이벤트 추가");
            PlayerManager.GetInstance().OnChangedPlace.AddListener(CharPlaceCheck);
        }
    }

    public void CharPlaceCheck()
    {
        StageMasterData stage = ContentData.StageDic[CurStep];
        TOrderItem place = stage.SuccesConList[0];
        Debug.Log("플레이어 어디 입장" + PlayerManager.GetInstance().GetHeroPlace() + "목표" + (TileType)place.Value);
        if (place.Value == (int)PlayerManager.GetInstance().GetHeroPlace())
        {
            Debug.Log("원하는 장소에 입장 성공");
            ClearStage();
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

}