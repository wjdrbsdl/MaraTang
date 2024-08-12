using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudySlot : MonoBehaviour
{
    private int m_actionPid; //할당된 스킬 pid

    public void SetAction(int _pid)
    {
        m_actionPid = _pid;
    }

    public void OnClick()
    {
        Debug.Log(m_actionPid + "학습 ");
    }
}

