using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#region 타일 enum
public enum TileType
{
    //각 타일의 타입 기본 산, 노말, 강 거기에 용도변경으로 농지,광산, 마을 단계까지 모두 정의 해놓기? 아니면 레벨은 따로"?
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
    public List<int> doneInteriorList; //지어진 장소
    public int ChunkNum;
    public List<WorkOrder> m_workOrder;
    public WorkOrder m_outBuildOrder;
    private TileViewState m_viewState = TileViewState.Fog;
    private TokenEvent m_enteranceEvent; //입장시 발동하는 이벤트가 있는가
  

    /*타일 상호 순서
     * 1. 타일에 1 캐릭 존재 - 타캐릭 점유시 입장불가
     * 2. 이벤트는 해당 타일 입장으로만 발동
     * 3. 타일 사용은 점유로 가능(타일위에 혹은 해당 영역 보유)하나 이벤트 존재시 사용 불가
     */

    #region 타일생성
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
        //타일 높이에 따라 강, 노말, 산으로 구별 
        int tileHeight = (int)(_ecoHeight * 100f);
        SetStatValue(TileStat.Height, tileHeight);
        if (tileHeight >= 66)
            tileType = TileType.Mountain;
    
    }

   

    public void SetResourceValue()
    {
        //int resourceValue = (int)(_resourceValue * 100f); //리소스 점수 0~99로 전환
        int resourceValue = Random.Range(0, 100);
        MainResource mainResource = MainResource.Mineral; //최고점 미네랄 기본 설정 
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
        int gradeValue = resourceValue % 25 + minGradeValue; //최소값 25에 등급으로 추가
        //Debug.Log(resourceValue + " 해당 용도에서 등급은" + gradeValue);
        SetStatValue(TileStat.MainResource, (int)mainResource); //해당 벨류로 넣음. 
        SetStatValue(TileStat.TileEnergy, gradeValue); //해당 벨류로 넣음. 
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

    #region 캐릭 이동 관련
    public void Migrate(TokenChar _char)
    {
        //이민 시켰으면 소속된 토큰의 지도상 좌표값도 수정
        m_inTileCharList.Add(_char);
        _char.SetMapIndex(m_xIndex, m_yIndex);
    }

    public void RemoveCharToken(TokenChar _char)
    {
        //이주 보냈으면 리스트에서 제거
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

    #region 타일 이미지
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
        //공사중일때 추가 가능한지 체크 
        if (m_workOrder.Count >= 1)
        {
            if(workType == WorkType.ChangeBuild)
            {
                Debug.Log("외부공사는 다른공사 중에 진행 불가");
                return false;
            }

            if(m_workOrder[0].m_workType == WorkType.ChangeBuild)
            {
                Debug.Log("외부공사 중엔 내부공사 추가도 불가");
                return false;
            }
        }

        //그외 최초의 외부 공사이거나, 내부공사 추가는 가능 
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
        //해당 타일에 미치고있는 신 가져오기
        Nation nation = GetNation();

        if (nation == null)
            return GodClassEnum.무;

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

    #region 입장 이벤트
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
