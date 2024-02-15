using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MGContent 
{
    public static MGContent g_instance;
    private ParseData parseData;
    private List<Quest> m_QuestReports = new List<Quest>();
    #region 초기화
    public MGContent()
    {
        g_instance = this;
    }
    public void ReferenceSet()
    {
        parseData = MgParsing.GetInstance().GetMasterData(MasterData.ContentData);

        string matchCode = "";
        for (int i = 0; i < parseData.DbValueList.Count; i++)
        {
            for (int inter = 0; inter < parseData.DbValueList[i].Length; inter++)
            {
                matchCode += parseData.DbValueList[i][inter] + " ";
            }
            matchCode += "\n";
        }
        Debug.Log(matchCode);
    }
    #endregion

    public class Quest
    {
        //과제
        //클리어조건
        //보상을 명기한 컨텐트
        public int QuestPid = 0; //해당 퀘스트 pid mgContet 리스트에 추가되는 값으로 인덱스용

        public void SendQuestResult(TokenBase _token)
        {
            //token의 타입에 따라 결과 코드 생성
            int resultCode = 0;
            TokenType type = _token.GetTokenType();
            if (type.Equals(TokenType.Char))
            {
                //몬스터의 경우 토큰의 상태에 따라 코드를 만들어서 전달 - 즉 죽었을 경우, 어떤 상태의 경우등에 따라 코드를 정의 해놔야함.
                resultCode = 5;
            }
            CallResultCode(_token, resultCode);
        }

        public void CallResultCode(TokenBase _token, int _concludeCode)
        {
            //몬스터 사망시 알림받는 장소 
            Debug.Log(_token.GetItemName() + " 토큰" + _concludeCode + "코드 호출");
            CheckQuestComplete();
        }

        private void CheckQuestComplete()
        {
            //토큰의 호출시 마다 결과 코드를 기록하고 퀘스트 완료 여부를 체크한다. 

        }
    }

    public enum ContentEnum
    {
        진행턴, 발생컨텐츠
    }


    public void NextWorldTurn()
    {
        //진행도나 무언가에 따라서 컨텐츠 발생

        /*컨텐츠는 
         * - 해당 지역 몬스터 제거하기 
         * - 해당 지역 점거 하기 
         * - 해당 지역 자원 캐기 등 특정한 이벤트를 발생시키고, 그에 따라 보상을 약속하는 시스템 
         * 
        */

        //테스트용, 3턴 마다 특정 청크 n개에 몬스터 발생 
        //기존 발생되었던 녀석은 어떻게? 
        //1. 기존께 강화
        //2. 기존께 소멸
        //3. 기존꺼 유지 
        Debug.Log("컨텐츠 활성화");
        SelectContent();
    }

    private void SelectContent()
    {
        //미리 세팅해둔 컨텐츠 테이블에 따라 수행할것

        //임시로 3턴 마다 몬스터 퀘스트 수행하도록 
        GamePlayData data = GamePlayMaster.GetInstance().GetPlayData();
        if (data.PlayTime % 3 == 0)
            MonsterQuest();
    }

    private void MonsterQuest()
    {
        //스폰 시킨몬스터를 퀘스트 몬스터로
        Quest newQuest = new();
        m_QuestReports.Add(newQuest);
        newQuest.QuestPid = m_QuestReports.Count - 1;
        Debug.Log("몬스터 소환 컨텐츠");
       TokenChar questMonster = MgToken.GetInstance().SpawnMonster(new int[] {3,3}, 1); //몬스터의 경우 사망시에 설치
        questMonster.QuestCard = newQuest;
        //컨텐츠 이벤트 등록은 여기서 따로 시행

    }
}
