using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SystemLoading : MonoBehaviour
{
    public static SystemLoading g_instance;
    private float m_time = 0;
    private float m_loadTime;
    [SerializeField] GameObject m_loadImage;
    public bool m_isLoading = false;
    public Image test_image;

    void Awake()
    {
        g_instance = this; 
    }

    void Update()
    {
        if (m_isLoading)
        {
            m_time += Time.deltaTime;
            Display();
            if (m_time >= m_loadTime)
            {
                m_isLoading = false;
                m_loadImage.SetActive(false);
                SystemPause.g_instance.Play(PauseReason.LoadingCut);
            }

        }
    }

    public void PlayLoadingScene(float _loadTime = 1)
    {
        m_time = 0;
        m_loadTime = _loadTime;
        m_loadImage.SetActive(true);
        m_isLoading = true;
        SystemPause.g_instance.Pause(PauseReason.LoadingCut);
    }

    private void Display()
    {
        Color a = test_image.color;
        a.a = (m_loadTime - m_time) /m_loadTime;
        test_image.color = a;
    }
}
