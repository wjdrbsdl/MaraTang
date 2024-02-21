using System.Collections;
using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    private Vector3 m_goalVec;
    private Vector3 m_dir;

    public float m_speed = 5f; 
    
    private void Update()
    {
        MoveToGoal();
    }

    public void SetMagnetInfo(Vector3 dropPosition, TokenBase _goal = null)
    {
        if (_goal == null)
            _goal = PlayerManager.GetInstance().GetMainChar();

        transform.position = dropPosition;
        m_goalVec = _goal.GetObject().gameObject.transform.position;
        m_dir = m_goalVec - transform.position;
    }

    private void MoveToGoal()
    {
        if(Vector2.Distance(transform.position, m_goalVec) > GamePlayMaster.c_movePrecision)
        {
            transform.position += (m_dir.normalized * m_speed * Time.deltaTime);
            return;     
        }
        Destroy(gameObject);
    }
}
