using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharHud : MonoBehaviour
{
    private TokenBase m_token;

    [SerializeField]
    private GameObject m_window;

    [SerializeField]
    private Image m_hpBar;

    private Vector2 m_screenPoint; //대상 토큰의 스크린에서 위치

    private void LateUpdate()
    {
        Positioning();
    }

    private void Positioning()
    {
        if (CheckInCam() == false)
            m_window.SetActive(false);

        m_window.SetActive(true);
        transform.position = Camera.main.ViewportToWorldPoint(m_screenPoint) + Vector3.down*2f;
    }

    private bool CheckInCam()
    {
        //추적중인 토큰의 오브젝트가 씬에 들어와있는가
        m_screenPoint = Camera.main.WorldToViewportPoint(m_token.GetObject().transform.position);
        bool isIn = m_screenPoint.x > 0 && m_screenPoint.x < 1 && m_screenPoint.y > 0 && m_screenPoint.y < 1;
        // Debug.Log(isIn + " "+screenPoint);
        return isIn;
    }

    public void SetHp(TokenBase _base)
    {
        m_token = _base;
        float ratio = _base.GetStat(CharStat.CurHp) / _base.GetStat(CharStat.MaxHp);

        m_hpBar.fillAmount = ratio;
    }
    
}
