using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using System;

#region 타일 enum
public enum TileType
{
    //각 타일의 타입 기본 산, 노말, 강 거기에 용도변경으로 농지,광산, 마을 단계까지 모두 정의 해놓기? 아니면 레벨은 따로"?
    None, Nomal, WoodLand, Farm, Town, Mine, Capital, Mountain, WaterLand,
    Training, Guild, Lab, Temple, Police, Motel, AbandonStatue, Clinic, Cave, Child, 
    WoodLand2, GoldMine, Mine2, Farm2, GoldWood, GoldFarm, 
    Hatchery폭력, Hatchery나태, Hatchery공포, Hatchery기만, Hatchery역병, Hatchery부패, Hatchery타락,
    ChunkCore
}

public enum TileViewState
{
    Fog, Sight, NoSight
}

public enum ETileStat
{
   Nation, BaseSight, PlaceSight, MaxDurability, CurDurability, Capability, Efficiency, MaxLaborCoin
}

public enum MainResource
{
    Tree, Food, Mineral
}
#endregion

public class TokenTile : TokenBase
{
    private List<TokenChar> m_inTileCharList = new();
    private List<LaborCoin> m_laborCoinList = new(); //할당된 노동 코인들 
    [SerializeField]
    public TileType tileType;
    public List<int> doneInteriorList; //지어진 장소
    public int ChunkNum;
    public bool IsReadyInherece =false;
    public bool IsAutoReady = false; //기존작업이 끝났을 떄 고유작업 준비를 자동으로 할것인가. 
    public bool NeedReady = false; //기존작업이 필요한 장소인가 - 귀속되는 장소는 모두 외부장소 속성값을 따라갈것. 
    public WorkOrder m_workOrder = null; //진행중인 공사
    public Complain m_complain = null; //진행중인 불만
    private float m_height;
    [JsonProperty] private TileViewState m_viewState = TileViewState.Fog;
    [JsonProperty] MainResource m_mainResource = MainResource.Tree;
    [JsonProperty] private int m_resourceGrade = 0;
    public int[] parent; //재료 관계시 부모 타일
    [JsonProperty] private List<int[]> childList;// 재료 관계시 자식 타일들
    
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
        tileToken.tileType = TileType.Nomal;
        tileToken.m_tokenIValues = new int[GameUtil.EnumLength(ETileStat.Nation)];
        tileToken.m_tokenType = TokenType.Tile;
        tileToken.doneInteriorList = new();
        tileToken.childList = new();
        return tileToken;
    }

    public void SetHeightValue(float _ecoHeight)
    {
        //타일 높이에 따라 강, 노말, 산으로 구별 
        int tileHeight = (int)(_ecoHeight * 100f);
        m_height = tileHeight;
        if (tileHeight >= 66)
        {
            //생성단계에서 nomal이 아닌경우 장소와 효과를 다시 정의 
            SetTileType(TileType.Mountain);
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
            //GetObject().SetSprite(TempSpriteBox.GetInstance().GetTileSprite(tileType));
            GetObject().SetSprite(TempSpriteBox.GetInstance().GetSprite(TokenType.Tile, (int)tileType));
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
        //장소마다 따로 특정되어잇는 벨류값들을 재할당하는 부분
        //1. 마스터 데이터 값 불러옴
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        int[] masterValue = tileData.TileStat;
        int size = masterValue.Length;

        //2. 보존이 필요한 값을 따로 저장
        int originNationNum = m_tokenIValues[(int)ETileStat.Nation]; //
        
        //3. 마스터 데이터대로 값들 다 복사
        System.Array.Copy(masterValue, m_tokenIValues, size); //스텟값 복사

        //4. 보존했던 값 덮어쓰기
        m_tokenIValues[(int)ETileStat.Nation] = originNationNum;

        //5.
        IsAutoReady = tileData.IsAutoReady;
        NeedReady = tileData.NeedReadyWork;
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

    #region 장소 변경
    public void ChangePlace(TileType _tileType)
    {
        SetTileType(_tileType);
        SetTileSprite();
        SetTileValue();
        CancleWork();
        SetInhereceReady(false); //고유기능 수행 false로 전환
        RepeatInhereceReady(); //맨처음 생성시 고유작업 준비 할지
    }
    #endregion 

    #region 작업서
    public bool RegisterWork(WorkOrder _work)
    {
        if(m_workOrder != null)
        {
           // Debug.Log("공사는 하나만");
            return false;
        }

        m_workOrder = _work;
       // Debug.Log(_work.m_workType + "작업 타일에 등록");
        MgWorkOrderPin.GetInstance().RequestWorkOrderPin(this);
        return true;
    }

    public void SendWorkStep(WorkStateCode _code)
    {
        WorkOrder preWork = m_workOrder; //이전 작업중이던거 저장
        RemoveWork(); //기존 작업은 없애고
        //완료인 경우에만 재 수행 여부 체크 
        if(_code == WorkStateCode.Complete)
        {
            if (CheckInhereceWork(preWork))
            {
                SetInhereceReady(true);
                SelfCastInhereceWork();
            }
        }
    }

    public void RemoveWork()
    {
        //1. 작업오더가 핀의 키값이므로 먼저 핀제거 요청 
        MgWorkOrderPin.GetInstance().RemovePin(this);
        m_workOrder = null;
        return;
    }

    public bool IsWorkingInterPlace(WorkType _workType, int _pid)
    {
        if (m_workOrder == null)
            return false;

        if (m_workOrder.workType != _workType || m_workOrder.WorkPid != _pid)
            return false;

        return true;
    }

    public bool IsWorking()
    {
        //다른 건설중인가
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

    public void CancleWork()
    {
        //작업중이던 땅에 컨텐츠로 그냥 Change가 일어난경우
        //해당 타일에 토지변경 작업이 아닌 다른 작업중에 Change 가 발생가능
        //진행중이던 작업이 없으면 상관없음
        if (m_workOrder == null)
            return;

        m_workOrder.Cancle(); //있으면 그건 취소. 
        
    }
    #endregion

    #region 노동코인
    public int GetMaxLaborCoinCount()
    {
        return GetStat(ETileStat.MaxLaborCoin);
    }

    public int GetLaborCoinCount()
    {
        return m_laborCoinList.Count;
    }

    public List<LaborCoin> GetLaborList()
    {
        return m_laborCoinList;
    }

    public void PutInLaborCoin(LaborCoin _coin)
    {
        //해당 로직은 LaborCoin 자체에서 진행
        //Debug.Log(GetNationNum() + "번 국가 해당 타일에 " + _coin.ListIndex + " 코인 투입");
        if(m_laborCoinList.IndexOf(_coin)== -1)
        {
            m_laborCoinList.Add(_coin);
        }
    }

    public void TakeOutLaborCoin(LaborCoin _coin)
    {
        //해당 로직은 LaborCoin 자체에서 진행
        if (m_laborCoinList.IndexOf(_coin) != -1)
        {
            m_laborCoinList.Remove(_coin);
        }
    }

    public LaborCoin RequestLaborCoin()
    {
        //보유한 리스트중 0 번째를 반환
        if (m_laborCoinList.Count == 0)
            return null;

        LaborCoin firstLabor = m_laborCoinList[0];
        //뺀걸 해당 리스트에서 뺼필욘없음. 
        //반환한 코인을 다른곳에 GoWork()보내면 그 함수에서 SetPos로 이곳에서 takeOut이 호출 
        return firstLabor;
    }
    #endregion

    #region 불만,사고,요청
    public bool SendComplain(Complain _complain)
    {
        if (m_complain != null)
        {
            Debug.Log("사건사고는 하나만");
            return false;
        }

        m_complain = _complain;
        GetNation().AddComplaint(_complain); //해당 국가에 민원 등록
        MgWorkOrderPin.GetInstance().RequestComplainPin(this);
        return true;
    }

    public void RemoveComplain()
    {
        //1. 작업오더가 핀의 키값이므로 먼저 핀제거 요청 
        MgWorkOrderPin.GetInstance().RemoveComplainPin(this);
        GetNation().RemoveCompalint(m_complain); //해당 국가에 민원 삭제
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

    public void SetNation(int _nationNumber)
    {
        SetStatValue(ETileStat.Nation, _nationNumber);
    }

    public int GetNationNum()
    {
        return m_tokenIValues[(int)ETileStat.Nation];
    }

    public Nation GetNation()
    {
        int nationNum = m_tokenIValues[(int)ETileStat.Nation];
        if (nationNum.Equals(FixedValue.NO_NATION_NUMBER))
            return null;

        return MgNation.GetInstance().GetNation(m_tokenIValues[(int)ETileStat.Nation]);
    }

    public GodClassEnum GetGodClass()
    {
        //해당 타일에 미치고있는 신 가져오기
        Nation nation = GetNation();

        if (nation == null)
            return GodClassEnum.무;

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
            //만약 자신의 자식이 있다면 그자식들의 부모도 바꿈
            GameUtil.GetTileTokenFromMap(childList[i]).SetParentIndex(_parentTile);
        }
        //자식들의 부모를 바꾸고나면 자신의 자식을 초기화 시켜서 중복되지 않도록 
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

    #region 타일 내구도
    public void AttackTile(int _damage)
    {
        if (tileType == TileType.Nomal)
        {
         //   Debug.Log("노말타입엔 공격 의미 없음");
            return;
        }
        CalStat(ETileStat.CurDurability, -_damage);
        CheckDurability();
    }

    public override void CalStat(Enum _enumIndex, int _value)
    {
        base.CalStat(_enumIndex, _value);
    }

    public void CheckDurability()
    {
        int durability = GetStat(ETileStat.CurDurability);
        if(durability <= 0)
        {
            SetStatValue(ETileStat.CurDurability, 0);
            DestroyPlace();
        }
    }

    public void DestroyPlace()
    {
        TileType preType = tileType;
        ChangePlace(TileType.Nomal);
        for (int i = 0; i < childList.Count; i++)
        {
            TokenTile childTile = GameUtil.GetTileTokenFromMap(childList[i]);
            //재귀가 되는데, 어차피 자식들은 자식 타일이 없어서 한번만 발생. 그래도 파괴판정은 들어갈수있으므로 해당함수로 진행
            childTile.DestroyPlace(); 
        }
        //이전에 파괴되었던 부분이 수도면 
        if(preType == TileType.Capital)
        {
            MgNation.GetInstance().DestroyNation(GetNation());
        }
    }
    #endregion

    #region 타일 고유 기능 수행
    public void SetInhereceReady(bool _ready)
    {
        IsReadyInherece = _ready;
    }

    public void DoInhereceWork(TileType _tileType)
    {
        // Debug.Log("고유기능 수행");
        
        //준비가 필요한 기능인데 준비가 안되었으면 패싱
        if (NeedReady && IsReadyInherece == false)
            return;
      
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)_tileType);
        List<TOrderItem> effectList = tileData.EffectData.GetItemList();
        for (int i = 0; i < effectList.Count; i++)
        {
            GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, effectList[i], _tileType);
        }
        //고유작업 사용유무는 각 기능에서 따로 
        //DoneInhereceFunction();
        if (tileData.NeedCommand == false)
            DoneInhereceFunction(); //따로 조작이 필요없이 사용하기 기능을 수행한경우에는 다음 작업 준비 

        //따로 조작이 필요한 경우에는 - UI를 열고 선택을 하는등 그 구조에서 결과가 수행되었을때 호출할것. 
    }

    public void SelfCastInhereceWork()
    {
        //준비작업이 완료되었을때 자동 캐스팅 
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        if (tileData.NeedCommand == true) //별도 추가 조작이 필요한거면 자동수행 못함
            return;
        if (tileData.ImmediatelyEffect == false) //즉시 적용 효과가 아니면 패스 
            return;
        if (IsReadyInherece == false)
        {
            // Debug.Log("고유 기능 수행 준비 안 되었음");
            return;
        }
        List<TOrderItem> effectList = tileData.EffectData.GetItemList();
        for (int i = 0; i < effectList.Count; i++)
        {
            GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, effectList[i], tileType);
        }
        DoneInhereceFunction(); //셀프 캐스팅 이후 작업마무리
    }

    public void DoneInhereceFunction()
    {
        SetInhereceReady(false);
        RepeatInhereceReady();
    }

    public void RepeatInhereceReady()
    {
        //해당 타일에서 고유작업을 준비 - 내부 장소도 모두 외부 장소의 준비상태에 귀속
        if (IsReadyInherece == true)
            return;
        
        //자동 준비 기능이면 준비 수행
        if(IsAutoReady)
           ReadyInherenceWork();
    }

    private bool CheckInhereceWork(WorkOrder _workOrder)
    {
        //  Debug.Log("고유여부 " + (_workOrder.workType == WorkType.Inherence));
        return _workOrder.workType == WorkType.Inherence;
    }

    public void ReadyInherenceWork()
    {
        //이미 준비가 된 상태 타일이면 준비는 미실행
        if (IsReadyInherece == true)
            return;

        //다른 일 혹은 이미 준비 작업이 진행중이면 추가 작업 불가 
        if (m_workOrder != null)
            return;

        //해당 장소의 고유작업에 필요한 자원으로 작업오더 생성. 
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        new WorkOrder(null, tileData.NeedLaborTurn, tileData.NeedLaborAmount, this, (int)tileType, WorkType.Inherence);
    }

    #endregion
}
