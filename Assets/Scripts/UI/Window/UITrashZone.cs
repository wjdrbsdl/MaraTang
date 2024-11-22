using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITrashZone : MonoBehaviour
{

    public void SetTrashZone(GameObject _UI)
    {
        //�ش� UI ������Ʈ ���� �� ������ ��ġ�ϱ� 
        gameObject.SetActive(true);
        gameObject.transform.SetParent(_UI.transform);
        transform.SetAsFirstSibling();
    }
}
