using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaviPin : MonoBehaviour
{
    public Vector2 naviPoint; //�����ִ� �� 
    private Camera m_mainCam;
    private void Start()
    {
        m_mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        Navigate();
    }

    private void Navigate()
    {
        //ī�޶� �信 ����������
        if (IsInCamevaView())
        {
            transform.position = naviPoint;
            return;
        }

        Vector2 dirVec = screenPoint - new Vector2(0.5f, 0.5f); //�� ���Ϳ��� ������ ������ ����
        float ratio = Scalar(dirVec); //�� ���ͳ� ���� ���� ����
        float padding = 0.95f; //�� ��輱���� ���� �� �е�
        dirVec.x *= ratio * padding; //������ ���� ���Ϳ� ������ �е��� ���ؼ� �� ��Ʈ�� ���� ����
        dirVec.y *= ratio * padding;
        
        Vector2 posVec = new Vector2(0.5f, 0.5f) + dirVec; //���Ϳ��� ����� ���͸� ���ؼ� ��ġ�� ���
        //Debug.Log("���� ���� ����" + dirVec + "��ġ ����" + posVec);
        Vector2 viewVec = m_mainCam.ViewportToWorldPoint(posVec); //����Ʈ ���� ���� ��ǥ������ �Űܼ� ȭ�鿡 ǥ�� - �׳� ����Ʈ ��ǥ ��� �ص��ǰڴµ�
        transform.position = viewVec; //����� ������ǥ�� �� �̵�

    }

    private float Scalar(Vector2 _dir)
    {
        //  Debug.Log("��ũ�� ����Ʈ " + screenPoint + "������ ����" + _dir);
        float gap = Mathf.Abs(0.5f - screenPoint.x);
        if (Mathf.Abs(_dir.x) < Mathf.Abs(_dir.y))
          gap = Mathf.Abs(0.5f - screenPoint.y);

        if (gap == 0)
            gap = 0.0001f;

        float ratio = 0.5f / gap;
        return ratio;
    }

    Vector2 screenPoint;
    private bool IsInCamevaView()
    {
        screenPoint = m_mainCam.WorldToViewportPoint(naviPoint);
        bool isIn = screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
       // Debug.Log(isIn + " "+screenPoint);
        return isIn;
    }
}
