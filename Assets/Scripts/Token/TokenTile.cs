using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

#region Ÿ�� enum
public enum TileType
{
    //�� Ÿ���� Ÿ�� �⺻ ��, �븻, �� �ű⿡ �뵵�������� ����,����, ���� �ܰ���� ��� ���� �س���? �ƴϸ� ������ ����"?
    Nomal, WoodLand, Farm, Town, Mine, Capital, Mountain, Cage=16, Child
}

public enum TileViewState
{
    Fog, Sight
}

public enum TileStat
{
   Nation, Height, MainResource, TileEnergy
}

public enum MainResource
{
    Tree, Food, Mineral
}
#endregion

public class TokenTile : TokenBase, IWorkOrderPlace
{
    private List<TokenChar> m_inTileCharList = new();
    [SerializeField]
    public TileType tileType;
    public TileEffectEnum effectType = TileEffectEnum.None;
    public List<int> doneInteriorList; //������ ���
    public int ChunkNum;
    public WorkOrder m_workOrder = null; //�������� ����
    [JsonProperty] private TileViewState m_viewState = TileViewState.Fog;
    [JsonProperty] private TokenEvent m_enteranceEvent; //����� �ߵ��ϴ� �̺�Ʈ�� �ִ°�
    [JsonProperty] MainResource m_density = MainResource.Tree;
    [JsonProperty] private int densityGrade = 0;
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
        return tileToken;
    }

    public void SetHeightValue(float _ecoHeight)
    {
        //Ÿ�� ���̿� ���� ��, �븻, ������ ���� 
        int tileHeight = (int)(_ecoHeight * 100f);
        SetStatValue(TileStat.Height, tileHeight);
        if (tileHeight >= 66)
            tileType = TileType.Mountain;
    
    }

    public void SetDensityValue((MainResource, int) _mildo)
    {
        m_density = _mildo.Item1;
        densityGrade = _mildo.Item2;
    }
   

    public void SetResourceValue()
    {
        //int resourceValue = (int)(_resourceValue * 100f); //���ҽ� ���� 0~99�� ��ȯ
        int resourceValue = Random.Range(0, 100);
        MainResource mainResource = MainResource.Mineral; //�ְ��� �̳׶� �⺻ ���� 
        if (resourceValue < 25)
        {
            mainResource = MainResource.Tree;
        }
        else if (resourceValue < 50)
        {
            mainResource = MainResource.Food;
        }
       

        int minGradeValue = 25;
        int gradeValue = resourceValue % 25 + minGradeValue; //�ּҰ� 25�� ������� �߰�
        //Debug.Log(resourceValue + " �ش� �뵵���� �����" + gradeValue);
        SetStatValue(TileStat.MainResource, (int)mainResource); //�ش� ������ ����. 
        SetStatValue(TileStat.TileEnergy, gradeValue); //�ش� ������ ����. 
    }

    public void SetTileSprite()
    {
        if(tileType != TileType.Nomal)
        {
            GetObject().SetSprite(TempSpriteBox.GetInstance().GetTileSprite(tileType));
            return;
        }

        GetObject().SetSprite(TempSpriteBox.GetInstance().GetBaseTile(m_density));
        List<int> ran = GameUtil.GetRandomNum(4, 4 - densityGrade);
        for (int i = 0; i < ran.Count; i++)
        {
            ObjectTile tileObj = (ObjectTile)GetObject();
            tileObj.SetElement(i, TempSpriteBox.GetInstance().GetTileElement(ran[i]));
        }
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

    #region �۾���
    public bool RegisterWork(WorkOrder _work)
    {
        if(m_workOrder != null)
        {
            Debug.Log("����� �ϳ���");
            return false;
        }

        m_workOrder = _work;
        _work.SetOrderPlace( this);
        Debug.Log(_work.m_workType + "�۾� Ÿ�Ͽ� ���");
        MgWorkOrderPin.GetInstance().RequestWorkOrderPin(this);
        return true;
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

        if (m_workOrder.m_workType != _workType || m_workOrder.m_workPid != _pid)
            return false;

        return true;
    }

    public bool IsOutBuilding()
    {
        if (m_workOrder == null)
            return false;

        if (m_workOrder.m_workType == WorkType.InterBuild)
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

    #region GetSet
    public TileType GetTileType()
    {
        return tileType;
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

    public void ChangePlace(TileType _tileType)
    {
        tileType = _tileType;

        SetTileSprite();
        DoAutoTileAction();
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
    #endregion

    private void DoAutoTileAction()
    {
        //���� ��Ұ� �Ǽ��Ǿ��� �� �ڵ� ��� ����
        int[] ablePid = MgMasterData.GetInstance().GetTileData((int)tileType).AbleTileActionPID;
        for (int i = 0; i < ablePid.Length; i++)
        {
            TokenTileAction action = MgMasterData.GetInstance().GetTileAction(ablePid[i]);
            if (action == null)
                continue;
            if (action.IsAutoStart)
            {
                Debug.Log("�ڵ� ������" + tileType);
                GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, action);
            }
                
        }

    }



    #region ���� �̺�Ʈ
    public TokenEvent GetEneteranceEvent()
    {
        return m_enteranceEvent;
    }

    public void SetEnteraceEvent(TokenEvent _enterEvent)
    {
        m_enteranceEvent = _enterEvent;
    }

    public void DeleteEnterEvent()
    {
        m_enteranceEvent = null;
    }
    #endregion
}
