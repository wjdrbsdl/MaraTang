using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgGeneric<T> : MonoBehaviour
{
    public static T g_instance;

    public virtual void InitiSet()
    {
        //�Ŵ����μ� �ڽ��� static �ν��Ͻ��� �����ϰ�, �ڱ� ���ο��� ó�������� �ʱ�ȭ�� ����
    }

}
