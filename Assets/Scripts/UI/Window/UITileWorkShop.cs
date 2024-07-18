using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITileWorkShop : UIBase
{
    //�ش� Ÿ���� ���� ǥ�� �� Ÿ�Ͽ��� �� �� �ִ� �۾��� �����ϴ� UI
    [SerializeField]
    private TMP_Text m_statText; //���� ���� ǥ�� 
    [SerializeField]
    private TMP_Text m_nationText; //���� ���� ǥ�� 
    [SerializeField]
    private Transform m_tileActionBox; //�׼� ����Ʈ ��ư ���� ���
    [SerializeField]
    private BtnTileWorkShop m_workButtonSample;
    [SerializeField]
    private BtnTileWorkShop[] m_workButtones;

    [SerializeField]
    private Transform m_placeBox; //�׼� ����Ʈ ��ư ���� ���
    [SerializeField]
    private BtnPlace m_placeButtonSample;
    [SerializeField]
    private BtnPlace[] m_placeButtones;

    [SerializeField]
    private BtnOccupy m_occupyButton; //���� ���� ��ư
    private int setCount = 0;

    public void SetTileWorkShopInfo(TokenTile _tile)
    {
        Switch(true);
        TokenTile _selectedTile = _tile;
       // Debug.Log(_tile.GetTileType());
        bool inMain = IsInMainChar(_selectedTile);
        //Debug.Log("���� ĳ�� �ִ� " + inMain);
        SetTileWorkAction(_selectedTile);
        SetPlace(_selectedTile);
        SetResourceInfo(_selectedTile);
        SetOccupyButton(_selectedTile);
        SetTileStat(_selectedTile);
    }

    //Ÿ�� �׼��� ������ �� �ִ� ĳ���� �ȿ� �־�� ����
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
        //����ϴ� ��ŭ ��ư Ȱ��ȭ 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, setCount, m_tileActionBox);
        //��ư ����
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
        //��ư ����
        SetPlaceButtons(place);
    }

    private void SetPlaceButtons(int[] _place)
    {
        //���� ������ ��Ҹ� ��ư���� ���� ����
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
      //  Debug.Log((TokenTile.MainResource)mainType+" �ش� �뵵���� �����" + value);
    }

    private void SetTileStat(TokenTile _tile)
    {
        //���� ���� ���� ������ �����ֱ� 
        MainResource mainResource = (MainResource)_tile.GetStat(TileStat.MainResource);
        TileType tileType = _tile.GetTileType();
        int NationNum = _tile.GetStat(TileStat.Nation);
        string tileStat = string.Format("�Ҽ� ���� : {0}\n���� �뵵 {1}\n ���� ���յ�{2} ������ {3}", NationNum, tileType, mainResource, _tile.GetStat(TileStat.TileEnergy));
        m_statText.text = tileStat;

        m_nationText.text = "";
        if (_tile.GetTileType().Equals(TileType.Capital))
        {
            Nation nation = _tile.GetNation();
            string nationStat = string.Format("���� ��ȣ : {0}\n���� �ڿ� \n" +
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
        // ���� ��ư 
        //1.�Ұ����ϸ� ��ư ���� ����
        if (GamePlayMaster.GetInstance().RuleBook.AbleOccupy(_tile) == false)
        {
            m_occupyButton.SetActive(false);
            return;
        }
        //2. �����ϸ� ��ư Ȱ��ȭ
        m_occupyButton.SetActive(true);
        //3. ����
        m_occupyButton.SetButton(_tile, this); 
        
    }


 }
