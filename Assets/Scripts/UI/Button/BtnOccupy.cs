using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnOccupy : MonoBehaviour
{
    private TokenTile m_tile;

    public void SetButton(TokenTile _tile)
    {
        m_tile = _tile;
    }
    public void OnClickOccupy()
    {
       GamePlayMaster.GetInstance().ClickOccupy(m_tile);
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
