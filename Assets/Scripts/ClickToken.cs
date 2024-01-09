using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickToken : MonoBehaviour
{
    private void Update()
    {
        CallTokenClick();
        InputKey();
    }

    void CallTokenClick()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (EventSystem.current.IsPointerOverGameObject())
        {
           // Debug.Log("유아이 슬랏 눌러서 종료");
            return;
        }
     
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f);
        int maxPri = -1; //최소 선호도는 0 
        ObjectTokenBase clickToken = null;
        for (int i = 0; i < hit.Length; i++)
        {
            ObjectTokenBase tokenObject = hit[i].collider.GetComponent<ObjectTokenBase>();
            if (tokenObject == null)
                continue;

            int curClickPrior = tokenObject.GetClickPrior();
            if (maxPri < curClickPrior)
            {
                maxPri = curClickPrior;
                clickToken = tokenObject;
            }
        }
        //최우선 우선순위 오브젝트가 있다면
        if (clickToken != null)
            clickToken.OnClickObject();
        
    }

    void InputKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }
}
