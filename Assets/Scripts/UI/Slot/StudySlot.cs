﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudySlot : SlotBase
{
    private int m_actionPid; //할당된 스킬 pid

    public void SetAction(int _pid)
    {
        gameObject.SetActive(true);
        m_actionPid = _pid;
        //1. 액션 정보 표기 

        //2. 학습 여부 표기

        //3. 학습 가능 여부 표기
    }

    public void OnClick()
    {
        bool doneStudy = PlayerManager.GetInstance().StudyPlayerAction(m_actionPid);
        if (doneStudy)
        {
    //        Debug.Log("학습함");
        }
        else
        {
     //       Debug.Log("학습 못함");
        }
    }
}

