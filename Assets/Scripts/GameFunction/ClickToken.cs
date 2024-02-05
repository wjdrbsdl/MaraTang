using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToken : MonoBehaviour
{
    private bool m_isMouseClick = false;
    [SerializeField]
    private float m_doubleClickInterval = 0.13f; //더블클릭 인정시간
    [SerializeField]
    private float m_dragCutDistance = 6.0f; //드래그로 인정되는 이동거리
    [SerializeField]
    private bool m_isDragMode = false;


   [SerializeField]
    private float m_minDragSpeed = 0.1f;
    [SerializeField]
    private float m_maxDragSpeed = 0.3f;
    public float dragSpeed = 0f;
    private float m_dragRatioByTileLength; //기본 맵 타일 크기에 따른 드래그 속도 비율
    private static float m_camMinX = 5f;
    private static float m_camMinY = 0f;
    private static float m_camMaxX = 0f;
    private static float m_camMaxY = 0f;

    

    Vector2 priorMousePosition = new Vector2();

    

    private void Start()
    {
        float tileRLength = MgToken.GetInstance().m_rLength; //맵 타일 반지름
        m_dragRatioByTileLength = tileRLength * 0.55f; // 타일 크기에 비례한 속도 증감, 기존 타일 크기 1.5f
        int tileXNum = GameUtil.GetMapLength(true);
        int tileYNum = GameUtil.GetMapLength(false);
        m_camMaxX = MgToken.GetInstance().GetMaps()[tileXNum - 1, tileYNum - 1].GetObject().transform.position.x - m_camMinX;
        m_camMaxY = MgToken.GetInstance().GetMaps()[tileXNum - 1, tileYNum - 1].GetObject().transform.position.y;
        Debug.Log("최고 너비는 " + m_camMaxX + " : " + m_camMaxY);
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
            if (m_isDragMode == false)
                CallTokenClick();
            //클릭 상태는 초기화
            m_isMouseClick = false;
            m_isDragMode = false;
        }
    }

    private ObjectTokenBase m_preClickToken = null;
    private float m_preClickTime =0f;
    private void CallTokenClick()
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
            return;

        float curTime = Time.realtimeSinceStartup;

        if(m_preClickToken == null)
        {
            //첫 클릭
            //Debug.Log("생 초클릭");
            clickToken.OnClickObject();
            m_preClickTime = curTime; //누른시간 넣고
            m_preClickToken = clickToken; //누른 토큰 넣고
            return;
        }

        if(m_preClickToken == clickToken)
        {
            //만약 같은 토큰을 눌렀다면 누른 시간 간격에 따라 더블클릭 혹은 원클릭으로 진행
            if(curTime - m_preClickTime < m_doubleClickInterval)
            {
                // Debug.Log("더블클릭");
                PlayerManager.GetInstance().DoubleClickTokenObject(clickToken.GetToken());
            }
            else
            {
                //Debug.Log("원클릭");
                clickToken.OnClickObject();
            }
            m_preClickTime = curTime; //누른시간 넣고
            m_preClickToken = clickToken; //누른 토큰 넣고
            return;
        }

        //만약 다른 경우라면
        //첫 클릭으로 진행
       // Debug.Log("다른 원클릭");
        clickToken.OnClickObject();
        m_preClickTime = curTime; //누른시간 넣고
        m_preClickToken = clickToken; //누른 토큰 넣고
        return;


    }
   
    private void DragCam()
    {
        if (m_isDragMode == false)
            return;

        
        Vector3 direct = new Vector2(Input.mousePosition.x - priorMousePosition.x , Input.mousePosition.y - priorMousePosition.y);
        direct = -direct.normalized;
        
        //인버스러프 -> 그 구간에서 해당 값이 어느 비율값인지 0~1 사이 값
        float ratio = Mathf.InverseLerp(CameraMove.m_minScopeSize, CameraMove.m_maxScopeSize, Camera.main.orthographicSize);
        //러프 -> 그 구간에서 비율(0~1)에 해당하는 값 min~max 사이 값
        dragSpeed = Mathf.Lerp(m_minDragSpeed, m_maxDragSpeed, ratio); //크기 비율에 따라 속도 변화
        dragSpeed *= m_dragRatioByTileLength;
        //Camera.main.gameObject.transform.Translate(direct * dragSpeed);

        Vector3 moved = Camera.main.gameObject.transform.position + direct * dragSpeed;

        RestrictCamPos(moved);


        priorMousePosition = Input.mousePosition; //이동한 위치로 갱신 
    }
    
    public static void RestrictCamPos(Vector3 _moved)
    {
        //정해진 범위 밖으로 벗어나지 않도록 수정
        _moved.x = Mathf.Max(m_camMinX, _moved.x);
        _moved.x = Mathf.Min(m_camMaxX, _moved.x);
        _moved.y = Mathf.Max(m_camMinY, _moved.y);
        _moved.y = Mathf.Min(m_camMaxY, _moved.y);

        Camera.main.gameObject.transform.position = _moved; //카메라 허용범위 벗어난게 아니라면 위치 이동.
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
