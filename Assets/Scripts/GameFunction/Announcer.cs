using System.Collections;
using UnityEngine;


public class Announcer
{
    public static Announcer Instance;

    public Announcer()
    {
        Instance = this;
    }

    public void TestInstance()
    {
        Debug.Log("테스트 완료");
    }

    public void AnnounceState(string message)
    {

    }
}
