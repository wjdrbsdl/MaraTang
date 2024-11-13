using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //기존 스크린 비율 너비
    private const float c_xWide = 1920f;
    [SerializeField]
    public const float m_minScopeSize = 6f;
    [SerializeField]
    public const float m_maxScopeSize = 15f;

    [SerializeField]
    private float m_restrictSize = 85f;
    [SerializeField]
    private float m_moveSpeed = 0.5f;
    [SerializeField]
    private float m_scopeSpeed = 0.5f;

    public bool OnBoundaryMove = true;

    public CameraFollow m_follow;

    private void Update()
    {
        ScopeCamera();
        if (Input.GetKeyDown(KeyCode.F6) && m_follow != null)
            m_follow.MoveToTarget();
            
    }

    private bool CheckBoundary()
    {
        if (OnBoundaryMove == false)
            return false;
        //현재 마우스 커서의 위치가 겜 화면에서 경계선인지 체크 
        float x = Screen.width;
        float y = Screen.height;
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        float restrictSize = m_restrictSize * (x / c_xWide); //해상도에 따라 커서 구역 변화
        if(mouseX < 0 || mouseY < 0 || x < mouseX || y < mouseY)
        {
            //화면 스크린 벗어난 경우 false
            return false;
        }

        //스크린 내부에서 제한 구역에 있는 경우 true
        if(mouseX< restrictSize
          || mouseY< restrictSize
          || (x- restrictSize) <mouseX
          ||(y - restrictSize) < mouseY)
        {
            return true;
        }

        return false;
    }

    private void MoveCamToMouse()
    {
        float centerX = Screen.width/2;
        float centerY = Screen.height/2;
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        Vector2 direct = new Vector2(centerX - mouseX, centerY - mouseY);
        direct = -direct.normalized;

        Camera.main.gameObject.transform.Translate(direct*m_moveSpeed);
    }

    private void ScopeCamera()
    {
      
            //줌 정도를 받아서
            float zoom = Input.GetAxis("Mouse ScrollWheel") * -m_scopeSpeed;
            if (zoom != 0)
            {
                SizeZoom(zoom);

            }
       
    }

    private void SizeZoom(float zoom)
    {
        float size = Camera.main.orthographicSize;
        size += zoom;
        if (size < m_minScopeSize || m_maxScopeSize < size)
            return;
        Camera.main.orthographicSize = size;

    }
}
