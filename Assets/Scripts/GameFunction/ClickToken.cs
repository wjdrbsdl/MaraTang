using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToken : MonoBehaviour
{
    private bool m_isMouseClick = false;
    [SerializeField]
    private float m_doubleClickInterval = 0.13f; //����Ŭ�� �����ð�
    [SerializeField]
    private float m_dragCutDistance = 6.0f; //�巡�׷� �����Ǵ� �̵��Ÿ�
    [SerializeField]
    private bool m_isDragMode = false;


   [SerializeField]
    private float m_minDragSpeed = 0.1f;
    [SerializeField]
    private float m_maxDragSpeed = 0.3f;
    public float dragSpeed = 0f;
    private float m_dragRatioByTileLength; //�⺻ �� Ÿ�� ũ�⿡ ���� �巡�� �ӵ� ����
    private static float m_camMinX = 5f;
    private static float m_camMinY = 0f;
    private static float m_camMaxX = 0f;
    private static float m_camMaxY = 0f;

    

    Vector2 priorMousePosition = new Vector2();

    

    private void Start()
    {
        float tileRLength = MgToken.GetInstance().m_rLength; //�� Ÿ�� ������
        m_dragRatioByTileLength = tileRLength * 0.55f; // Ÿ�� ũ�⿡ ����� �ӵ� ����, ���� Ÿ�� ũ�� 1.5f
        int tileXNum = GameUtil.GetMapLength(true);
        int tileYNum = GameUtil.GetMapLength(false);
        m_camMaxX = MgToken.GetInstance().GetMaps()[tileXNum - 1, tileYNum - 1].GetObject().transform.position.x - m_camMinX;
        m_camMaxY = MgToken.GetInstance().GetMaps()[tileXNum - 1, tileYNum - 1].GetObject().transform.position.y;
        Debug.Log("�ְ� �ʺ�� " + m_camMaxX + " : " + m_camMaxY);
    }

    private void Update()
    {
        LeftMouse();
        InputKey();
        DragCam();
        InputCancle();
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
                GamePlayMaster.GetInstance().CamTraceOff();
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

    private ObjectTokenBase m_preClickToken = null;
    private float m_preClickTime =0f;
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

            int curClickPrior = tokenObject.GetClickPriority();
            if (maxPri < curClickPrior)
            {
                maxPri = curClickPrior;
                clickToken = tokenObject;
            }
        }
        if (clickToken == null)
            return;

        float curTime = Time.realtimeSinceStartup;

        if(m_preClickToken == null)
        {
            //ù Ŭ��
            //Debug.Log("�� ��Ŭ��");
            clickToken.OnClickObject();
            m_preClickTime = curTime; //�����ð� �ְ�
            m_preClickToken = clickToken; //���� ��ū �ְ�
            return;
        }

        if(m_preClickToken == clickToken)
        {
            //���� ���� ��ū�� �����ٸ� ���� �ð� ���ݿ� ���� ����Ŭ�� Ȥ�� ��Ŭ������ ����
            if(curTime - m_preClickTime < m_doubleClickInterval)
            {
                // Debug.Log("����Ŭ��");
                PlayerManager.GetInstance().DoubleClickTokenObject(clickToken.GetToken());
            }
            else
            {
                //Debug.Log("��Ŭ��");
                clickToken.OnClickObject();
            }
            m_preClickTime = curTime; //�����ð� �ְ�
            m_preClickToken = clickToken; //���� ��ū �ְ�
            return;
        }

        //���� �ٸ� �����
        //ù Ŭ������ ����
       // Debug.Log("�ٸ� ��Ŭ��");
        clickToken.OnClickObject();
        m_preClickTime = curTime; //�����ð� �ְ�
        m_preClickToken = clickToken; //���� ��ū �ְ�
        return;


    }
   
    private void DragCam()
    {
        if (m_isDragMode == false)
            return;

        
        Vector3 direct = new Vector2(Input.mousePosition.x - priorMousePosition.x , Input.mousePosition.y - priorMousePosition.y);
        direct = -direct.normalized;
        
        //�ι������� -> �� �������� �ش� ���� ��� ���������� 0~1 ���� ��
        float ratio = Mathf.InverseLerp(CameraMove.m_minScopeSize, CameraMove.m_maxScopeSize, Camera.main.orthographicSize);
        //���� -> �� �������� ����(0~1)�� �ش��ϴ� �� min~max ���� ��
        dragSpeed = Mathf.Lerp(m_minDragSpeed, m_maxDragSpeed, ratio); //ũ�� ������ ���� �ӵ� ��ȭ
        dragSpeed *= m_dragRatioByTileLength;
        //Camera.main.gameObject.transform.Translate(direct * dragSpeed);

        Vector3 moved = Camera.main.gameObject.transform.position + direct * dragSpeed;

        RestrictCamPos(moved);


        priorMousePosition = Input.mousePosition; //�̵��� ��ġ�� ���� 
    }
    
    public static void RestrictCamPos(Vector3 _moved)
    {
        //������ ���� ������ ����� �ʵ��� ����
        _moved.x = Mathf.Max(m_camMinX, _moved.x);
        _moved.x = Mathf.Min(m_camMaxX, _moved.x);
        _moved.y = Mathf.Max(m_camMinY, _moved.y);
        _moved.y = Mathf.Min(m_camMaxY, _moved.y);

        Camera.main.gameObject.transform.position = _moved; //ī�޶� ������ ����� �ƴ϶�� ��ġ �̵�.
    }

    private void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }

    private void InputCancle()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            PlayerManager.GetInstance().ClickCancle();
    }
}
