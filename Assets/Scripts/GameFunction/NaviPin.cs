using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviPin : MonoBehaviour
{
    public Vector2 naviPoint; //꽂혀있는 곳 
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
        //카메라 뷰에 들어와있으면
        IsInCamevaView();
        //위치에 꽂고 그대로 리턴
        //카메라 뷰에 나가있으면
        //카메라 포지션에서 point 방향의 포커스 라인에 위치 
    }

    private bool IsInCamevaView()
    {
        Vector2 screenPoint = m_mainCam.WorldToViewportPoint(naviPoint);
        bool isIn = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        Debug.Log(isIn + " "+screenPoint);
        return true;
    }
}
