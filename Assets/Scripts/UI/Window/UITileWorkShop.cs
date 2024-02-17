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

    private Dictionary<int, int[]> m_ableWorkData = new();

    public void SetTileWorkShopInfo()
    {
        Switch(true);
        TokenTile _selectedTile = PlayerManager.GetInstance().GetSelectedTile();
        bool inMain = IsInMainChar(_selectedTile);
        //Debug.Log("���� ĳ�� �ִ� " + inMain);
        _selectedTile.GetInfoForTileWorkShop(); //Ÿ�� ���� ���� ���
        TokenAction[] tileWorks = GamePlayMaster.GetInstance().RuleBook.RequestTileActions(_selectedTile);
        setCount = Random.Range(1, tileWorks.Length + 1);
        //����ϴ� ��ŭ ��ư Ȱ��ȭ 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, setCount, m_tileActionBox);
        //��ư ����
        SetButtons(_selectedTile, tileWorks);

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
            m_workButtones[i].gameObject.SetActive(true);
            m_workButtones[i].SetButtonInfo(_tile, _workes[i]);
        }
        for (int i = setCount; i < m_workButtones.Length; i++)
        {
            m_workButtones[i].gameObject.SetActive(false);
        }
    }

 }
