using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EQuestType
{
    SpawnMonster, SpawnEvent
}

public enum ERewardType
{
    CharStat, Capital, ActionToken, EventToken, CharToken
}

public class Quest : IOrderCustomer
{
    //과제
    //클리어조건
    //보상을 명기한 컨텐트
    public int QuestPid = 0; //해당 퀘스트 pid mgContet 리스트에 추가되는 값으로 인덱스용
    public int RestWoldTurn = 3; //유지되는 기간 
    public int ChunkNum = 0;
    public QuestCondition Condition; //수행 조건
    public RewardData Reward; //보상
    public PenaltyData Penalty;
    public List<TokenBase> TempQuestTokens = new(); //퀘스트에 관련된 토큰들 

    #region 생성
    public Quest()
    {
        Reward = new RewardData(EOrderType.None); //임시로 자원 보상
    }

    public Quest(int _monsterPID, int _monsterCount, EOrderType _rewardType, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        QuestPid = MGContent.g_instance.m_questCount;
        ChunkNum = _chunkNum;
        Condition = new QuestCondition(_monsterPID, _monsterCount);
        Reward = new RewardData(_rewardType, _chunkNum); //임시로 자원 보상
        Penalty = new PenaltyData();
    }

    public Quest(EQuestType _questType, ERewardType _rewardType, int _chunkNum)
    {
        QuestPid = MGContent.g_instance.m_questCount;
        ChunkNum = _chunkNum;
        //퀘스트 타입에 맞게 조건서 작성
        Condition = new QuestCondition(_questType, _chunkNum);
        //퀘스트 조건의 주문서에 콜백 대상으로 자신을 할당 
        Condition.TokenOrder.SetOrderCustomer(this);
        //보상 타입에 맞게 보상내용 작성
        Reward = new RewardData(_rewardType, _chunkNum); //임시로 자원 보상
        Penalty = new PenaltyData();
    }

    public void OrderCallBack(OrderReceipt _orderReceipt)
    {
        //오더익스큐터로 생성된 토큰들을 콜백받으면 거기에 자신을 할당 
        TokenBase tokens = _orderReceipt.madeToken;
        if (tokens == null)
            return;
        tokens.QuestPid = QuestPid;
    }

    #endregion

    public void UseTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    public void SendQuestCallBack(TokenBase _token)
    {
        //각 토큰에서 개별적으로 자신의 상태를 콜백함. 
        int resultCode = 0;
        TokenType type = _token.GetTokenType();

        //1. 토큰 타입에 따라 현재 토큰의 상태에 따라 콜백 코드를 생성
        if (type.Equals(TokenType.Char))
        {
            //몬스터의 경우 토큰의 상태에 따라 코드를 만들어서 전달 - 즉 죽었을 경우, 어떤 상태의 경우등에 따라 코드를 정의 해놔야함.
            resultCode = 5;
        }
        //2. 생성된 코드를 토큰과 함께 전달
        FindCallBackCode(_token, resultCode);
    }

    public void CleanQuest()
    {
        //퀘스트 부산물 정리하는 부분
        //1. 사냥 퀘스트의 경우 남은 몬스터에 대한 정리 
        for (int i = 0; i < TempQuestTokens.Count; i++)
        {
            TempQuestTokens[i].Clean();
        }
    }

    private void FindCallBackCode(TokenBase _token, int _concludeCode)
    {
        //전달받은 코드와 토큰으로 해당 퀘스트에 어떻게 적용할지 정의
   
        //현재는 몬스터 사망으로 잡아야할 몬스터 리스트에서 제거하는 중
        TempQuestTokens.Remove((TokenChar)_token);
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
        if (TempQuestTokens.Count == 0)
        {
            return true;
        }
        return false;
    }

    
}
public enum QuestType
{
    Battle, Action, Item
}

public class QuestCondition
{
    //퀘스트 조건
    public EQuestType QuestType;
    public TTokenOrder TokenOrder;
    public int monsterPID;
    public int monsterCount;

    //수행해야할 액션 조건

    //득템해야할 아이템 조건

    public QuestCondition(int _monsterPID, int _monsterCount)
    {
        monsterPID = _monsterPID;
        monsterCount = _monsterCount;
    }

    public QuestCondition(EQuestType _questType, int _chunkNum)
    {
        //퀘스트 타입에 따라서 조건 내용을 채우기 
        QuestType = _questType;
        switch (_questType)
        {
            //이벤트토큰 생성하는 경우
            case EQuestType.SpawnEvent:
                TokenOrder = new TTokenOrder(EOrderType.SpawnEvent,1,1);
                break;
            case EQuestType.SpawnMonster:
                //어떤 몬스터를 얼마나 어떤식으로 소환할지 필요
                int tempPid = 2;
                int tempCount = 3;
                ESpawnPosType tempSpawnPos = ESpawnPosType.Random;
                TokenOrder = new TTokenOrder().Spawn(EOrderType.SpawnMonster, tempPid, tempCount, tempSpawnPos, _chunkNum);
                break;
        }
        
    }

}