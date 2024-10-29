using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#region Ÿ�� enum
public enum TileType
{
    //�� Ÿ���� Ÿ�� �⺻ ��, �븻, �� �ű⿡ �뵵�������� ����,����, ���� �ܰ���� ��� ���� �س���? �ƴϸ� ������ ����"?
    Nomal, WoodLand, Farm, Town, Mine, Capital, Mountain
}

public enum TileViewState
{
    Fog, Sight
}

public enum TileAction
{
    Grass, Mineral, RemoveGrass, RemoveMineral
}

public enum TileStat
{
   Nation, Height, MainResource, TileEnergy
}

public enum MainResource
{
    Tree, Food, House, Mineral
}
#endregion

public class TokenTile : TokenBase
{
    List<TokenChar> m_inTileCharList = new();
    [SerializeField]
    public TileType tileType;
    public List<int> doneInteriorList; //������ ���
    public int ChunkNum;
    public List<WorkOrder> m_workOrder;
    public WorkOrder m_outBuildOrder;
    private TileViewState m_viewState = TileViewState.Fog;
    private TokenEvent m_enteranceEvent; //����� �ߵ��ϴ� �̺�Ʈ�� �ִ°�
  

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
        tileToken.m_workOrder = new();
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
        else if (resourceValue < 75)
        {
            mainResource = MainResource.House;
        }

        int minGradeValue = 25;
        int gradeValue = resourceValue % 25 + minGradeValue; //�ּҰ� 25�� ������� �߰�
        //Debug.Log(resourceValue + " �ش� �뵵���� �����" + gradeValue);
        SetStatValue(TileStat.MainResource, (int)mainResource); //�ش� ������ ����. 
        SetStatValue(TileStat.TileEnergy, gradeValue); //�ش� ������ ����. 
    }

    public void SetTileSprite()
    {
        if(tileType == TileType.Capital)
        {
            GetObject().SetSprite(TempSpriteBox.GetInstance().GetTileSprite(tileType));
            return;
        }
        
        List<int> ran = GameUtil.GetRandomNum(4, 3);
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

    public bool RegisterWork(WorkOrder _work)
    {
        WorkType workType = _work.m_workType;
        //�������϶� �߰� �������� üũ 
        if (m_workOrder.Count >= 1)
        {
            if(workType == WorkType.ChangeBuild)
            {
                Debug.Log("�ܺΰ���� �ٸ����� �߿� ���� �Ұ�");
                return false;
            }

            if(m_workOrder[0].m_workType == WorkType.ChangeBuild)
            {
                Debug.Log("�ܺΰ��� �߿� ���ΰ��� �߰��� �Ұ�");
                return false;
            }
        }

        //�׿� ������ �ܺ� �����̰ų�, ���ΰ��� �߰��� ���� 
        m_workOrder.Add(_work);
        GamePlayMaster.GetInstance().RegistorWork(_work);
        return true;
    }

    public void RemoveWork(WorkOrder _work)
    {
        m_workOrder.Remove(_work);
        GamePlayMaster.GetInstance().RemoveWork(_work);
    }

    public bool IsWorking(WorkType _workType, int _pid)
    {
        for (int i = 0; i < m_workOrder.Count; i++)
        {
            if (m_workOrder[i].m_workType == _workType && m_workOrder[i].m_workPid == _pid)
            {
                return true;
            }
        }
        return false;
    }

    public TileType GetTileType()
    {
        return tileType;
    }

    public void SetNation(int _nationNumber)
    {
        SetStatValue(TileStat.Nation, _nationNumber);
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
    }

    public void BuildInterior(int _pid)
    {
        doneInteriorList.Add(_pid);
    }

    public bool IsBuildInterior(int _pid)
    {
        if (doneInteriorList.IndexOf(_pid) == -1)
            return false;

        return true;
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
