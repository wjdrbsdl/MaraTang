using System.Collections;
using UnityEngine;


public class SkillSfx : MonoBehaviour
{
    public float m_persist;
    // Update is called once per frame
    void Update()
    {
        Timer();
    }

    public void Timer()
    {
        m_persist -= Time.deltaTime;
        if (m_persist <= 0)
            Destroy(gameObject);
    }

    public void SetTimer(float _timer)
    {
        m_persist = _timer;
    }
}
