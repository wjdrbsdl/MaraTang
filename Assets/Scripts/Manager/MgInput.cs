using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MgInput : MonoBehaviour
{
    private bool m_isMouseClick = false;
    [SerializeField]
    private float m_doubleClickInterval = 0.13f; //����Ŭ�� �����ð�
    [SerializeField]
    private float m_dragCutDistance = 55.0f; //�巡�׷� �����Ǵ� �̵��Ÿ�
    [SerializeField]
    private bool m_isDragMode = false;
    [SerializeField]
    private float m_minDragSpeed = 0.1f;
    [SerializeField]
    private float m_maxDragSpeed = 0.3f;
    public float dragSpeed = 0f;
    private static float m_dragRatioByTileLength; //�⺻ �� Ÿ�� ũ�⿡ ���� �巡�� �ӵ� ����
 
    Vector2 priorMousePosition = new Vector2();
    KeyCode[] inputNum = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
    KeyCode mainCharCam = KeyCode.Space;

    private void Update()
    {
        LeftMouse();
        DragCam();
        InputCancle();
        InputKeyBoard();
    }

    #region Ŭ��
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
            ObjectTokenBase endClickObject = ParseObjectToken(); //�� ��ġ���� ������Ʈ
            if (m_isDragMode == false)
                ClickTokenObject(endClickObject);
            //Ŭ�� ���´� �ʱ�ȭ
            m_isMouseClick = false;
            m_isDragMode = false;
        }
    }

    private ObjectTokenBase m_preClickToken = null;
    private float m_preClickTime =0f;
    private void ClickTokenObject(ObjectTokenBase _clickToken)
    {
        if(_clickToken == null)
                return;
        float curTime = Time.realtimeSinceStartup;

        if(m_preClickToken == null)
        {
            //ù Ŭ��
            //Debug.Log("�� ��Ŭ��");
            _clickToken.OnClickObject();
            m_preClickTime = curTime; //�����ð� �ְ�
            m_preClickToken = _clickToken; //���� ��ū �ְ�
            return;
        }

        if(m_preClickToken == _clickToken)
        {
            //���� ���� ��ū�� �����ٸ� ���� �ð� ���ݿ� ���� ����Ŭ�� Ȥ�� ��Ŭ������ ����
            if(curTime - m_preClickTime < m_doubleClickInterval)
            {
                // Debug.Log("����Ŭ��");
                PlayerManager.GetInstance().DoubleClickTokenObject(_clickToken.GetToken());
            }
            else
            {
                //Debug.Log("��Ŭ��");
                _clickToken.OnClickObject();
            }
            m_preClickTime = curTime; //�����ð� �ְ�
            m_preClickToken = _clickToken; //���� ��ū �ְ�
            return;
        }

        //���� �ٸ� �����
        //ù Ŭ������ ����
       // Debug.Log("�ٸ� ��Ŭ��");
        _clickToken.OnClickObject();
        m_preClickTime = curTime; //�����ð� �ְ�
        m_preClickToken = _clickToken; //���� ��ū �ְ�
        return;


    }

    private ObjectTokenBase ParseObjectToken()
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
            return null;

        return clickToken;
    }
    #endregion

    #region �巡��
    private void DragCam()
    {
        if (m_isDragMode == false)
            return;

        //1. �巡�� ����
        Vector3 direct = new Vector2(priorMousePosition.x - Input.mousePosition.x, priorMousePosition.y - Input.mousePosition.y);
        float dragDistance = direct.magnitude;
        direct = direct.normalized;

        //2.0 �巡�� �ӵ��� ���� �̵��Ÿ� ���� �ʿ� - �� ��������� �� ���⵵��. 

        //2.1 �� ��ġ�� ���� �ӵ� ���� - �а� ���ϼ��� �巡�� ���̴� ���������.
        //�ι������� -> �� �������� �ش� ���� ��� ���������� 0~1 ���� ��
        float ratio = Mathf.InverseLerp(CameraMove.m_minScopeSize, CameraMove.m_maxScopeSize, Camera.main.orthographicSize);
        //���� -> �� �������� ����(0~1)�� �ش��ϴ� �� min~max ���� ��
        dragSpeed = Mathf.Lerp(m_minDragSpeed, m_maxDragSpeed, ratio); //ũ�� ������ ���� �ӵ� ��ȭ
        dragSpeed *= m_dragRatioByTileLength;
        
        //3. �巡�� �� ī�޶� ��ġ ����
        Vector3 moved = Camera.main.gameObject.transform.position + direct * dragSpeed;

        //4. ī�޶� �̵� ���� ���� ����
        CamRestrict.RestricCamPos(moved);

        priorMousePosition = Input.mousePosition; //�̵��� ��ġ�� ���� 
    }

    public static void SetDragRatio(float _tileRLength)
    {
        m_dragRatioByTileLength = _tileRLength * 0.55f; // Ÿ�� ũ�⿡ ����� �ӵ� ����, ���� Ÿ�� ũ�� 1.5f
    }

    #endregion

    #region Ű����
    private void InputKeyBoard()
    {
       
        for (int i = 0; i < inputNum.Length; i++)
        {
            if (Input.GetKeyDown(inputNum[i]))
            {
                PlayerManager.GetInstance().InputActionSlot(i);
            }
        }
        if (Input.GetKeyDown(mainCharCam))
        {
            GamePlayMaster.GetInstance().CamFocusMainChar();
        }
    }
    #endregion

    private void InputCancle()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            PlayerManager.GetInstance().ClickCancle();
    }
}
