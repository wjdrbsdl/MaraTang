using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class BtnBuild : MonoBehaviour
{
    //외부 공사 버튼 
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
    public void OnClickBuild()
    {
        MakeOutBuildWork();
     

        m_tileInfoUI.EnterPlace(m_tile, m_tileType);
    }

    public void MakeOutBuildWork()
    {
        //해당 타일 변경 건축물 건설 진행 
        List<TOrderItem> nothing = new();
        TokenTile tile = m_tile;
        TileType buildType = m_tileType;

        WorkOrder order = new WorkOrder(nothing, 100, (int)buildType, WorkType.InterBuild);
        //다른 타일로 작업시 m_tile이 변경될수있으므로 다른 인스턴스로 생성
        Action doneEffect = delegate
        {
            tile.ChangePlace(buildType);
        };
        order.SetDoneEffect(doneEffect);
        m_tile.RegisterWork(order);
        m_tileInfoUI.ResetSetPlace();
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
