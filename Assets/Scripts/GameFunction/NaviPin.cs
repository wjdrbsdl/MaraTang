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

    private void LateUpdate()
    {
        Navigate();
    }

    private void Navigate()
    {
        //카메라 뷰에 들어와있으면
        if (IsInCamevaView())
        {
            transform.position = naviPoint;
            return;
        }

        Vector2 dirVec = screenPoint - new Vector2(0.5f, 0.5f); //뷰 센터에서 목적지 까지의 벡터
        float ratio = Scalar(dirVec); //뷰 센터내 벡터 비율 산출
        float padding = 0.95f; //뷰 경계선으로 부터 줄 패딩
        dirVec.x *= ratio * padding; //목적지 까지 벡터에 비율과 패딩을 곱해서 뷰 포트내 벡터 산출
        dirVec.y *= ratio * padding;
        
        Vector2 posVec = new Vector2(0.5f, 0.5f) + dirVec; //센터에서 산출된 벡터를 더해서 위치값 계산
        //Debug.Log("비율 적용 벡터" + dirVec + "위치 벡터" + posVec);
        Vector2 viewVec = m_mainCam.ViewportToWorldPoint(posVec); //뷰포트 값을 월드 좌표상으로 옮겨서 화면에 표시 - 그냥 뷰포트 좌표 대로 해도되겠는데
        transform.position = viewVec; //산출된 월드좌표로 핀 이동

    }

    private float Scalar(Vector2 _dir)
    {
        //  Debug.Log("스크린 포인트 " + screenPoint + "수정전 벡터" + _dir);
        float gap = Mathf.Abs(0.5f - screenPoint.x);
        if (Mathf.Abs(_dir.x) < Mathf.Abs(_dir.y))
          gap = Mathf.Abs(0.5f - screenPoint.y);

        if (gap == 0)
            gap = 0.0001f;

        float ratio = 0.5f / gap;
        return ratio;
    }

    Vector2 screenPoint;
    private bool IsInCamevaView()
    {
        screenPoint = m_mainCam.WorldToViewportPoint(naviPoint);
        bool isIn = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
       // Debug.Log(isIn + " "+screenPoint);
        return isIn;
    }
}
