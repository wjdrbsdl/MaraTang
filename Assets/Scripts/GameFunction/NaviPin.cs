using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviPin : MonoBehaviour
{
    public Vector2 naviPoint; //�����ִ� �� 
    private Camera m_mainCam;
    private void Start()
    {
        m_mainCam = Camera.main;
    }

    private void Update()
    {
        Navigate();
    }

    private void Navigate()
    {
        //ī�޶� �信 ����������
        IsInCamevaView();
        //��ġ�� �Ȱ� �״�� ����
        //ī�޶� �信 ����������
        //ī�޶� �����ǿ��� point ������ ��Ŀ�� ���ο� ��ġ 
    }

    private bool IsInCamevaView()
    {
        Vector2 screenPoint = m_mainCam.WorldToViewportPoint(naviPoint);
        bool isIn = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        Debug.Log(isIn + " "+screenPoint);
        return true;
    }
}
