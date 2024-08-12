using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnOccupy : MonoBehaviour
{
    private TokenTile m_tile;
    private UIBase m_motherUI;

    public void SetButton(TokenTile _tile, UIBase _motherUI)
    {
        m_tile = _tile;
        m_motherUI = _motherUI;
    }
    public void OnClickOccupy()
    {
       GamePlayMaster.GetInstance().ClickOccupy(m_tile);
        m_motherUI.UISwitch(false);
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
