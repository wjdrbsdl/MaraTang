using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGeneric<T> : MonoBehaviour
{
    public static T g_instance;

    public virtual void InitiSet()
    {
        //매니저로서 자신의 static 인스턴스를 생성하고, 자기 내부에서 처리가능한 초기화를 진행

        MakeSingleton();
    }

    public virtual void ReferenceSet()
    {

    }

    public void MakeSingleton()
    {
        //매니저로서 자신의 static 인스턴스를 생성하고, 자기 내부에서 처리가능한 초기화를 진행
        if (g_instance != null)
            return;
        g_instance = gameObject.GetComponent<T>();
    }

    public static T GetInstance()
    {
        return g_instance;
    }
}
