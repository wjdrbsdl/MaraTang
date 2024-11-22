using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITileInfo : UIBase
{
    #region ����
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
    private Transform m_placeBox; //������� ��ư ���� ������
    [SerializeField]
    private BtnPlace m_placeButtonSample;
    [SerializeField]
    private BtnPlace[] m_placeButtones;

    [SerializeField]
    private Transform m_buildBox; //�ܺΰǼ� ��ư ���� ������
    [SerializeField]
    private BtnBuild m_buildButtonSample;
    [SerializeField]
    private BtnBuild[] m_buildButtones;

    [SerializeField]
    private BtnInputResource m_putButton; //���� ���� ��ư

    [SerializeField]
    private BtnOccupy m_occupyButton; //���� ���� ��ư
    private int setCount = 0;
    #endregion

    Stack<TileType> m_placeStack = new(); //������ �ε� 
    TileType m_curType = TileType.Nomal;
    TokenTile m_curTile = null;
    public void SetTileInfo(TokenTile _tile, TileType _tileType)
    {
        UISwitch(true);
        m_curTile = _tile;
        TileDistance distance = new TileDistance();

        //Ÿ�� �ͼ��� ���� ���� ���� �����
        //Debug.Log("�Ҵ�� �ڽ� "+_tile.GetChildIndex().Count);
        //if(_tile.parent != null)
        //    Debug.Log(" �θ� " + _tile.parent[0] + "" + _tile.parent[1]);

        // Debug.Log(_tile.GetLaborCoinCount() + " �뵿 ���� �� ");
  
        m_curType = _tileType;
        PlayerManager.GetInstance().SetHeroPlace(_tileType);
        // Debug.Log(_tile.GetTileType());

        //Debug.Log("���� ĳ�� �ִ� " + inMain);
        ResetUI();
    }

    public void ResetSetPlace()
    {
        SetInPlace();
    }

    public void ResetUI()
    {
        if(m_curTile.IsOutBuilding() == true)
        {
            //�ܺΰ������̸� ���系���� �����ְ� ��
            SetOutBuildInfo();
            SetPushButton();
            return;
        }
        SetTileAction();
        SetInPlace();
        SetOutBuild();
        SetResourceInfo();
        SetOccupyButton();
        SetPushButton();
        SetTileStat();
    }

    #region UI ����

    private void SetOutBuildInfo()
    {
        m_nationText.text = ((TileType)m_curTile.m_workOrder.WorkPid).ToString() + "�۾� ��";
        m_curTile.m_workOrder.NoticeNeedResource();
    }

    private void SetOutBuild()
    {
        //�ش� ��ҿ��� ���� �� �ִ� ���๰
        int[] buildPlace = MgMasterData.GetInstance().GetTileData((int)m_curType).AbleBuildPid.ToArray();
        MakeSamplePool<BtnBuild>(ref m_buildButtones, m_buildButtonSample.gameObject, buildPlace.Length, m_buildBox);
        //��ư ����
        SetBuildButtons(m_curTile, buildPlace);
    }

    private void SetBuildButtons(TokenTile _selectedTile, int[] _place)
    {
        //���� ������ ��Ҹ� ��ư���� ���� ����
        for (int i = 0; i < _place.Length; i++)
        {
            m_buildButtones[i].SetActive(true);
            m_buildButtones[i].SetButton(_selectedTile, (TileType)_place[i], this);
        }
        for (int dontUse = _place.Length; dontUse < m_buildButtones.Length; dontUse++)
        {
            m_buildButtones[dontUse].SetActive(false);
        }
    }


    private void SetTileAction()
    {
        TokenTileAction[] tileWorks = GamePlayMaster.GetInstance().RuleBook.RequestTileActions(m_curType);
        setCount = tileWorks.Length;
        //����ϴ� ��ŭ ��ư Ȱ��ȭ 
        MakeSamplePool<BtnTileWorkShop>(ref m_workButtones, m_workButtonSample.gameObject, setCount, m_tileActionBox);
        //��ư ����
        SetActionButtons(m_curTile, tileWorks);
    }

    private void SetActionButtons(TokenTile _tile, TokenTileAction[] _workes)
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

    private void SetInPlace()
    {
        //�ش� ��ҿ��� �� �� �ִ� ��� 
        //������ �ȵ������� ������� ��� 
        int[] interiorPlace = MgMasterData.GetInstance().GetTileData((int)m_curType).AbleInteriorPid.ToArray();
        MakeSamplePool<BtnPlace>(ref m_placeButtones, m_placeButtonSample.gameObject, interiorPlace.Length, m_placeBox);
        //��ư ����
        SetPlaceButtons(m_curTile, interiorPlace);
    }

    private void SetPlaceButtons(TokenTile _selectedTile, int[] _place)
    {
        //���� ������ ��Ҹ� ��ư���� ���� ����
        for (int i = 0; i < _place.Length; i++)
        {
            m_placeButtones[i].SetActive(true);
            m_placeButtones[i].SetButton(_selectedTile, (TileType)_place[i], this);
        }
        for (int dontUse = _place.Length; dontUse < m_placeButtones.Length; dontUse++)
        {
            m_placeButtones[dontUse].SetActive(false);
        }
    }
    
    private void SetResourceInfo()
    {
        int mainType = (int)m_curTile.GetMainResource();
        int value = m_curTile.GetResrouceGrade();
      //  Debug.Log((TokenTile.MainResource)mainType+" �ش� �뵵���� �����" + value);
    }

    private void SetTileStat()
    {
        TokenTile _tile = m_curTile;
        //���� ���� ���� ������ �����ֱ� 
        MainResource mainResource = m_curTile.GetMainResource();
        TileType tileType = _tile.GetTileType();
        int NationNum = _tile.GetStat(TileStat.Nation);
        string tileStat = string.Format("�Ҽ� ���� : {0}\n���� �뵵 {1}\n ���� ���յ�{2} ������ {3}\n��ǥ {4},{5}\n�뵿 ����{6}",
            NationNum, tileType, mainResource, _tile.GetStat(TileStat.TileEnergy), _tile.GetMapIndex()[0], _tile.GetMapIndex()[1], _tile.GetLaborCoinCount());
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

    private void SetOccupyButton()
    {
        TokenTile _tile = m_curTile;
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

    private void SetPushButton()
    {
        TokenTile _tile = m_curTile;
        m_putButton.SetButton(_tile, this);
    }
    #endregion

    public void EnterPlace(TokenTile _tile, TileType _tileType)
    {
        //�̹� UI�� ���� �ش� ��ҿ��� ���� ��ҷ� �̵��� 
        //1. ���� ��ġ�� ���ÿ� �߰� 
        m_placeStack.Push(m_curType);
        //2. �� ��ҷ� �ٽ� Ÿ�� ���� ���� 
        SetTileInfo(_tile, _tileType);
    }

    public override void ReqeustOff()
    {
        OutPlace();
    }

    public void OutPlace()
    {
        if(m_placeStack.Count == 0)
        {
            //���ư� ��Ұ� ������ uioff
            UISwitch(false);
            return;
        }
        TileType priorPlace = m_placeStack.Pop();
        SetTileInfo(m_curTile, priorPlace);
    }

    public void ForceOut()
    {
        //���� �ƿ���
        m_placeStack.Clear();
        UISwitch(false);
    }

    public override void OffWindow()
    {
        base.OffWindow();
        PlayerManager.GetInstance().SetHeroPlace(TileType.Nomal);
    }
}
