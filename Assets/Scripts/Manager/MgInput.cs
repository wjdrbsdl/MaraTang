using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MgInput : MonoBehaviour
{
    private bool m_isMouseClick = false;
    [SerializeField]
    private float m_doubleClickInterval = 0.13f; //더블클릭 인정시간
    [SerializeField]
    private float m_dragCutDistance = 55.0f; //드래그로 인정되는 이동거리
    [SerializeField]
    private bool m_isDragMode = false;
    [SerializeField]
    private float m_minDragSpeed = 0.1f;
    [SerializeField]
    private float m_maxDragSpeed = 0.3f;
    public float dragSpeed = 0f;
    private static float m_dragRatioByTileLength; //기본 맵 타일 크기에 따른 드래그 속도 비율
 
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

    #region 클릭
    private void LeftMouse()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //누르기 전
            if (m_isMouseClick == false)
                return;

            //누른 상태로 UI 영역 침범시
            if(m_isMouseClick == true)
            {
                //그냥 초기화
                m_isMouseClick = false;
                m_isDragMode = false;
            }
            //
            return;
        }

        //짧게 눌렀다 때면 클릭
        //길게 누르고 있으면 드래그 

        //1. 눌렀을 때
        if (Input.GetMouseButtonDown(0)&&m_isMouseClick == false)
        {
            m_isMouseClick = true; //눌러졌음으로 바꾸고
            m_isDragMode = false; //초기화
            priorMousePosition = Input.mousePosition;
            return;
        }
        //2. 누른 상태로 - 클릭이냐 드래그냐 가르는 부분
        if(Input.GetMouseButton(0) && m_isMouseClick == true && m_isDragMode == false)
        {
            //마우스 누른상태에서 지속중이면 클릭일지 드래그일지 판별 -> 판별나고 나선 판별 안해도 됨
           if(Vector2.Distance(Input.mousePosition, priorMousePosition) > m_dragCutDistance)
            {
                m_isDragMode = true;
                GamePlayMaster.GetInstance().CamTraceOff();
            }
        }
        //3. 떼었을 때
        if(Input.GetMouseButtonUp(0) && m_isMouseClick == true)
        {
            //마우스를 뗀순간 드래그모드인지 따라서 클릭 진행
            ObjectTokenBase endClickObject = ParseObjectToken(); //뗀 위치에서 오브젝트
            if (m_isDragMode == false)
                ClickTokenObject(endClickObject);
            //클릭 상태는 초기화
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
            //첫 클릭
            //Debug.Log("생 초클릭");
            _clickToken.OnClickObject();
            m_preClickTime = curTime; //누른시간 넣고
            m_preClickToken = _clickToken; //누른 토큰 넣고
            return;
        }

        if(m_preClickToken == _clickToken)
        {
            //만약 같은 토큰을 눌렀다면 누른 시간 간격에 따라 더블클릭 혹은 원클릭으로 진행
            if(curTime - m_preClickTime < m_doubleClickInterval)
            {
                // Debug.Log("더블클릭");
                PlayerManager.GetInstance().DoubleClickTokenObject(_clickToken.GetToken());
            }
            else
            {
                //Debug.Log("원클릭");
                _clickToken.OnClickObject();
            }
            m_preClickTime = curTime; //누른시간 넣고
            m_preClickToken = _clickToken; //누른 토큰 넣고
            return;
        }

        //만약 다른 경우라면
        //첫 클릭으로 진행
       // Debug.Log("다른 원클릭");
        _clickToken.OnClickObject();
        m_preClickTime = curTime; //누른시간 넣고
        m_preClickToken = _clickToken; //누른 토큰 넣고
        return;


    }

    private ObjectTokenBase ParseObjectToken()
    {
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f);
        int maxPri = -1; //최소 선호도는 0 
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

    #region 드래그
    private void DragCam()
    {
        if (m_isDragMode == false)
            return;

        //1. 드래그 방향
        Vector3 direct = new Vector2(priorMousePosition.x - Input.mousePosition.x, priorMousePosition.y - Input.mousePosition.y);
        float dragDistance = direct.magnitude;
        direct = direct.normalized;

        //2.0 드래그 속도에 따른 이동거리 보정 필요 - 휙 당겼을때는 더 땡기도록. 

        //2.1 줌 수치에 따른 속도 조정 - 넓게 보일수록 드래그 길이는 길어져야함.
        //인버스러프 -> 그 구간에서 해당 값이 어느 비율값인지 0~1 사이 값
        float ratio = Mathf.InverseLerp(CameraMove.m_minScopeSize, CameraMove.m_maxScopeSize, Camera.main.orthographicSize);
        //러프 -> 그 구간에서 비율(0~1)에 해당하는 값 min~max 사이 값
        dragSpeed = Mathf.Lerp(m_minDragSpeed, m_maxDragSpeed, ratio); //크기 비율에 따라 속도 변화
        dragSpeed *= m_dragRatioByTileLength;
        
        //3. 드래그 후 카메라 위치 산출
        Vector3 moved = Camera.main.gameObject.transform.position + direct * dragSpeed;

        //4. 카메라 이동 범위 제한 적용
        CamRestrict.RestricCamPos(moved);

        priorMousePosition = Input.mousePosition; //이동한 위치로 갱신 
    }

    public static void SetDragRatio(float _tileRLength)
    {
        m_dragRatioByTileLength = _tileRLength * 0.55f; // 타일 크기에 비례한 속도 증감, 기존 타일 크기 1.5f
    }

    #endregion

    #region 키보드
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
