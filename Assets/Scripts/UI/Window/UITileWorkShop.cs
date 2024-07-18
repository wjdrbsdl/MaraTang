using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITileWorkShop : UIBase
{
    //해당 타일의 정보 표기 및 타일에서 할 수 있는 작업을 제공하는 UI
    [SerializeField]
    private TMP_Text m_statText; //토지 스텟 표기 
    [SerializeField]
    private TMP_Text m_nationText; //토지 스텟 표기 
    [SerializeField]
    private Transform m_tileActionBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private BtnTileWorkShop m_workButtonSample;
    [SerializeField]
    private BtnTileWorkShop[] m_workButtones;

    [SerializeField]
    private Transform m_placeBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private BtnPlace m_placeButtonSample;
    [SerializeField]
    private BtnPlace[] m_placeButtones;

    [SerializeField]
    private BtnOccupy m_occupyButton; //토지 점령 버튼
    private int setCount = 0;

    public void SetTileWorkShopInfo(TokenTile _tile)
    {
        Switch(true);
        TokenTile _selectedTile = _tile;
       // Debug.Log(_tile.GetTileType());
        bool inMain = IsInMainChar(_selectedTile);
        //Debug.Log("메인 캐릭 있다 " + inMain);
        SetTileWorkAction(_selectedTile);
        SetPlace(_selectedTile);
        SetResourceInfo(_selectedTile);
        SetOccupyButton(_selectedTile);
        SetTileStat(_selectedTile);
    }

    //타일 액션을 수행할 수 있는 캐릭이 안에 있어야 가능
    private bool IsInMainChar(TokenTile _tile)
    {
        List<TokenChar> chars = _tile.GetCharsInTile();
        for (int i = 0; i < chars.Count; i++)
        {
            if (chars[i].isMainChar)
                return true;
        }
        return false;
    }

    private void SetTileWorkAction(TokenTile _selectedTile)
    {
        TokenAction[] tileWorks = GamePlayMaster.GetInstance().RuleBook.RequestTileActions(_selectedTile);
        setCount = tileWorks.Length;
        //사용하는 만큼 버튼 활성화 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, setCount, m_tileActionBox);
        //버튼 세팅
        SetActionButtons(_selectedTile, tileWorks);
    }
    private void SetActionButtons(TokenTile _tile, TokenAction[] _workes)
    {

        for (int i = 0; i < setCount; i++)
        {
            m_workButtones[i].SetActive(true);
            m_workButtones[i].SetButtonInfo(_tile, _workes[i]);
        }
        for (int i = setCount; i < m_workButtones.Length; i++)
        {
            m_workButtones[i].SetActive(false);
        }
    }

    private void SetPlace(TokenTile _selectedTile)
    {
        int[] place = MgMasterData.GetInstance().GetTileData((int)_selectedTile.GetTileType()).Places;
        MakeSamplePool<BtnPlace>(ref m_placeButtones, m_placeButtonSample.gameObject, place.Length, m_placeBox);
        //버튼 세팅
        SetPlaceButtons(place);
    }

    private void SetPlaceButtons(int[] _place)
    {
        //입장 가능한 장소를 버튼으로 만들어서 정렬
        for (int i = 0; i < _place.Length; i++)
        {
            m_placeButtones[i].SetActive(true);
            m_placeButtones[i].SetButton((TileType)_place[i], this);
        }
        for (int dontUse = _place.Length; dontUse < m_placeButtones.Length; dontUse++)
        {
            m_placeButtones[dontUse].SetActive(false);
        }
    }
    

    private void SetResourceInfo(TokenTile _tile)
    {
        int mainType = _tile.GetStat(TileStat.MainResource);
        int value = _tile.GetStat(TileStat.TileEnergy);
      //  Debug.Log((TokenTile.MainResource)mainType+" 해당 용도에서 등급은" + value);
    }

    private void SetTileStat(TokenTile _tile)
    {
        //현재 땅의 스텟 정보를 보여주기 
        MainResource mainResource = (MainResource)_tile.GetStat(TileStat.MainResource);
        TileType tileType = _tile.GetTileType();
        int NationNum = _tile.GetStat(TileStat.Nation);
        string tileStat = string.Format("소속 국가 : {0}\n토지 용도 {1}\n 토지 적합도{2} 토지력 {3}", NationNum, tileType, mainResource, _tile.GetStat(TileStat.TileEnergy));
        m_statText.text = tileStat;

        m_nationText.text = "";
        if (_tile.GetTileType().Equals(TileType.Capital))
        {
            Nation nation = _tile.GetNation();
            string nationStat = string.Format("국가 번호 : {0}\n보유 자원 \n" +
                "{1}:{2} / {3}:{4} / {5}:{6} / {7}:{8}",
                NationNum,
                (Capital)0, nation.GetResourceAmount((Capital)0),
                (Capital)1, nation.GetResourceAmount((Capital)1),
                (Capital)2, nation.GetResourceAmount((Capital)2),
                (Capital)3, nation.GetResourceAmount((Capital)3));
            m_nationText.text = nationStat;
        }
    }

    private void SetOccupyButton(TokenTile _tile)
    {
        // 점령 버튼 
        //1.불가능하면 버튼 끄고 종료
        if (GamePlayMaster.GetInstance().RuleBook.AbleOccupy(_tile) == false)
        {
            m_occupyButton.SetActive(false);
            return;
        }
        //2. 가능하면 버튼 활성화
        m_occupyButton.SetActive(true);
        //3. 세팅
        m_occupyButton.SetButton(_tile, this); 
        
    }


 }
