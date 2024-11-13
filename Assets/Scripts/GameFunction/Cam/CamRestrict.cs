using System.Collections;
using UnityEngine;


public static class CamRestrict
{
    private static float m_camMinX = 5f;
    private static float m_camMinY = 0f;
    private static float m_camMaxX = 0f;
    private static float m_camMaxY = 0f;

    public static void SetCamRestrict()
    {
        int tileXNum = GameUtil.GetMapLength(true);
        int tileYNum = GameUtil.GetMapLength(false);
        m_camMaxX = MgToken.GetInstance().GetMaps()[tileXNum - 1, tileYNum - 1].GetObject().transform.position.x - m_camMinX;
        m_camMaxY = MgToken.GetInstance().GetMaps()[tileXNum - 1, tileYNum - 1].GetObject().transform.position.y;
        // Debug.Log("최고 너비는 " + m_camMaxX + " : " + m_camMaxY);
    }
  
    public static void RestricCamPos(Vector3 _moved)
    {
        //정해진 범위 밖으로 벗어나지 않도록 수정
        _moved.x = Mathf.Max(m_camMinX, _moved.x);
        _moved.x = Mathf.Min(m_camMaxX, _moved.x);
        _moved.y = Mathf.Max(m_camMinY, _moved.y);
        _moved.y = Mathf.Min(m_camMaxY, _moved.y);

        Camera.main.gameObject.transform.position = _moved; //카메라 허용범위 벗어난게 아니라면 위치 이동.
    }

    public static Vector3 CalGamePos(float _xRatio, float _yRatio)
    {
        //어느 공간(미니맵)에서 특정점의 위치 비율을 전체 캠 너비 비율로 환산
        float xPos = (m_camMaxX - m_camMinX) * _xRatio;
        float yPos = (m_camMaxY - m_camMinY) * _yRatio;
        return new Vector3(xPos, yPos, Camera.main.transform.position.z);
    }
}
