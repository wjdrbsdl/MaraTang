using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGeneric<T> : MonoBehaviour
{
    public static T g_instance;

    public virtual void InitiSet()
    {
        //�Ŵ����μ� �ڽ��� static �ν��Ͻ��� �����ϰ�, �ڱ� ���ο��� ó�������� �ʱ�ȭ�� ����

        MakeSingleton();
    }

    public virtual void ReferenceSet()
    {

    }

    public void MakeSingleton()
    {
        //�Ŵ����μ� �ڽ��� static �ν��Ͻ��� �����ϰ�, �ڱ� ���ο��� ó�������� �ʱ�ȭ�� ����
        if (g_instance != null)
            return;
        g_instance = gameObject.GetComponent<T>();
    }

    public static T GetInstance()
    {
        return g_instance;
    }
}
