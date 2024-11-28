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
    private TMP_Text m_placeText; //��� �̸� ǥ�� 
    [SerializeField]
    private TMP_Text m_workText; //���� ���� ǥ�� 
    [SerializeField]
    private Transform m_LaborSlotBox; //�׼� ����Ʈ ��ư ���� ���
    [SerializeField]
    private LaborSlot m_LaborSlotSample;
    [SerializeField]
    private LaborSlot[] m_LaborSlots;

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
    #endregion

    Stack<TileType> m_placeStack = new(); //������ �ε� 
    TileType m_curPlace = TileType.Nomal;
    TokenTile m_curTile = null;

    #region ���� ���� ȣ��
    public void SetTileInfo(TokenTile _tile, TileType _tileType)
    {
        UISwitch(true);
        m_curTile = _tile;
        m_curPlace = _tileType;
        m_placeText.text = _tileType.ToString();
        PlayerManager.GetInstance().SetHeroPlace(_tileType);
        MakeLaborSlots();
        //Debug.Log("���� ĳ�� �ִ� " + inMain);
        ResetUI();
    }

    public void ResetSetPlace()
    {
        SetInPlace();
    }

    public void ResetUI()
    {
        SetTileWork(); //Ÿ�Ͽ��� �������� �۾�
        SetLaborCoin();
        SetInPlace(); //Ÿ�� ���� ��ҵ�
        SetOutBuildList(); //Ÿ�Ͽ��� �Ǽ������� �ܺΰǹ���
        SetOccupyButton(); //�����ϱ��ư - �̰Ǿ��� ��
        SetTileStat(); //�ش� Ÿ�� ����
    }
    
    private void MakeLaborSlots()
    {
        MakeSamplePool<LaborSlot>(ref m_LaborSlots, m_LaborSlotSample.gameObject, 3, m_LaborSlotBox);
    }
    #endregion

    public void OnClickTileAction()
    {
        m_curTile.DoInhereceWork(m_curPlace);
    }

    #region UI ����

    private void SetOutBuildList()
    {
        //�ش� ��ҿ��� ���� �� �ִ� ���๰
        int[] buildPlace = MgMasterData.GetInstance().GetTileData((int)m_curPlace).AbleBuildPid.ToArray();
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

    private void SetTileWork()
    {
        SetPushButton(); //�ڿ����� - �۾��� �ʿ��� �κ��̹Ƿ� Ÿ�Ͽ�ũ���� ���� 
        WorkOrder work = m_curTile.GetWorkOrder();
        if (work == null)
        {
            m_workText.text = "�������� �� ����";
            return;
        }

        m_workText.text = work.workType.ToString() + "�۾� ��";
            

    }

    private void SetLaborCoin()
    {
        List<LaborCoin> labors = m_curTile.GetLaborList();
        for (int i = 0; i < labors.Count; i++)
        {
            m_LaborSlots[i].gameObject.SetActive(true);
            m_LaborSlots[i].SetSlot(labors[i]);
        }
        for (int x = labors.Count; x < 3; x++)
        {
            m_LaborSlots[x].gameObject.SetActive(false);
        }
    }

    private void SetInPlace()
    {
        //�ش� ��ҿ��� �� �� �ִ� ��� 
        //������ �ȵ������� ������� ��� 
        int[] interiorPlace = MgMasterData.GetInstance().GetTileData((int)m_curPlace).AbleInteriorPid.ToArray();
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

    #region UI OnOff
    public void EnterPlace(TokenTile _tile, TileType _tileType)
    {
        //�̹� UI�� ���� �ش� ��ҿ��� ���� ��ҷ� �̵��� 
        //1. ���� ��ġ�� ���ÿ� �߰� 
        m_placeStack.Push(m_curPlace);
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

    public override void OffWindow()
    {
        base.OffWindow();
        PlayerManager.GetInstance().SetHeroPlace(TileType.Nomal);
    }
    #endregion
}
