using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMiniMap : MonoBehaviour, IPointerClickHandler
{
    float m_minX, m_maxX;
    float m_minY, m_maxY;
    Vector2 minVector;
    public RectTransform m_mapRect;
    float widht;
    float height;

    private void Start()
    {
        SetSizeInfo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //정해진 영역에서 어느 포인터에 찍혔는지 
        Vector2 gap = eventData.position - minVector; //찍은 부분에서 미니맵의 왼쪽아래 대각점 까지의 거리 
        Debug.Log(gap);
        MgInput.RatioValue(gap.x/widht, gap.y/height );
    }

    private void SetSizeInfo()
    {
        //우하 앵커라 우하 쪽이 위치점
        widht = m_mapRect.rect.width;
        height = m_mapRect.rect.height;

        m_maxX = transform.position.x;
        m_minX = m_maxX - m_mapRect.rect.width;

        m_minY = transform.position.y;
        m_maxY = m_minY + m_mapRect.rect.height;

        minVector = new Vector2(m_minX, m_minY);
    }

}
