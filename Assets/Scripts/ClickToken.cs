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
           // Debug.Log("������ ���� ������ ����");
            return;
        }
     
        RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0f);
        int maxPri = -1; //�ּ� ��ȣ���� 0 
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
        //�ֿ켱 �켱���� ������Ʈ�� �ִٸ�
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
