using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMiniMap : UIBase, IPointerClickHandler
{
    #region 변수
    //기준점
    public Transform left;
    public Transform right;
    public Transform up;
    public Transform down;

    Vector2 minVector;

    float widht;
    float height;
    #endregion

    private void Start()
    {
        CalMinimapSize();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //1. 정해진 영역에서 어느 포인터에 찍혔는지 
        Vector2 gap = eventData.position - minVector; //찍은 부분에서 미니맵의 왼쪽아래 대각점 까지의 거리 
        //2. 게임 전체 맵으로 환산한 위치 값 산출                                              
        Vector3 realPos = CamRestrict.CalGamePos(gap.x/widht, gap.y/height);
        //3. 제한 값 적용해서 캠 위치 조정
        CamRestrict.RestricCamPos(realPos);
    }

    private void CalMinimapSize()
    {
        //미니맵 너비 산출 
        widht = right.transform.position.x - left.transform.position.x;
        height= up.transform.position.y - down.transform.position.y;
        minVector = new Vector2(left.transform.position.x, down.transform.position.y);
    }

}
