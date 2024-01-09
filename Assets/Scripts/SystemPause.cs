using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PauseReason
{
    Intro, LoadingCut, LandExpander, Request, None
}

public class SystemPause : MonoBehaviour
{
    public static SystemPause g_instance;
    private bool[] m_pauseState;
    // Start is called before the first frame update
    void Awake()
    {
        g_instance = this;
        m_pauseState = new bool[System.Enum.GetValues(typeof(PauseReason)).Length];
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Pause(PauseReason.Request);
        else if (Input.GetKeyDown(KeyCode.R))
            Play(PauseReason.Request);

    }

    public void Pause(PauseReason _reason)
    {
        FixedValue.WORLD_SPEED = 0;
        

        m_pauseState[(int)_reason] = true;
    }

    public void Play(PauseReason _reason)
    {
        m_pauseState[(int)_reason] = false;
        for (int i = 0; i < m_pauseState.Length; i++)
        {
            //���� ������ ��ø�Ǵ� ���� ���¸� ����Ͽ�, ��� ���� ��Ұ� Ǯ�� ��� �÷���. 
            if(m_pauseState[i] == true)
            {
                return;
            }
        }

        FixedValue.WORLD_SPEED = 1;
    }
}
