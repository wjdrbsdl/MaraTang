using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICapital : UIBase
{
    [SerializeField] private TMP_Text m_grassText;

    public void ResetCapitalInfo(PlayerCapitalData _capitalData)
    {
        m_grassText.text = _capitalData.GetData(PlayerCapitalData.Capital.Grass).ToString();
    }
}
