using System.Collections;
using UnityEngine;
using TMPro;
using System;


public class BtnReportCheck : MonoBehaviour
{
    private Action nationCallBack;

    public void SetEvent(Action _action)
    {
        nationCallBack = _action;
    }

    public void OnClick()
    {
        gameObject.SetActive(false);
        Action preAction = nationCallBack;
        nationCallBack = null;

        if (preAction != null)
            preAction();
    
    }
}
