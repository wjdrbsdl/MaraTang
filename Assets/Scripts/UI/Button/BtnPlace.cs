using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnPlace : MonoBehaviour
{
    private TokenTile m_tile;
    private TileType m_tileType;
    private UITileInfo m_tileInfoUI;
    [SerializeField] private TMP_Text m_placeText;

    public void SetButton(TokenTile _tile, TileType _tileType, UITileInfo _motherUI)
    {
        m_tile = _tile;
        m_tileType = _tileType;
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)_tileType);
        m_placeText.text = tileData.PlaceName;
        m_tileInfoUI = _motherUI;
    }
    public void OnClickPlaceEnter()
    {
        m_tileInfoUI.EnterPlace(m_tile, m_tileType);
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
