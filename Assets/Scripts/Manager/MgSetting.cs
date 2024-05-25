using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgSetting : MgGeneric<MgSetting>
{
    public enum SettingButtonType
    {
        다시하기, 앱종료, 사운드조정, 언어선택
    }

    public float m_SoundValue;

    public void QuitApp()
    {
#if UNITY_EDITOR
        Debug.Log("게임 종료");
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        Application.Quit();
    }
}
