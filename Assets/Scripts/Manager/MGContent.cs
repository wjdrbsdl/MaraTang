using System.Collections;
using UnityEngine;


public class MGContent 
{
    public static MGContent g_instance;

    public MGContent()
    {
        g_instance = this;
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
        MonsterQuest();
    }

    private void MonsterQuest()
    {
        MgToken.GetInstance().SpawnMonster(new int[] {3,3}, 1);
    }
}
