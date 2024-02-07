using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject m_target;
    public Vector3 offSet = new Vector3(0, 0, -5);
    public bool m_onTrace = false;

    private void Update()
    {
        if (m_onTrace == true)
            MoveToTarget();
    }

    public void SetTarget(ObjectTokenBase _tokenObject)
    {
        m_target = _tokenObject.gameObject;
        MoveToTarget();
    
    }

    public void MoveToTarget()
    {
        if (m_target != null)
        {
            ClickToken.RestricCamPos(m_target.transform.position + offSet);
        }
    }

    public void TraceOnOff(bool _on)
    {
        m_onTrace = _on;
    }

    public void FocusTarget(ObjectTokenBase _tokenObject)
    {
        ClickToken.RestricCamPos(_tokenObject.gameObject.transform.position + offSet);
    }
}
