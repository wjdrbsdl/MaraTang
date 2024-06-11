using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITileWorkShop : UIBase
{
    //�ش� Ÿ���� ���� ǥ�� �� Ÿ�Ͽ��� �� �� �ִ� �۾��� �����ϴ� UI
    [SerializeField]
    private Transform m_tileActionBox; //�׼� ����Ʈ ��ư ���� ���
    [SerializeField]
    private BtnTileWorkShop m_workButtonSample;
    [SerializeField]
    private BtnTileWorkShop[] m_workButtones;
    [SerializeField]
    private BtnOccupy m_occupyButton; //���� ���� ��ư

    public void SetTileWorkShopInfo(TokenTile _tile)
    {
        Switch(true);
        TokenTile _selectedTile = _tile;
        bool inMain = IsInMainChar(_selectedTile);
        //Debug.Log("���� ĳ�� �ִ� " + inMain);
        TokenAction[] tileWorks = GamePlayMaster.GetInstance().RuleBook.RequestTileActions(_selectedTile);
        setCount = tileWorks.Length;
        //����ϴ� ��ŭ ��ư Ȱ��ȭ 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, setCount, m_tileActionBox);
        //��ư ����
        SetButtons(_selectedTile, tileWorks);
        SetResourceInfo(_selectedTile);
        SetOccupyButton(_selectedTile);
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
        Debug.Log((TokenTile.MainResource)mainType+" �ش� �뵵���� �����" + value);
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
