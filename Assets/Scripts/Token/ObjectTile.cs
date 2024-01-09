using System.Collections;
using UnityEngine;
using TMPro;

public class ObjectTile : ObjectTokenBase
{
    private Vector3 offsize = new Vector3(0, 0, -15);
   
    [SerializeField]
    public TMP_Text text;

    public void ShowRouteNumber(int _number)
    {
        text.gameObject.SetActive(true);
        text.text = _number.ToString();
    }

    public void OffRouteNumber()
    {
        text.gameObject.SetActive(false);
        text.text = null;
    }
}
