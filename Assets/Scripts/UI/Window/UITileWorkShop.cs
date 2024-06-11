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
    [SerializeField]
    private BtnOccupy m_occupyButton; //토지 점령 버튼

    public void SetTileWorkShopInfo(TokenTile _tile)
    {
        Switch(true);
        TokenTile _selectedTile = _tile;
        bool inMain = IsInMainChar(_selectedTile);
        //Debug.Log("메인 캐릭 있다 " + inMain);
        TokenAction[] tileWorks = GamePlayMaster.GetInstance().RuleBook.RequestTileActions(_selectedTile);
        setCount = tileWorks.Length;
        //사용하는 만큼 버튼 활성화 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, setCount, m_tileActionBox);
        //버튼 세팅
        SetButtons(_selectedTile, tileWorks);
        SetResourceInfo(_selectedTile);
        SetOccupyButton(_selectedTile);
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
    int setCount = 0;
    private void SetButtons(TokenTile _tile, TokenAction[] _workes)
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

    private void SetResourceInfo(TokenTile _tile)
    {
        int mainType = _tile.GetStat(TileStat.MainResource);
        int value = _tile.GetStat(TileStat.TileEnergy);
        Debug.Log((TokenTile.MainResource)mainType+" 해당 용도에서 등급은" + value);
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
