using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System;

#region Ÿ�� enum
public enum TileType
{
    //�� Ÿ���� Ÿ�� �⺻ ��, �븻, �� �ű⿡ �뵵�������� ����,����, ���� �ܰ���� ��� ���� �س���? �ƴϸ� ������ ����"?
    Nomal, WoodLand, Farm, Town, Mine, Capital, Mountain, Cage=16, Child
}

public enum TileViewState
{
    Fog, Sight, NoSight
}

public enum TileStat
{
   Nation, Height, TileEnergy, BaseSight, PlaceSight, MaxDurability, CurDurability
}

public enum MainResource
{
    Tree, Food, Mineral
}
#endregion

public class TokenTile : TokenBase
{
    private List<TokenChar> m_inTileCharList = new();
    private List<LaborCoin> m_laborCoinList = new(); //�Ҵ�� �뵿 ���ε� 
    [SerializeField]
    public TileType tileType;
    public TileEffectEnum m_effectType = TileEffectEnum.None;
    public List<int> doneInteriorList; //������ ���
    public int ChunkNum;
    public WorkOrder m_workOrder = null; //�������� ����
    public Complain m_complain = null; //�������� �Ҹ�
    [JsonProperty] private TileViewState m_viewState = TileViewState.Fog;
    [JsonProperty] MainResource m_mainResource = MainResource.Tree;
    [JsonProperty] private int m_resourceGrade = 0;
    public int[] parent; //��� ����� �θ� Ÿ��
    [JsonProperty] private List<int[]> childList;// ��� ����� �ڽ� Ÿ�ϵ�
    
    /*Ÿ�� ��ȣ ����
     * 1. Ÿ�Ͽ� 1 ĳ�� ���� - Ÿĳ�� ������ ����Ұ�
     * 2. �̺�Ʈ�� �ش� Ÿ�� �������θ� �ߵ�
     * 3. Ÿ�� ����� ������ ����(Ÿ������ Ȥ�� �ش� ���� ����)�ϳ� �̺�Ʈ ����� ��� �Ұ�
     */

    #region Ÿ�ϻ���
    public TokenTile()
    {

    }

    public TokenTile MakeTileToken()
    {
        TokenTile tileToken = new TokenTile();
        tileToken.m_tokenIValues = new int[GameUtil.EnumLength(TileStat.Height)];
        tileToken.m_tokenType = TokenType.Tile;
        tileToken.doneInteriorList = new();
        tileToken.childList = new();
        tileToken.SetTileEffect(); //�ʱ� Nomal�� �´� effect�� 
        return tileToken;
    }

    public void SetHeightValue(float _ecoHeight)
    {
        //Ÿ�� ���̿� ���� ��, �븻, ������ ���� 
        int tileHeight = (int)(_ecoHeight * 100f);
        SetStatValue(TileStat.Height, tileHeight);
        if (tileHeight >= 66)
        {
            //�����ܰ迡�� nomal�� �ƴѰ�� ��ҿ� ȿ���� �ٽ� ���� 
            SetTileType(TileType.Mountain);
            SetTileEffect();
        }
    }

    public void SetDensityValue((MainResource, int) _mildo)
    {
        m_mainResource = _mildo.Item1;
        m_resourceGrade = _mildo.Item2;
    }
   
    public void SetTileSprite()
    {
        if(tileType != TileType.Nomal)
        {
            GetObject().SetSprite(TempSpriteBox.GetInstance().GetTileSprite(tileType));
            return;
        }

        GetObject().SetSprite(TempSpriteBox.GetInstance().GetBaseTile(m_mainResource));
        List<int> ran = GameUtil.GetRandomNum(4, 4 - m_resourceGrade);
        for (int i = 0; i < ran.Count; i++)
        {
            ObjectTile tileObj = (ObjectTile)GetObject();
            tileObj.SetElement(i, TempSpriteBox.GetInstance().GetTileElement(ran[i]));
        }
    }

    public void SetTileValue()
    {
        //��Ҹ��� ���� Ư���Ǿ��մ� ���������� ���Ҵ��ϴ� �κ�
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        int[] masterValue = tileData.TileStat;
        m_tokenIValues[(int)TileStat.PlaceSight] = masterValue[(int)TileStat.PlaceSight];
        m_tokenIValues[(int)TileStat.MaxDurability] = masterValue[(int)TileStat.MaxDurability];
        m_tokenIValues[(int)TileStat.CurDurability] = m_tokenIValues[(int)TileStat.MaxDurability];
        // Debug.Log(tileType + "�þ� �Ÿ� " + m_tokenIValues[(int)TileStat.PlaceSight]);
    }

    private void SetTileEffect()
    {
        m_effectType = MgMasterData.GetInstance().GetTileData((int)tileType).effectType;
    }
    #endregion

    public List<TokenChar> GetCharsInTile()
    {
        return m_inTileCharList;
    }

    #region ĳ�� �̵� ����
    public void Migrate(TokenChar _char)
    {
        //�̹� �������� �Ҽӵ� ��ū�� ������ ��ǥ���� ����
        m_inTileCharList.Add(_char);
        _char.SetMapIndex(m_xIndex, m_yIndex);
    }

    public void RemoveCharToken(TokenChar _char)
    {
        //���� �������� ����Ʈ���� ����
        m_inTileCharList.Remove(_char);
    }

    public void ShowRouteNumber(int _number)
    {
        ((ObjectTile)m_object).ShowRouteNumber(_number);
    }

    public void OffRouteNumber()
    {
        ((ObjectTile)m_object).OffRouteNumber();
    }
    #endregion

    #region Ÿ�� �̹���
    public void ChangeViewState(TileViewState _toState)
    {
        if (_toState.Equals(m_viewState))
            return;

        if(_toState.Equals(TileViewState.Sight))
        GameUtil.GetHideTileFromMap(GetMapIndex()).FogOff();
    }


    public void Dye(Color _color)
    {
        GetObject().GetComponent<SpriteRenderer>().color = _color;
    }
    #endregion

    #region ��� ����
    public void ChangePlace(TileType _tileType)
    {
        SetTileType(_tileType);
        SetTileEffect();
        SetTileSprite();
        SetTileValue();
        DoAutoTileAction();
    }
    #endregion 

    #region �۾���
    public bool RegisterWork(WorkOrder _work)
    {
        if(m_workOrder != null)
        {
            Debug.Log("����� �ϳ���");
            return false;
        }

        m_workOrder = _work;
       // Debug.Log(_work.m_workType + "�۾� Ÿ�Ͽ� ���");
        MgWorkOrderPin.GetInstance().RequestWorkOrderPin(this);
        return true;
    }

    public void SendWorkStep(WorkStateCode _code)
    {
        switch (_code)
        {
            case WorkStateCode.Complete:
                WorkOrder preWork = m_workOrder; //�Ϸ���۾� ����
                RemoveWork(); //���� �۾��� ���ְ�
                RepeatTileAction(preWork);
                break;
            case WorkStateCode.Cancle:
                RemoveWork();
                break;
        }
    }

    public void RemoveWork()
    {
        //1. �۾������� ���� Ű���̹Ƿ� ���� ������ ��û 
        MgWorkOrderPin.GetInstance().RemovePin(this);
        m_workOrder = null;
        return;
    }

    public bool IsWorking(WorkType _workType, int _pid)
    {
        if (m_workOrder == null)
            return false;

        if (m_workOrder.workType != _workType || m_workOrder.WorkPid != _pid)
            return false;

        return true;
    }

    public bool IsOutBuilding()
    {
        if (m_workOrder == null)
            return false;

        if (m_workOrder.workType == WorkType.InterBuild)
            return false;

        return true;
    }

    public bool IsBuilding()
    {
        //�ٸ� �Ǽ����ΰ�
        if (m_workOrder != null)
            return true;

        return false;
    }

    public bool IsBuildInterior(int _pid)
    {
        if (doneInteriorList.IndexOf(_pid) == -1)
            return false;

        return true;
    }

    public void CompleteInterBuild(int _pid)
    {
        doneInteriorList.Add(_pid);
    }


    #endregion

    #region �뵿����
    public int GetLaborCoinCount()
    {
        return m_laborCoinList.Count;
    }

    public void PutInLaborCoin(LaborCoin _coin)
    {
        //Debug.Log(GetNationNum() + "�� ���� �ش� Ÿ�Ͽ� " + _coin.ListIndex + " ���� ����");
        if(m_laborCoinList.IndexOf(_coin)== -1)
        {
            m_laborCoinList.Add(_coin);
        }
    }

    public void TakeOutLaborCoin(LaborCoin _coin)
    {
        if (m_laborCoinList.IndexOf(_coin) != -1)
        {
            m_laborCoinList.Remove(_coin);
        }
    }
    #endregion

    #region �Ҹ�,���,��û
    public bool SendComplain(Complain _complain)
    {
        if (m_complain != null)
        {
            Debug.Log("��ǻ��� �ϳ���");
            return false;
        }

        m_complain = _complain;
        MgWorkOrderPin.GetInstance().RequestComplainPin(this);
        return true;
    }

    public void RemoveComplain()
    {
        //1. �۾������� ���� Ű���̹Ƿ� ���� ������ ��û 
        MgWorkOrderPin.GetInstance().RemoveComplainPin(this);
        m_complain = null;
        return;
    }

    public bool HaveComplain()
    {
        return m_complain != null;
    }

    public Complain GetComplain()
    {
        return m_complain;
    }
    #endregion

    #region GetSet
    public TileType GetTileType()
    {
        return tileType;
    }
  
    public void SetTileType(TileType _tileType)
    {
        tileType = _tileType;
    }

    public TileEffectEnum GetEffect()
    {
        return m_effectType;
    }

    public void SetNation(int _nationNumber)
    {
        SetStatValue(TileStat.Nation, _nationNumber);
    }

    public int GetNationNum()
    {
        return m_tokenIValues[(int)TileStat.Nation];
    }

    public Nation GetNation()
    {
        int nationNum = m_tokenIValues[(int)TileStat.Nation];
        if (nationNum.Equals(FixedValue.NO_NATION_NUMBER))
            return null;

        return MgNation.GetInstance().GetNation(m_tokenIValues[(int)TileStat.Nation]);
    }

    public GodClassEnum GetGodClass()
    {
        //�ش� Ÿ�Ͽ� ��ġ���ִ� �� ��������
        Nation nation = GetNation();

        if (nation == null)
            return GodClassEnum.��;

        return nation.GetGodClass();

    }

  

    public WorkOrder GetWorkOrder()
    {
        return m_workOrder;
    }

    public void SetParentIndex(TokenTile _parentTile)
    {
        parent = _parentTile.GetMapIndex();
        _parentTile.AddChild(this);
        for (int i = 0; i < childList.Count; i++)
        {
            //���� �ڽ��� �ڽ��� �ִٸ� ���ڽĵ��� �θ� �ٲ�
            GameUtil.GetTileTokenFromMap(childList[i]).SetParentIndex(_parentTile);
        }
        //�ڽĵ��� �θ� �ٲٰ��� �ڽ��� �ڽ��� �ʱ�ȭ ���Ѽ� �ߺ����� �ʵ��� 
        childList = new(); 
    }

    public void AddChild(TokenTile _tile)
    {
        childList.Add(_tile.GetMapIndex());
    }

    public List<int[]> GetChildIndex()
    {
        return childList;
    }

    public MainResource GetMainResource()
    {
        return m_mainResource;
    }

    public int GetResrouceGrade()
    {
        return m_resourceGrade;
    }
    #endregion

    #region Ÿ�� ������
    public void AttackTile(int _damage)
    {
        if (tileType == TileType.Nomal)
        {
            Debug.Log("�븻Ÿ�Կ� ���� �ǹ� ����");
            return;
        }
            

        CalStat(TileStat.CurDurability, -_damage);
    }

    public override void CalStat(Enum _enumIndex, int _value)
    {
        base.CalStat(_enumIndex, _value);
        if (_enumIndex.Equals(TileStat.CurDurability)){
            CheckDurability();
        }
    }

    public void CheckDurability()
    {
        int durability = GetStat(TileStat.CurDurability);
        if(durability <= 0)
        {
            SetStatValue(TileStat.CurDurability, 0);
            DestroyPlace();
        }
    }

    public void DestroyPlace()
    {
        ChangePlace(TileType.Nomal);
        for (int i = 0; i < childList.Count; i++)
        {
            TokenTile childTile = GameUtil.GetTileTokenFromMap(childList[i]);
            //��Ͱ� �Ǵµ�, ������ �ڽĵ��� �ڽ� Ÿ���� ��� �ѹ��� �߻�. �׷��� �ı������� ���������Ƿ� �ش��Լ��� ����
            childTile.DestroyPlace(); 
        }
    }
    #endregion

    #region Ÿ�� ��� ����
    public void DoneWorkReady()
    {
        //���� �۾��� ���� �غ� ��������
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        bool isAuto = tileData.IsAuto;
        List<TOrderItem> effectList = tileData.EffectData.GetItemList();
        if (isAuto)
        {
            for (int i = 0; i < effectList.Count; i++)
            {
                GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, effectList[i]);
            }
            
        }
    }

    private void DoAutoTileAction()
    {
        Debug.Log("�ڵ� ���� üũ �ʿ� ");
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        TileEffectEnum effectType = tileData.effectType;

        if (effectType == TileEffectEnum.Tool)
        {
            Debug.Log(tileType + "���� �۾��� �ڵ� ���");
            new WorkOrder(null, tileData.NeedLaborTurn, tileData.NeedLaborAmount, this, 0, WorkType.Inherence);
        }
        //for (int i = 0; i < ablePid.Length; i++)
        //{
        //    TokenTileAction action = MgMasterData.GetInstance().GetTileAction(ablePid[i]);
        //    if (action == null)
        //        continue;
        //    if (action.IsAutoStart)
        //    {
        //        Debug.Log("�ڵ� ������" + tileType);
        //        GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, action);
        //    }
        //}
    }

    private void RepeatTileAction(WorkOrder _workOrder)
    {
        Debug.Log("Torder ���·� �ݺ� üũ �ʿ�");
        //�ѹ��� �����ϴ� �۾����� üũ
        //int[] ablePid = MgMasterData.GetInstance().GetTileData((int)tileType).AbleTileActionPID;
        //for (int i = 0; i < ablePid.Length; i++)
        //{
        //    TokenTileAction action = MgMasterData.GetInstance().GetTileAction(ablePid[i]);
        //    if (action == null)
        //        continue;

        //    //Ÿ�Ͼ׼� Ÿ���� ��ũ������
        //    if(action.GetStat(TileActionStat.TileActionType) == (int)TileActionType.WorkOrder)
        //    {
        //        //�� �۾�Ÿ���� �����ϴٸ� ���� ����� �Ϸ�� �����̹Ƿ� �ϴ� �ݺ�
        //        if(action.GetStat(TileActionStat.SubValue) == (int)_workOrder.workType)
        //        {
        //            Debug.Log("�ݺ� �۾� ���� " + _workOrder.workType);
        //            new WorkOrder(_workOrder.GetNeedList(), _workOrder.GetWorkGauge(), this, _workOrder.WorkPid, _workOrder.workType);
        //        } 
        //    }
        //}
    }
    #endregion
}
