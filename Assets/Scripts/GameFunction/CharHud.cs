using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharHud : MonoBehaviour
{
    private TokenBase m_token;

    [SerializeField]
    private GameObject m_target;

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
        {
            m_window.SetActive(false);
            return;
        }

        m_window.SetActive(true);
        transform.position = Camera.main.ViewportToScreenPoint(m_screenPoint)+Vector3.down*35f;
    }

    private bool CheckInCam()
    {
        //추적중인 토큰의 오브젝트가 씬에 들어와있는가
        m_screenPoint = Camera.main.WorldToViewportPoint(m_target.transform.position);
        bool isIn = m_screenPoint.x > 0 && m_screenPoint.x < 1 && m_screenPoint.y > 0 && m_screenPoint.y < 1;
        return isIn;
    }

    public void SetHp(TokenBase _targetToken)
    {
        m_token = _targetToken;
        m_target = _targetToken.GetObject().gameObject;
        float curHp = _targetToken.GetStat(CharStat.CurHp);
        float maxHp = _targetToken.GetStat(CharStat.MaxHp);
        float ratio = curHp / maxHp;

        m_hpBar.fillAmount = ratio;
    }

    public void DestroyHud()
    {
        Destroy(gameObject);
    }
    
}
