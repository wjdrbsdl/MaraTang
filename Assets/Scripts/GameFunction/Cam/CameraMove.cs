using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    //���� ��ũ�� ���� �ʺ�
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
        //���� ���콺 Ŀ���� ��ġ�� �� ȭ�鿡�� ��輱���� üũ 
        float x = Screen.width;
        float y = Screen.height;
        float mouseX = Input.mousePosition.x;
        float mouseY = Input.mousePosition.y;
        float restrictSize = m_restrictSize * (x / c_xWide); //�ػ󵵿� ���� Ŀ�� ���� ��ȭ
        if(mouseX < 0 || mouseY < 0 || x < mouseX || y < mouseY)
        {
            //ȭ�� ��ũ�� ��� ��� false
            return false;
        }

        //��ũ�� ���ο��� ���� ������ �ִ� ��� true
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
      
            //�� ������ �޾Ƽ�
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
