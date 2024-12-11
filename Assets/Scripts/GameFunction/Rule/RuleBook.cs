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
    private RuleBookTileAction m_tileActionPart;
   
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

        public void AttackPlace(TokenTile _tile)
        {
            _tile.AttackTile((int)t_oriignDamage);
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

    public RuleBook()
    {
        RuleBookTileAction tilePart = new();
        m_tileActionPart = tilePart;
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

                if(targetTile.m_Side != _playChar.m_Side)
                attackProgress.AttackPlace(targetTile);
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
        else if(actionType == ActionType.Wrongfulness)
        {
            animateCoroutine = co_WrongFulAction(_playChar, actionToken);
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
        while (Vector2.Distance(_char.GetObject().transform.position, goal) > GamePlayMaster.c_movePrecision* GamePlayMaster.GetInstance().m_playSpeed)
        {
           _char.GetObject().transform.position += (dir.normalized * GamePlayMaster.GetInstance().m_moveSpeed * Time.deltaTime * GamePlayMaster.GetInstance().m_playSpeed);
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

    IEnumerator co_WrongFulAction(TokenChar _devil, TokenAction _wrongAction)
    {
        //애니메이션 동작시간 
        float waitTime = 0.5f; //임의로 0.5 이후 해당 액션 이벤트 종료 시간에 맞추기 
        while (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }

     //   Debug.Log(_devil.GetItemName() + "이 " + _wrongAction.GetItemName() + "부정을 끼쳤다.");
        TokenTile tile = _devil.GetTargetTile();
        tile.GetNation().CalStat(NationStatEnum.Happy, -_wrongAction.GetStat(CharActionStat.Power));

        GamePlayMaster.GetInstance().DoneCharAction(_devil);
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
        if (_char.IsPlayerChar() && _char.GetStat(CharStat.CurActionEnergy) < _action.GetStat(CharActionStat.NeedActionEnergy))
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

    #region 타일 액션
    public bool AbleOccupy(TokenTile _tile)
    {
        //해당 타일 소속 국가가 없으면 가능
        if (_tile.GetStat(ETileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER))
            return true;

        return false;
    }

    public void ConductTileAction(TokenTile _tile, TOrderItem _actionOrder, TileType _actionPlace)
    {
        m_tileActionPart.ConductTileAction(_tile, _actionOrder, _actionPlace);
    }

    #endregion

    #region 캐릭 스텟 강화
    public void IntenseStat(TokenChar _targetChar, CharStat _targetStat)
    {
        _targetChar.CalStat(_targetStat, 100);
    }
    #endregion

    #region 자원 규칙
    public TMineTileResult MineResource(TokenTile _tile, int _findAbility = 0)
    {
        List<(Capital, int)> result = new();
        TMineTileResult mineResult = new(result);
        TileTypeData tileData = MgMasterData.GetInstance().GetTileData((int)_tile.tileType);
        TileEffectEnum tileEffect = tileData.effectType;

        //해당 타일이 수급가능한 토지가 아니면 얻는거 없이 반환
        if (tileEffect != TileEffectEnum.Money)
            return mineResult;
        
        List<TOrderItem> resourceList = tileData.EffectData.GetItemList();
        //해당 타일에서 산출되는 자원 마스터 데이터 가져옴 
        for (int i = 0; i < resourceList.Count; i++)
        {
            TOrderItem resource = resourceList[i];
            //한번더 자원종류인지 체크하고
            if (resource.Tokentype == TokenType.Capital)
            {
                if(AbleFindCapital((Capital)resource.SubIdx, _findAbility))
                    result.Add(((Capital)resource.SubIdx, resource.Value));
                continue;
            }
            if(resource.Tokentype == TokenType.Random)
            {
                TOrderItem selectItem = SelectRandomItem(resource);
                if (AbleFindCapital((Capital)selectItem.SubIdx, _findAbility))
                    result.Add(((Capital)selectItem.SubIdx, selectItem.Value));
                continue;
            }
                
        }

        return mineResult;
    }

    public TOrderItem SelectRandomItem(TOrderItem _randomOrder)
    {
        RandomTypeEnum randomType = (RandomTypeEnum)_randomOrder.SubIdx;
        int value = _randomOrder.Value;
        TOrderItem selectItem = new TOrderItem(TokenType.None,0,0);
        switch (randomType)
        {
            case RandomTypeEnum.Capital1:
                //1. 
                int amount = value; //레벨에 맞는 자원들을 훑어와야함. 
                List<Capital> randomList = new();
                Array capital = System.Enum.GetValues(typeof(Capital));
                for (int i = 0; i < capital.Length; i++)
                {
                    Capital test = (Capital)capital.GetValue(i);
                    CapitalData capitalData = MgMasterData.GetInstance().GetCapitalData(test);
                    if (capitalData == null)
                    {
                        Debug.LogError("없는 데이터 토큰타입" + test);
                        continue;
                    }
                        
                    if(capitalData.Tier == 1)
                    {
                        randomList.Add((Capital)capitalData.capital);
                    }
                }
                if (randomList.Count == 0)
                    break;

                Capital selectCapital = randomList[GameUtil.GetRandomNum(randomList.Count, 1)[0]];
                selectItem = new TOrderItem(TokenType.Capital, (int)selectCapital, amount);
                break; 
        }
        return selectItem;
    }


    private bool AbleFindCapital(Capital _cpaital, int _findAbility)
    {
        //해당 자원을 찾았는지 여부 
        CapitalData capitalData = MgMasterData.GetInstance().GetCapitalData(_cpaital);
        int ratio = capitalData.baseRatio + _findAbility;
        if (ratio >= 100)
            return true;
        if (ratio <= 0)
            return false;
        int random = UnityEngine.Random.Range(1, 101);

        //Debug.Log(ratio + "범위중에" + random + "수치 뽑음");
        return random <= ratio;
    }

    public void MixCapital(List<(Capital, int)> _resources)
    {
        PlayerCapitalData playerCapital = PlayerCapitalData.g_instance;
        TItemListData costData = new TItemListData(_resources);
        if (playerCapital.CheckInventory(costData) == false)
        {
            Announcer.Instance.AnnounceState("합성 재료 부족", true);
            return;
        }
        playerCapital.PayCostData(costData);//사용한만큼 감소 시키고
        (Capital, int) mixed = m_capitalRecipe.MixCapital(_resources);
        TItemListData mixedData = new TItemListData(mixed);
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
    #endregion

}

public enum CapitalAction
{
    //ActionToken 3번의 재료 합성 타입에서 추가 분류
    MixCapital, ChageCapital
}
