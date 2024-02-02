using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayData : UIBase
{
    [SerializeField] private TMP_Text m_turnText;

    public void ShowPlayData()
    {
        GamePlayData _gamePlayData = GamePlayMaster.GetInstance().GetPlayData();
        m_turnText.text = _gamePlayData.PlayTime.ToString();
    }
}
