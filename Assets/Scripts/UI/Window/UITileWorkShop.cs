using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITileWorkShop : UIBase
{
    //해당 타일의 정보 표기 및 타일에서 할 수 있는 작업을 제공하는 UI
    [SerializeField]
    private Transform m_tileActionBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private BtnTileWorkShop m_workButtonSample;

    [SerializeField]
    private BtnTileWorkShop[] m_workButtones;
    public void SetTileWorkShopInfo(TokenTile _tile)
    {
        Switch(true);
        
        _tile.GetInfoForTileWorkShop(); //타일 고유 정보 얻고
        //기본 타일 액션들
        string[] baseWorks = System.Enum.GetNames(typeof(TileAction));

        //둘이 조합해서 가능한 액션들 뽑는다
        string[] confirmWorks = ConfirmTileActions(baseWorks);
        
        //사용하는 만큼 버튼 활성화 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, confirmWorks.Length, m_tileActionBox);
        //버튼 세팅
        SetButtons(_tile, confirmWorks);

    }

    private string[] ConfirmTileActions(string[] _baseWorks)
    {
        //여러 정보 활용해서 타일에서 할 수 있는 액션을 최종적으로 확정.

        return _baseWorks;
    }

    private void SetButtons(TokenTile _tile, string[] _workes)
    {
        int setCount = Random.Range(1, 3);
        for (int i = 0; i < setCount; i++)
        {
            m_workButtones[i].gameObject.SetActive(true);
            m_workButtones[i].SetButtonInfo(_tile, _workes[i]);
        }
        for (int i = setCount; i < _workes.Length; i++)
        {
            m_workButtones[i].gameObject.SetActive(false);
        }
    }
}
