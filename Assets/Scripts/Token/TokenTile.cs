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
    private List<LaborCoin> m_laborCoinList = new(); //할당된 노동 코인들 
    [SerializeField]
    public TileType tileType;
    public TileEffectEnum m_effectType = TileEffectEnum.None;
    public List<int> doneInteriorList; //지어진 장소
    public int ChunkNum;
    public WorkOrder m_workOrder = null; //진행중인 공사
    public Complain m_complain = null; //진행중인 불만
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
        tileToken.m_tokenIValues = new int[GameUtil.EnumLength(TileStat.Height)];
        tileToken.m_tokenType = TokenType.Tile;
        tileToken.doneInteriorList = new();
        tileToken.childList = new();
        tileToken.SetTileEffect(); //초기 Nomal에 맞는 effect로 
        return tileToken;
    }

    public void SetHeightValue(float _ecoHeight)
    {
        //타일 높이에 따라 강, 노말, 산으로 구별 
        int tileHeight = (int)(_ecoHeight * 100f);
        SetStatValue(TileStat.Height, tileHeight);
        if (tileHeight >= 66)
        {
            //생성단계에서 nomal이 아닌경우 장소와 효과를 다시 정의 
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
        //장소마다 따로 특정되어잇는 벨류값들을 재할당하는 부분
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        int[] masterValue = tileData.TileStat;
        m_tokenIValues[(int)TileStat.PlaceSight] = masterValue[(int)TileStat.PlaceSight];
        m_tokenIValues[(int)TileStat.MaxDurability] = masterValue[(int)TileStat.MaxDurability];
        m_tokenIValues[(int)TileStat.CurDurability] = m_tokenIValues[(int)TileStat.MaxDurability];
        // Debug.Log(tileType + "시야 거리 " + m_tokenIValues[(int)TileStat.PlaceSight]);
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
        SetTileEffect();
        SetTileSprite();
        SetTileValue();
        DoAutoTileAction();
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
       // Debug.Log(_work.m_workType + "작업 타일에 등록");
        MgWorkOrderPin.GetInstance().RequestWorkOrderPin(this);
        return true;
    }

    public void SendWorkStep(WorkStateCode _code)
    {
        switch (_code)
        {
            case WorkStateCode.Complete:
                WorkOrder preWork = m_workOrder; //완료된작업 저장
                RemoveWork(); //기존 작업은 없애고
                RepeatTileAction(preWork);
                break;
            case WorkStateCode.Cancle:
                RemoveWork();
                break;
        }
    }

    public void RemoveWork()
    {
        //1. 작업오더가 핀의 키값이므로 먼저 핀제거 요청 
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


    #endregion

    #region 노동코인
    public int GetLaborCoinCount()
    {
        return m_laborCoinList.Count;
    }

    public void PutInLaborCoin(LaborCoin _coin)
    {
        //Debug.Log(GetNationNum() + "번 국가 해당 타일에 " + _coin.ListIndex + " 코인 투입");
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

    #region 불만,사고,요청
    public bool SendComplain(Complain _complain)
    {
        if (m_complain != null)
        {
            Debug.Log("사건사고는 하나만");
            return false;
        }

        m_complain = _complain;
        MgWorkOrderPin.GetInstance().RequestComplainPin(this);
        return true;
    }

    public void RemoveComplain()
    {
        //1. 작업오더가 핀의 키값이므로 먼저 핀제거 요청 
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
            Debug.Log("노말타입엔 공격 의미 없음");
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
            //재귀가 되는데, 어차피 자식들은 자식 타일이 없어서 한번만 발생. 그래도 파괴판정은 들어갈수있으므로 해당함수로 진행
            childTile.DestroyPlace(); 
        }
    }
    #endregion

    #region 타일 기능 수행
    public void DoneWorkReady()
    {
        //고유 작업을 위한 준비가 끝났을때
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
        Debug.Log("자동 진행 체크 필요 ");
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)tileType);
        TileEffectEnum effectType = tileData.effectType;

        if (effectType == TileEffectEnum.Tool)
        {
            Debug.Log(tileType + "에서 작업서 자동 등록");
            new WorkOrder(null, tileData.NeedLaborTurn, tileData.NeedLaborAmount, this, 0, WorkType.Inherence);
        }
        //for (int i = 0; i < ablePid.Length; i++)
        //{
        //    TokenTileAction action = MgMasterData.GetInstance().GetTileAction(ablePid[i]);
        //    if (action == null)
        //        continue;
        //    if (action.IsAutoStart)
        //    {
        //        Debug.Log("자동 시작함" + tileType);
        //        GamePlayMaster.GetInstance().RuleBook.ConductTileAction(this, action);
        //    }
        //}
    }

    private void RepeatTileAction(WorkOrder _workOrder)
    {
        Debug.Log("Torder 형태로 반복 체크 필요");
        //한번더 수행하는 작업인지 체크
        //int[] ablePid = MgMasterData.GetInstance().GetTileData((int)tileType).AbleTileActionPID;
        //for (int i = 0; i < ablePid.Length; i++)
        //{
        //    TokenTileAction action = MgMasterData.GetInstance().GetTileAction(ablePid[i]);
        //    if (action == null)
        //        continue;

        //    //타일액션 타입이 워크오더고
        //    if(action.GetStat(TileActionStat.TileActionType) == (int)TileActionType.WorkOrder)
        //    {
        //        //그 작업타입이 동일하다면 고유 기능이 완료된 상태이므로 일단 반복
        //        if(action.GetStat(TileActionStat.SubValue) == (int)_workOrder.workType)
        //        {
        //            Debug.Log("반복 작업 진행 " + _workOrder.workType);
        //            new WorkOrder(_workOrder.GetNeedList(), _workOrder.GetWorkGauge(), this, _workOrder.WorkPid, _workOrder.workType);
        //        } 
        //    }
        //}
    }
    #endregion
}
