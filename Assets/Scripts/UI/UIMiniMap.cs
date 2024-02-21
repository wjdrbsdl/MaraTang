using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMiniMap : UIBase, IPointerClickHandler
{
    public Transform left;
    public Transform right;
    public Transform up;
    public Transform down;

    Vector2 minVector;

    float widht;
    float height;

    private void Start()
    {
        SetSizeTwo();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //정해진 영역에서 어느 포인터에 찍혔는지 
        Vector2 gap = eventData.position - minVector; //찍은 부분에서 미니맵의 왼쪽아래 대각점 까지의 거리 
       // Debug.Log(gap);
        MgInput.RatioValue(gap.x/widht, gap.y/height );
    }

    private void SetSizeTwo()
    {
        widht = right.transform.position.x - left.transform.position.x;
        height= up.transform.position.y - down.transform.position.y;
        minVector = new Vector2(left.transform.position.x, down.transform.position.y);
    }

}
