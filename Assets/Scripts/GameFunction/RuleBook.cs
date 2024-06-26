using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBook
{
    public static GamePlayMaster m_PlayMaster;
    private RouteDisplay m_routeDisplayTool = new();
    private Dictionary<int, TokenAction> m_tileActionDic;
    private CapitalRecipe m_capitalRecipe = new();
    public struct TAttackProgress
    {
        public float t_oriignDamage;
        public float t_reductedDamage;
        public TokenChar t_attacker;
        public int t_revengeStep; //공격한 단계

        public TAttackProgress(TokenChar _attackChar, TokenAction _attackAction, int _revenge = 1)
        {
            //구조를 만들면서 내부에서 최종피해량 산출
            t_oriignDamage = _attackAction.GetStat(CharActionStat.Power);
            t_reductedDamage = t_oriignDamage;
            t_attacker = _attackChar;
            t_revengeStep = _revenge;
        }

        public float CalDamageByDefense(TokenChar _defenseChar)
        {
            float reductedDamage = t_oriignDamage * 0.8f;
            return reductedDamage;
        }

        public void ApplyDamage(TokenChar _target)
        {
            float reductedDamage = CalDamageByDefense(_target);
            int damage = (int)reductedDamage;
            PopupDamage.GetInstance().DamagePop(_target.GetObject().gameObject, damage);
            _target.CalStat(CharStat.CurHp, -damage);
            MgHud.GetInstance().ShowCharHud(_target);
            if (_target.GetStat(CharStat.CurHp) <= 0)
            {
           //     Debug.Log(_target.GetItemName()+"사망 체력"+ _target.GetStat(CharStat.CurActionEnergy));
                _target.Death();
                return;
            }
            //Revenge(_target);
        }

        public void Revenge(TokenChar _defenseChar)
        {
         //   Debug.Log("복수의 굴레 :" + t_revengeStep);
            if (t_revengeStep >= 2)
                return;

            TAttackProgress revenge = new TAttackProgress(_defenseChar, new TokenAction(), t_revengeStep +1);
            revenge.ApplyDamage(t_attacker);

        }
    }

    public struct TMineTileResult
    {
        List<(Capital, int)> resourceResultList; //각 자원 그리고 그양을 얼마나 캤는지 할당

        public TMineTileResult(List<(Capital, int)> _resourceResultList)
        {
            resourceResultList = _resourceResultList;
        }

        public List<(Capital, int)> GetResourceAmount()
        {
            return resourceResultList;
        }
    }

    #region 액션 수행 절차
    public void ReadCharAction(TokenChar _playChar)
    {
        TokenAction actionToken = _playChar.GetNextActionToken();
        ActionType actionType = actionToken.GetActionType();

        CalActionEnergy(_playChar, actionToken);

        //1. 필요한 행동들 선언
        Action effectDelegate = null; //액션 사용시 효과
        Action attackSfx = null; //액션의 스킬 이펙트
        IEnumerator animateCoroutine = null; //액션 수행 절차

        //2. 플레이어가 선택한 타겟 타일
        TokenTile clickedTile = GameUtil.GetTileTokenFromMap(actionToken.GetTargetPos()); //타겟 타일
        GameUtil.LookTargetTile(_playChar, clickedTile); //캐릭터 방향 조정

        //3. 공격은 타겟 지점 기준 범위내 적을 선별하여 어택
        if (actionType == ActionType.Attack)
        {
          //  Debug.Log("어택 내용 수행한다");
            //수정본
            //0. 겉으로 드러나는 액션은 1개. 휘두르거나 찌르거나 발사하거나 
            //1. 해당 공격액션의 범위를 설정
            TokenTile targetTile = clickedTile;
            attackSfx = delegate
            {
                MgSkillFx.GetInstance().MakeSkillFx(targetTile, "테스트");
            };
            //2. 범위 내의 타겟을 가져옴
            List<TokenChar> enemies = targetTile.GetCharsInTile();
            //3. 해당 타겟에게 해당 공격의 효과를 적용 
            effectDelegate = delegate
            {
                TAttackProgress attackProgress = new TAttackProgress(_playChar, actionToken);
                for (int i = 0; i < enemies.Count; i++)
                {
                    //   Debug.Log(_playChar.GetItemName() + "이 " + enemies[i].GetItemName() + "를 공격");
            
                    attackProgress.ApplyDamage(enemies[i]);
                }
            };
            animateCoroutine = co_AttacAction(_playChar, attackSfx, effectDelegate);

        }
        //4. 이동은 타겟 지점 위치로 이동
        else if (actionType == ActionType.Move)
        {
            TokenTile targetTile = clickedTile;
            effectDelegate = delegate 
            { 
                Migrate(_playChar, targetTile); 
            };
            animateCoroutine = co_MoveAction(_playChar, targetTile, effectDelegate);
        }

        //5. 준비된 예약으로 애니메이션 수행 
        GamePlayMaster.GetInstance().AnimateTokenObject(animateCoroutine, effectDelegate, _playChar);
    }

    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //해당 타일로 해당 캐릭터를 이주 시키기 
        MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()].RemoveCharToken(_char); //이사 보내고
        _targetTile.Migrate(_char); //이사 넣고 
        _char.GetObject().SyncObjectPosition();
    }

    public static void FirstMigrate(TokenChar _char, int[] _position)
    {
        //
        TokenTile tile = GameUtil.GetTileTokenFromMap(_position);
        tile.Migrate(_char);
        _char.GetObject().SyncObjectPosition();
    }

    IEnumerator co_MoveAction(TokenChar _char, TokenTile _goalTile, Action effectAction)
    {
        //   Debug.Log("이동 코루틴 수행 단계" + m_MaxStep+"/ " + curStep);

        Vector3 goal = _goalTile.GetObject().transform.position;

        _char.SetState(CharState.Move);

        Vector3 dir = goal - _char.GetObject().transform.position;
        while (Vector2.Distance(_char.GetObject().transform.position, goal) > GamePlayMaster.c_movePrecision)
        {
           _char.GetObject().transform.position += (dir.normalized * GamePlayMaster.GetInstance().m_moveSpeed * Time.deltaTime);
            yield return null;
        }
        if (effectAction != null)
        effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);
    }

    IEnumerator co_AttacAction(TokenChar _char, Action attackSfx, Action effectAction)
    {
        //   Debug.Log("이동 코루틴 수행 단계" + m_MaxStep+"/ " + curStep);
        _char.SetState(CharState.Attack);
        SoundManager.GetInstance().PlayEfx(SoundManager.EfxList.Attack);
        if (attackSfx != null)
            attackSfx();
        float waitTime = 1f;
        while (waitTime>0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        if (effectAction != null)
             effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);

    }

    private void CalActionEnergy(TokenChar _playChar, TokenAction _action)
    {
        //액션을 수행함에 있어 필요한 자원들을 계산하는 곳 

        //1. 해당 캐릭터에서 에너지와 액션 횟수를 감소
        int needCount = _action.GetStat(CharActionStat.NeedActionCount);
        _playChar.UseActionCount(needCount);
        _playChar.UseActionEnergy(_action.GetStat(CharActionStat.NeedActionEnergy));

        //2. 액션의 턴내 사용 횟수 감소 
        _action.CalStat(CharActionStat.RemainCountInTurn, -1); //액션토큰의 해당턴에서 가능한 사용 횟수 차감

    }

    #endregion

    #region 조건 체크 
    public bool IsInRangeTarget(TokenChar _char, TokenAction _action, TokenBase _target)
    {
        TMapIndex mapIndex = new TMapIndex(_char, _target);
        int targetRange = GameUtil.GetMinRange(mapIndex);

        //Debug.Log(_char.GetXIndex() + "," + _char.GetYIndex() + "에서 " + _target.GetXIndex() + "," + _target.GetYIndex() + "거리는 " + targetRange);
        if (targetRange < _action.GetStat(CharActionStat.MinRange))
            return false;

        if (_action.GetStat(CharActionStat.Range) < targetRange)
           return false;

        return true;
    }

    public bool IsAbleAction(TokenChar _char, TokenAction _action, ref string _failMessage)
    {
        //1. 액션의 소비 액션 카운트와 비교(액션소비카운트가 0 인 녀석인 경우 true)
        if(_char.GetActionCount() < _action.GetStat(CharActionStat.NeedActionCount))
        {
            _failMessage = "행동 카운트 부족 \n보유 :" + _char.GetActionCount() + "\n필요:"
                + _action.GetStat(CharActionStat.NeedActionCount);
            return false;
        }

        //2. 액션의 소비 에너지
        if (_char.GetStat(CharStat.CurActionEnergy) < _action.GetStat(CharActionStat.NeedActionEnergy))
        {
            _failMessage = "행동 에너지 부족 \n보유 :"+ _char.GetStat(CharStat.CurActionEnergy) +"\n필요:"
                + _action.GetStat(CharActionStat.NeedActionEnergy);
            return false;
        }

        //3. 액션 자체의 사용횟수(한 턴에 사용제한을 둬야하는 경우)
        if (_action.AbleUse() == false)
        {
            _failMessage = "사용불가 상태 액션";
            return false;
        }

        return true;
    }

    public bool CheckActionContent(TokenChar _char, TokenAction _action)
    {
        if (_action.GetTargetPos() == null)
        {
            Announcer.Instance.AnnounceState("타겟이 부정확", true);
            return false;
        }
            

        return true;
    }
    #endregion
   
    public TokenEvent CheckEnteranceEvent(int[] mapCoordi)
    {
        //타일에 즉발용 이벤트가 있는지 확인
        TokenTile mapTile = GameUtil.GetTileTokenFromMap(mapCoordi);
        //맵에 있는지 따져보고 반환
        TokenEvent enterEvent = mapTile.GetEneteranceEvent();

        return enterEvent;
    }

    #region 타일 액션
    public bool AbleOccupy(TokenTile _tile)
    {
        //해당 타일 소속 국가가 없으면 가능
        if (_tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER))
            return true;

        return false;
    }

    public TokenAction[] RequestTileActions(TokenTile _tile)
    {
        //해당 타일을 가지고 가능한 액션을 뽑아줌. 
        List<TokenAction> ableList = new List<TokenAction>();
        //1.해당 타일의 pid를 확인
        int tileType = (int)_tile.tileType;
        //2. pid에 맞는 타일 데이터 가져옴
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData(tileType); //해당 타입의 타일데이터를 가져옴
        if (tileData == null)
            return ableList.ToArray();
        //3. 가능한 액션 토큰 배열을 순환
        for (int i = 0; i < tileData.AbleTileActionPID.Length; i++)
        {
            //4. 타일 데이터의 작업 가능한 액션 pid 리스트에 해당하는 타일액션들을 찾아서 반환
            TokenAction ableAction = MgMasterData.GetInstance().GetTileActions(tileData.AbleTileActionPID[i]);
            if(ableAction == null)
            {
                Debug.Log("해당 pid에 해당하는 액션 없음");
                continue;
            }
            ableList.Add(ableAction);
        }
        //문제
        /*
         * 타일 액션은 해당 타일에 종속되어야하는데, 위는 원본 자료 참조 방식으로 모든 타일이 한 행동을 공유해서
         * 사용 상태등이 공유 되버림. 
         */

        
        return ableList.ToArray();
    }

    public void ConductTileAction(TokenTile _tile, TokenAction _action)
    {
        TokenChar player = MgToken.GetInstance().GetMainChar();
        if(GameUtil.GetMinRange(player, _tile) > 0 && GamePlayMaster.GetInstance().AdaptInTileForAct == true)
        {
            Debug.Log("해당 타일에 있어야 가능");
            return;
        }
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        int subValue = _action.GetStat(TileActionStat.SubValue); //해당 타입에서 부차적인 벨류
        //tileActionType으로 행태 구별 

        switch (tileActionType)
        {
            case TileActionType.Harvest:
                HarvestTile(_tile);
                MgUI.GetInstance().CancleLastUI();
                break;
            case TileActionType.Build:
                BuildTile(_tile, (TileType)subValue);
                MgUI.GetInstance().CancleLastUI();
                break;

            case TileActionType.CapitalChef:
                CapitalAction doCode = (CapitalAction)subValue;
                //재료 변환 
                MgUI.GetInstance().ShowCapitalWorkShop(doCode, _tile, _action);
                break;

            case TileActionType.LandUsage:
                UseTownFunction(_tile, subValue);
                break;

            case TileActionType.Destroy:
                BuildTile(_tile, TileType.Nomal);
                MgUI.GetInstance().CancleLastUI();
                break;
            default:
                MgUI.GetInstance().CancleLastUI();
                break;
        }
    
    }

    private void HarvestTile(TokenTile _tile)
    {
        List<(Capital, int)> mineResult = GamePlayMaster.GetInstance().RuleBook.MineResource(_tile).GetResourceAmount();
        for (int i = 0; i < mineResult.Count; i++)
        {
            //  Debug.Log(mineResult[i].Item1 + " 자원 채취" + mineResult[i].Item2);
            PlayerCapitalData.g_instance.CalCapital(mineResult[i].Item1, mineResult[i].Item2);
        }
    }
    public OrderCostData GetTileChangeCost(TileType _tileType)
    {
        OrderCostData costData = MgMasterData.GetInstance().GetTileData((int)_tileType).BuildCostData;
        return costData;
    }
    private void BuildTile(TokenTile _tile, TileType _tileType)
    {
        if (PlayerCapitalData.g_instance.CheckInventory(GetTileChangeCost(_tileType)) == true)
        {
            _tile.ChangeTileType(_tileType);
        }
    }

    private void UseTownFunction(TokenTile _tile, int _subValue)
    {
        Debug.Log(_subValue + "작업 수행 요청");
        TownFuction townFuction = (TownFuction)_subValue;
        switch (townFuction)
        {
            case TownFuction.GiveMoney:
                PlayerCapitalData _playerCapital = PlayerCapitalData.g_instance;
                //플레이어가 지불할 자원과 수를 가지고 코스트데이터를 생성
                List<(Capital, int)> _capitalList = new();
                _capitalList.Add((Capital.Food, 50));
                _capitalList.Add((Capital.Mineral, 50));
                _capitalList.Add((Capital.Person, 50));
               // OrderCostData _costData = _playerCapital.GetTrade(_capitalList);
                break;
        }
    }

    public enum TownFuction
    {
       GiveMoney = 1
    }

    #endregion

    #region 캐릭 스텟 강화
    public void IntenseStat(TokenChar _targetChar, CharStat _targetStat)
    {
        _targetChar.CalStat(_targetStat, 100);
    }
    #endregion

    public TMineTileResult MineResource(TokenTile _tile)
    {
        List<(Capital, int)> result = new();
        TMineTileResult mineResult = new(result);

        TileType territory = _tile.GetTileType();
        int mainType = _tile.GetStat(TileStat.MainResource);
        int tileEnegy = _tile.GetStat(TileStat.TileEnergy);
        
        //테이블로 각 타입에서 얻을 수 있는 자원을 정의해놓기?
        switch (territory)
        {
            case TileType.Nomal:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Town:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Farm:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Mine:
                result.Add((Capital.Food, tileEnegy));
                result.Add((Capital.Mineral, tileEnegy));
                result.Add((Capital.Wood, tileEnegy));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.WoodLand:
                result.Add((Capital.Food, tileEnegy*0));
                result.Add((Capital.Mineral, tileEnegy*1));
                result.Add((Capital.Wood, (int)(tileEnegy*1.5)));
                result.Add((Capital.Person, tileEnegy));
                break;
            case TileType.Capital:
                break;
            default:
                break;
        }

        return mineResult;
    }

    public void MixCapital(List<(Capital, int)> _resources)
    {
        PlayerCapitalData playerCapital = PlayerCapitalData.g_instance;
        OrderCostData costData = new OrderCostData(_resources);
        if (playerCapital.CheckInventory(costData) == false)
        {
            Announcer.Instance.AnnounceState("합성 재료 부족", true);
            return;
        }
        playerCapital.PayCostData(costData);//사용한만큼 감소 시키고
        (Capital, int) mixed = m_capitalRecipe.MixCapital(_resources);
        OrderCostData mixedData = new OrderCostData(mixed);
        bool isPay = false; //지불이 아님
        playerCapital.PayCostData(mixedData, isPay); //얻은 만큼 추가 시키고 
    }

    public void ChangeCapital((Capital, int) _input, Capital _outCapital)
    {
        PlayerCapitalData capitalData = PlayerCapitalData.g_instance;
        if (capitalData.IsEnough(_input.Item1, _input.Item2) == false)
        {
            Announcer.Instance.AnnounceState("변환 재료 부족", true);
            return;
        }
        capitalData.CalCapital(_input.Item1, -_input.Item2); //사용한만큼 감소 시키고
        (Capital, int) mixed = m_capitalRecipe.ChangeCapital(_input, _outCapital);
        capitalData.CalCapital(mixed.Item1, mixed.Item2); //얻은 만큼 추가 시키고 
    }

    public void AnnounceCapitalIncome()
    {

    }
}

public enum CapitalAction
{
    //ActionToken 3번의 재료 합성 타입에서 추가 분류
    MixCapital, ChageCapital
}
