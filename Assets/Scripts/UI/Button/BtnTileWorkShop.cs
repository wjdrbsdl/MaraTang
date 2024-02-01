using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnTileWorkShop : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_tmpText;
    private string m_workCode;
    private TokenTile m_tile;

    public void SetButtonInfo(TokenTile _tile, string _actionCode)
    {
        m_tmpText.text = _actionCode;
        m_tile = _tile;
        m_workCode = _actionCode;
    }

    public void OnButtonClick()
    {
        //해당 액션을 선택하면 
        PlayerManager.GetInstance().SelectTileAction(m_tile, m_workCode);
    }
}
