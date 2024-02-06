using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgSetting : MgGeneric<MgSetting>
{
    public enum ButtonType
    {
        �ٽ��ϱ�, ������, ��������, ����
    }

    public float m_SoundValue;

    public void QuitApp()
    {
#if UNITY_EDITOR
        Debug.Log("���� ����");
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        Application.Quit();
    }
}
