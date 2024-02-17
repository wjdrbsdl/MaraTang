using System.Collections;
using UnityEngine;


public class Mg<T>
{
    public static T g_instance;

    public virtual void InitiSet()
    {
        //매니저로서 자신의 static 인스턴스를 생성하고, 자기 내부에서 처리가능한 초기화를 진행

    }

    public virtual void ReferenceSet()
    {

    }

    public static T GetInstance()
    {
        return g_instance;
    }
}
