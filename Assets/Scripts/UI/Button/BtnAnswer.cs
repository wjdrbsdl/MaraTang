using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Answer
{
    Yes, No, Cancle
}

public class BtnAnswer : MonoBehaviour
{
    public Answer m_answer;
    public ISelectCustomer m_selectCustomer;

    public void SetButton(ISelectCustomer _customer)
    {
        m_selectCustomer = _customer;
    }

    public void OnBnt()
    {
        if(m_selectCustomer != null)
        {
            m_selectCustomer.OnSelectCallBack((int)m_answer);
        }
    }
}
