using System.Collections;
using UnityEngine;


public class WorkOrderPin : MonoBehaviour
{
    public Vector2 naviPoint; //꽂혀있는 곳 
    public SpriteRenderer m_spriteRender;
    private Camera m_mainCam;
    private void Start()
    {
        m_mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        Navigate();
    }

    public void SetPinInfo(TokenTile _tile)
    {
        naviPoint = new Vector2(_tile.GetXIndex(), _tile.GetYIndex());
    }

    public void SwitchPin(bool _on)
    {
        gameObject.SetActive(_on);
    }

    public void DestroyPin()
    {
        Destroy(gameObject);
    }

    #region 핀 조정
    private void Navigate()
    {
        //카메라 뷰에 들어와있으면
        if (IsInCamevaView())
        {
            transform.position = naviPoint;
            return;
        }

        SwitchPin(false); //화면밖이면 꺼
    }

    Vector2 screenPoint;
    private bool IsInCamevaView()
    {
        screenPoint = m_mainCam.WorldToViewportPoint(naviPoint);
        bool isIn = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        // Debug.Log(isIn + " "+screenPoint);
        return isIn;
    }
    #endregion
}
