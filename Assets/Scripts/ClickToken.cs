using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToken : MonoBehaviour
{
    private bool m_isMouseClick = false;
    [SerializeField]
    private float m_dragCutDistance = 2.0f; //�巡�׷� �����Ǵ� �̵��Ÿ�
    [SerializeField]
    private bool m_isDragMode = false;
    [SerializeField]
    private float m_minMoveSpeed = 0.1f;
    [SerializeField]
    private float m_maxMoveSpeed = 0.3f;
    public float dragSpeed = 0f;

    Vector2 priorMousePosition = new Vector2();
    private void Update()
    {
        LeftMouse();
        InputKey();
        DragCam();
    }

    private void LeftMouse()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //������ ��
            if (m_isMouseClick == false)
                return;

            //���� ���·� UI ���� ħ����
            if(m_isMouseClick == true)
            {
                //�׳� �ʱ�ȭ
                m_isMouseClick = false;
                m_isDragMode = false;
            }
            //
            return;
        }

        //ª�� ������ ���� Ŭ��
        //��� ������ ������ �巡�� 

        //1. ������ ��
        if (Input.GetMouseButtonDown(0)&&m_isMouseClick == false)
        {
            m_isMouseClick = true; //������������ �ٲٰ�
            m_isDragMode = false; //�ʱ�ȭ
            priorMousePosition = Input.mousePosition;
            return;
        }
        //2. ���� ���·� - Ŭ���̳� �巡�׳� ������ �κ�
        if(Input.GetMouseButton(0) && m_isMouseClick == true && m_isDragMode == false)
        {
            //���콺 �������¿��� �������̸� Ŭ������ �巡������ �Ǻ� -> �Ǻ����� ���� �Ǻ� ���ص� ��
           if(Vector2.Distance(Input.mousePosition, priorMousePosition) > m_dragCutDistance)
            {
                m_isDragMode = true;
            }
        }
        //3. ������ ��
        if(Input.GetMouseButtonUp(0) && m_isMouseClick == true)
        {
            //���콺�� ������ �巡�׸������ ���� Ŭ�� ����
            if (m_isDragMode == false)
                CallTokenClick();
            //Ŭ�� ���´� �ʱ�ȭ
            m_isMouseClick = false;
            m_isDragMode = false;
        }
    }

    private void CallTokenClick()
    {
     
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f);
        int maxPri = -1; //�ּ� ��ȣ���� 0 
        ObjectTokenBase clickToken = null;
        for (int i = 0; i < hit.Length; i++)
        {
            ObjectTokenBase tokenObject = hit[i].collider.GetComponent<ObjectTokenBase>();
            if (tokenObject == null)
                continue;

            int curClickPrior = tokenObject.GetClickPrior();
            if (maxPri < curClickPrior)
            {
                maxPri = curClickPrior;
                clickToken = tokenObject;
            }
        }
        //�ֿ켱 �켱���� ������Ʈ�� �ִٸ�
        if (clickToken != null)
            clickToken.OnClickObject();
        
    }
   
    private void DragCam()
    {
        if (m_isDragMode == false)
            return;

        
        Vector2 direct = new Vector2(Input.mousePosition.x - priorMousePosition.x , Input.mousePosition.y - priorMousePosition.y);
        direct = -direct.normalized;
        
        //�ι������� -> �� �������� �ش� ���� ��� ���������� 0~1 ���� ��
        float ratio = Mathf.InverseLerp(CameraMove.m_minScopeSize, CameraMove.m_maxScopeSize, Camera.main.orthographicSize);
        //���� -> �� �������� ����(0~1)�� �ش��ϴ� �� min~max ���� ��
        dragSpeed = Mathf.Lerp(m_minMoveSpeed, m_maxMoveSpeed, ratio); //ũ�� ������ ���� �ӵ� ��ȭ
        Camera.main.gameObject.transform.Translate(direct * dragSpeed);
        priorMousePosition = Input.mousePosition; //�̵��� ��ġ�� ���� 
    }
    
    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }
}
