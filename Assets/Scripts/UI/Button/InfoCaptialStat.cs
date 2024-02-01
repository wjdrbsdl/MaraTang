using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoCaptialStat : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpText;

    public void SetCaptialInfo(Capital _capital, int _value)
    {
        m_tmpText.text = _capital + ": " + _value.ToString();
        
    }
}
