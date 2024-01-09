using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolTipBase : MonoBehaviour
{
    public TipType m_tipType;
    [SerializeField] private GameObject m_back;

    public void TipSwitch(bool _on)
    {
        if (_on)
            TipOn();
        else
            TipOff();
    }

    public virtual void TipOn()
    {
        m_back.SetActive(true);
    }

    public virtual void TipOff()
    {
        m_back.SetActive(false);
    }

    public abstract void SetToolTip(TokenBase _item);
  

}
