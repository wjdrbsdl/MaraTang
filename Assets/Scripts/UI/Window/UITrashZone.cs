using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITrashZone : MonoBehaviour
{

    public void SetTrashZone(GameObject _UI)
    {
        //해당 UI 오브젝트 하위 중 맨위로 위치하기 
        gameObject.SetActive(true);
        gameObject.transform.SetParent(_UI.transform);
        transform.SetAsFirstSibling();
    }
}
