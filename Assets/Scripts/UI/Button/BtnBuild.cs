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
    private int[] needTiles;
    [SerializeField] private TMP_Text m_placeText;

    public void SetButton(TokenTile _tile, TileType _tileType, UITileInfo _motherUI)
    {
        m_tile = _tile;
        m_tileType = _tileType;
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)_tileType);
        needTiles = tileData.NeedTiles;
        m_placeText.text = tileData.PlaceName;
        m_tileInfoUI = _motherUI;
     
    }
    public void OnClickBuild()
    {
        MakeOutBuildWork();
    }

    public void MakeOutBuildWork()
    {
       // Debug.Log("외부건설 작업서 만듬");
        //해당 타일 변경 건축물 건설 진행 
        List<TOrderItem> nothing = new();
        TokenTile tile = m_tile;
        TileType buildType = m_tileType;

        if (needTiles.Length >= 2)
        {
            //필요한 타입이 2개 이상인거면 타입 조합기 UI 활성화 필요 
            Debug.Log("필요 재료 2개 이상");
            MgUI.GetInstance().ShowTileMix(m_tileType, tile.GetNationNum(), tile); //해당 타일 포함해서 전달 
            return;
        }

        WorkOrder order = new WorkOrder(nothing, 100, (int)buildType, WorkType.ChangeBuild);
        //다른 타일로 작업시 m_tile이 변경될수있으므로 다른 인스턴스로 생성
        Action doneEffect = delegate
        {
            tile.CompleteOutBuild(buildType);
         };
        order.SetDoneEffect(doneEffect);
        m_tile.RegisterWork(order);
        m_tileInfoUI.ReqeustOff();
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
