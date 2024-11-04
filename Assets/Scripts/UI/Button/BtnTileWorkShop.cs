using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnTileWorkShop : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpText;
    private TokenTileAction m_workCode;
    private TokenTile m_tile;
  
    public void SetButtonInfo(TokenTile _tile, TokenTileAction _tileAction)
    {
        m_tmpText.text = _tileAction.GetItemName();
        m_tile = _tile;
        m_workCode = _tileAction;
    }

    public void OnButtonClick()
    {
        //해당 액션을 선택하면 
        PlayerManager.GetInstance().SelectTileAction(m_tile, m_workCode);
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
