using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlarmPopup : MgGeneric<AlarmPopup>
{
    public TMP_Text m_text;
    public GameObject m_popupWindow;
    private float m_flowTime = 0;
    public float m_persistTime = 1f;
    private bool m_isOn = false;

    private void Start()
    {
        ManageInitiSet();
    }

    private void Update()
    {
        Timer();    
    }

    public void PopUpMessage(string _message)
    {
        //1. 창띄우고
        m_popupWindow.SetActive(true);
        //2. 메세지 띄우고
        m_text.text = _message;
        //3. 
        m_isOn = true; //켜졌음 알리고
        //4. 
        m_flowTime = 0; //시간 초기화
    }

    private void Timer()
    {
        if (m_isOn == false)
            return;

        m_flowTime += Time.deltaTime;
        if(m_flowTime>= m_persistTime)
        {
            CloseMessage();
        }
    }

    public void CloseMessage()
    {
        m_isOn = false;
        m_flowTime = 0;
        m_popupWindow.SetActive(false);
    }
}
