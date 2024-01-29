using System.Collections;
using UnityEngine;
using TMPro;
using AllIn1SpriteShader;
public class ObjectTile : ObjectTokenBase
{
    private Vector3 offsize = new Vector3(0, 0, -15);
    [SerializeField]
    AllIn1Shader m_allshader;
    

    private void Awake()
    {
        m_allshader = GetComponent<AllIn1Shader>();
    }

    public void ShowRouteNumber(int _number)
    {
        //오브젝트 풀링으로 대채피ㅏㄹ요 각 타일마다 갖고잇는건 비효울
        Debug.Log("길 표시하기 ");
    }

    public void OffRouteNumber()
    {
        Debug.Log("길 표시제거 ");
    }
}
