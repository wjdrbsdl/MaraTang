using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnDestroy : MonoBehaviour
{
    private TokenTile m_tile;
    private UIBase m_motherUI;

    public void SetButton(TokenTile _tile, UIBase _motherUI)
    {
        m_tile = _tile;
        m_motherUI = _motherUI;
    }
    public void OnClickDestroy()
    {
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)m_tile.GetTileType());
        TokenTile destroyTile = m_tile;
        WorkOrder work = new WorkOrder(tileData.DestroyCostData.GetItemList(), tileData.NeedDestroyTurn, 0, destroyTile, 0, WorkType.DestroyPlace);
        if (work.DoneWrite)
        {
            Debug.Log("파괴 요청 진행");
        }
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
