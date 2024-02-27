using System.Collections;
using UnityEngine;


public class Announcer
{
    public static Announcer Instance;

    public Announcer()
    {
        Instance = this;
    }

    //플레이 기록을 모아놓고,

    //**로그 보고서
    //1. 컷씬의 간이 보고서 - A 공격 실행 B 어택, B 결과물, 추가 리워드 따라랑
    //2. 턴종료 보고서 - 세계 변화 보고 - X의 발생과 종료
    //3. 턴시작 보고서 - 점령된 영지 혹은 진행 중이던 연구의 경과 보고 - 남은경우 : 박스의 발견 1턴 // 달성경우 : 박스가 발견되었다.


    public void AnnounceState(string message, bool isShow = false)
    {
        TokenBase chart = PlayerManager.GetInstance().GetMainChar();
        if(isShow)
        MgCharScript.GetInstance().PlayScript(chart, message);
    }
}
