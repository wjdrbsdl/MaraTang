using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    //과제
    //클리어조건
    //보상을 명기한 컨텐트
    public int QuestPid = 0; //해당 퀘스트 pid mgContet 리스트에 추가되는 값으로 인덱스용
    public int RestWoldTurn = 5; //유지되는 기간 
    public int TempMissonType = 5; //수행 조건 
    public int TempCompleteCode = 5; //완료 조건
    public RewardData reward; //보상
    public List<TokenBase> TempQuestTokens; //퀘스트에 관련된 토큰들 

    #region 생성
    public Quest()
    {
        reward = new RewardData(); //임시로 자원 보상
    }
    #endregion

    public void RemoveTurn(int _count = 1)
    {
        RestWoldTurn -= 1;
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
        //몬스터 사망시 알림받는 장소 
        Debug.Log(_token.GetItemName() + " 토큰" + _concludeCode + "코드 호출");
        CheckQuestComplete();
    }

    private void CheckQuestComplete()
    {
        //토큰의 호출시 마다 결과 코드를 기록하고 퀘스트 완료 여부를 체크한다. 
        Debug.Log("퀘스트 수행 성공 보상 진행");
        if (true)
            GamePlayMaster.GetInstance().RewardQuest(reward);
    }
}
