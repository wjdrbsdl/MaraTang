using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#region 타일 enum
public enum TileType
{
    //각 타일의 타입 기본 산, 노말, 강 거기에 용도변경으로 농지,광산, 마을 단계까지 모두 정의 해놓기? 아니면 레벨은 따로"?
    Nomal, WoodLand, Farm, Town, Mine, Capital, Mountain, Cage=16
}

public enum TileViewState
{
    Fog, Sight
}

public enum TileAction
{
    Grass, Mineral, RemoveGrass, RemoveMineral, Spawn = 11
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
    List<TokenChar> m_inTileCharList = new();
    [SerializeField]
    public TileType tileType;
    public List<int> doneInteriorList; //지어진 장소
    public int ChunkNum;
    public WorkOrder m_workOrder = null; //진행중인 공사
    public WorkOrder m_outBuildOrder;
    private TileViewState m_viewState = TileViewState.Fog;
    private TokenEvent m_enteranceEvent; //입장시 발동하는 이벤트가 있는가
    MainResource m_density = MainResource.Tree;
    private int densityGrade = 0;
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

    public void SetDensityValue((MainResource, int) _mildo)
    {
        m_density = _mildo.Item1;
        densityGrade = _mildo.Item2;
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
       

        int minGradeValue = 25;
        int gradeValue = resourceValue % 25 + minGradeValue; //최소값 25에 등급으로 추가
        //Debug.Log(resourceValue + " 해당 용도에서 등급은" + gradeValue);
        SetStatValue(TileStat.MainResource, (int)mainResource); //해당 벨류로 넣음. 
        SetStatValue(TileStat.TileEnergy, gradeValue); //해당 벨류로 넣음. 
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

    #region 작업서
    public bool RegisterWork(WorkOrder _work)
    {
        if(m_workOrder != null)
        {
            Debug.Log("공사는 하나만");
            return false;
        }

        m_workOrder = _work;
        _work.m_orderPlace = this;
        Debug.Log(_work.m_workType + "작업 타일에 등록");
        return true;
    }

    public void RemoveWork()
    {
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

    public void CompleteOutBuild(TileType _tileType)
    {
        tileType = _tileType;

        SetTileSprite();
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
        DoAutoTileAction();
    }
    #endregion

    private void DoAutoTileAction()
    {
        //최초 장소가 건설되었을 때 자동 기능 수행
        int[] ablePid = MgMasterData.GetInstance().GetTileData((int)tileType).AbleTileActionPID;
        for (int i = 0; i < ablePid.Length; i++)
        {
            TokenTileAction action = MgMasterData.GetInstance().GetTileAction(ablePid[i]);
            if (action.IsAutoStart)
            {
                Debug.Log("자동 시작함" + tileType);
                GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, action);
            }
                
        }

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
