using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMiniMap : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //정해진 영역에서 어느 포인터에 찍혔는지 
        Debug.Log(eventData.position);
    }

}
