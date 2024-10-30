using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
        if (_tile.IsWorking(WorkType.InterBuild, (int)_tileType)) //해당 장소가 공사중인지 확인해서 공사중이면
        {
            m_placeText.text += "\n공사중";
        }
    }
    public void OnClickPlaceEnter()
    {
        if (m_tile.IsBuildInterior((int)m_tileType) == false)
        {
           
            if (m_tile.IsWorking(WorkType.InterBuild, (int)m_tileType)) //이미 공사중이면 추가 발주 안함
            {
                Debug.Log("공사중인 장소");
                return;
            }
            Debug.Log("내부 공사 발주");
            MakeInteriorWork();
            return;
        }
            

        m_tileInfoUI.EnterPlace(m_tile, m_tileType);
    }

    public void MakeInteriorWork()
    {
        //해당 타일에 해당 내부 건축물 건설 진행 
        List<TOrderItem> nothing = new();
        WorkOrder order = new WorkOrder(nothing, 100, (int)m_tileType, WorkType.InterBuild);
        //다른 타일로 작업시 m_tile이 변경될수있으므로 다른 인스턴스로 생성
        TokenTile tile = m_tile;
        Action doneEffect = delegate
        {
            tile.doneInteriorList.Add((int)m_tileType);
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
