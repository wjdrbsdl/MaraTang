using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    //과제
    //클리어조건
    //보상을 명기한 컨텐트
    public int QuestPid = 0; //해당 퀘스트 pid mgContet 리스트에 추가되는 값으로 인덱스용
    public int RestWoldTurn = 15; //유지되는 기간 
    public int ChunkNum = 0;
    public QuestCondition Condition; //수행 조건
    public RewardData Reward; //보상
    public PenaltyData Penalty;
    public List<TokenBase> TempQuestTokens = new(); //퀘스트에 관련된 토큰들 

    #region 생성
    public Quest()
    {
        Reward = new RewardData(RewardType.None); //임시로 자원 보상
    }

    public Quest(int _monsterPID, int _monsterCount, RewardType _rewardType)
    {
        QuestPid = MGContent.g_instance.GetQuestList().Count;
        Condition = new QuestCondition(_monsterPID, _monsterCount);
        Reward = new RewardData(_rewardType); //임시로 자원 보상
        Penalty = new PenaltyData();
    }
    #endregion

    public void RemoveTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
        if (RestWoldTurn == 0)
            MGContent.GetInstance().FailQuest(this);
    }

    public void SendQuestCallBack(TokenBase _token)
    {
        //token의 타입에 따라 결과 코드 생성
        int resultCode = 0;
        TokenType type = _token.GetTokenType();
        if (type.Equals(TokenType.Char))
        {
            //몬스터의 경우 토큰의 상태에 따라 코드를 만들어서 전달 - 즉 죽었을 경우, 어떤 상태의 경우등에 따라 코드를 정의 해놔야함.
            resultCode = 5;
        }
        CheckCallBackCode(_token, resultCode);
    }

    public void CheckCallBackCode(TokenBase _token, int _concludeCode)
    {
        //전달받은 코드로, 퀘스트 조건값 수정 
   
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
    //몬스터 관련 조건
    public int monsterPID;
    public int monsterCount;

    //수행해야할 액션 조건

    //득템해야할 아이템 조건

    public QuestCondition(int _monsterPID, int _monsterCount)
    {
        monsterPID = _monsterPID;
        monsterCount = _monsterCount;
    }

}