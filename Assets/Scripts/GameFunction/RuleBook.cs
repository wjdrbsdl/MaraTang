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
    public struct TAttackRecipe
    {
        public float t_oriignDamage;
        public float t_reductedDamage;
        public TokenChar t_attacker;
        public int t_revengeStep; //공격한 단계

        public TAttackRecipe(TokenChar _attackChar, TokenAction _attackAction, int _revenge = 1)
        {
            //구조를 만들면서 내부에서 최종피해량 산출
            t_oriignDamage = _attackChar.GetPid() + 1000;
            t_reductedDamage = t_oriignDamage;
            t_attacker = _attackChar;
            t_revengeStep = _revenge;
        }

        public float CalDamageByDefense(TokenChar _defenseChar)
        {
            float reductedDamage = t_oriignDamage * 0.6f;
            return reductedDamage;
        }

        public void ApplyDamage(TokenChar _target)
        {
            float reductedDamage = CalDamageByDefense(_target);
            int damage = (int)reductedDamage;
            PopupDamage.GetInstance().DamagePop(_target.GetObject().gameObject, damage);
            _target.CalStat(CharStat.CurActionEnergy, -damage);
            if (_target.GetStat(CharStat.CurActionEnergy) <= 0)
            {
                Debug.Log(_target.GetItemName()+"사망 체력"+ _target.GetStat(CharStat.CurActionEnergy));
                _target.Death();
                return;
            }
            Revenge(_target);
        }

        public void Revenge(TokenChar _defenseChar)
        {
            Debug.Log("복수의 굴레 :" + t_revengeStep);
            if (t_revengeStep >= 2)
                return;

            TAttackRecipe revenge = new TAttackRecipe(_defenseChar, new TokenAction(), t_revengeStep +1);
            revenge.ApplyDamage(t_attacker);

        }
    }

    public void ParseTileActions()
    {
        //타일의 고유 속성값에 따라 가능한 액션이 다를 뿐 - 가능한 액션 풀은 모든 타일에서 동일
        //또한 액션에 사용되는 값 또한 타일마다 모두 동일 
        //따라서 한 액션타일 풀을 가지고 돌아가면서 사용가능. 

        m_tileActionDic = MgToken.GetInstance().GetTileActions(); 
    }

    #region 액션 수행 절차
    public void ReadCharAction(TokenChar _playChar)
    {
        TokenAction actionToken = _playChar.GetNextActionToken();
        ActionType actionType = actionToken.GetActionType();
        //1. 액션토큰 횟수 감소
        actionToken.CalStat(CharActionStat.RemainCountInTurn, -1); //액션토큰의 해당턴에서 가능한 사용 횟수 차감
    
        int[] targetPos = actionToken.GetTargetPos();

        Action effectDelegate = null;
        IEnumerator animateCoroutine = null;

        //2. 공격은 타겟 지점 기준 범위내 적을 선별하여 어택
        if (actionType == ActionType.Attack)
        {
            Debug.Log("어택 내용 수행한다");
            //수정본
            //0. 겉으로 드러나는 액션은 1개. 휘두르거나 찌르거나 발사하거나 
            //1. 해당 공격액션의 범위를 설정
            TokenTile targetTile = GameUtil.GetTileTokenFromMap(targetPos);
            //2. 범위 내의 타겟을 가져옴
            List<TokenChar> enemies = targetTile.GetCharsInTile();
            //3. 해당 타겟에게 해당 공격의 효과를 적용 
            effectDelegate = delegate
            {
                TAttackRecipe attackReceipt = new TAttackRecipe(_playChar, actionToken);
                for (int i = 0; i < enemies.Count; i++)
                {
                    Debug.Log(_playChar.GetItemName() + "이 " + enemies[i].GetItemName() + "를 공격");
                    attackReceipt.ApplyDamage(enemies[i]);
                }
            };
            animateCoroutine = co_AttacAction(_playChar, effectDelegate);

        }
        //3. 이동은 타겟 지점 위치로 이동
        else if (actionType == ActionType.Move)
        {
            TokenTile targetTile = GameUtil.GetTileTokenFromMap(targetPos);
            effectDelegate = delegate 
            { 
                Migrate(_playChar, targetTile); 
            };
            animateCoroutine = co_MoveAction(_playChar, targetTile, effectDelegate);
        }
        GamePlayMaster.GetInstance().AnimateTokenObject(animateCoroutine, effectDelegate, _playChar);
    }

    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //해당 타일로 해당 캐릭터를 이주 시키기 
        MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()].RemoveToken(_char); //이사 보내고
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

        effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);
    }

    IEnumerator co_AttacAction(TokenChar _char, Action effectAction)
    {
        //   Debug.Log("이동 코루틴 수행 단계" + m_MaxStep+"/ " + curStep);
        _char.SetState(CharState.Attack);
        float waitTime = 1f;
        while (waitTime>0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }
        effectAction();
        GamePlayMaster.GetInstance().DoneCharAction(_char);

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

    public bool IsAbleAction(TokenChar _char, TokenAction _action)
    {
        //1. 액션의 소비 액션 카운트와 비교(액션소비카운트가 0 인 녀석인 경우 true)
        if(_char.GetActionCount() < _action.GetStat(CharActionStat.NeedActionCount))
        {
            Announcer.Instance.AnnounceState("행동 카운트 부족");
            return false;
        }

        //2. 액션의 소비 에너지
        if (_char.GetStat(CharStat.CurActionEnergy) < _action.GetStat(CharActionStat.NeedActionEnergy))
        {
            Announcer.Instance.AnnounceState("행동 에너지 부족");
            return false;
        }

        //3. 액션 자체의 사용횟수(한 턴에 사용제한을 둬야하는 경우)
        if (_action.AbleUse() == false)
        {
            Announcer.Instance.AnnounceState("사용불가 상태 액션");
            return false;
        }

        return true;
    }

    public bool CheckActionContent(TokenChar _char, TokenAction _action)
    {
        if (_action.GetTargetPos() == null)
        {
            Announcer.Instance.AnnounceState("타겟이 부정확");
            return false;
        }
            

        return true;
    }
    #endregion

    #region 이벤트 발생 
    public void PlayEntranceEvent(TokenEvent _eneterEvent)
    {
        //타일에 이동한 것만으로 발생하는 이벤트
        Debug.Log("입장시 이벤트 발생");
    }

    public TokenEvent CheckEnteranceEvent(int[] mapCoordi)
    {
        //타일에 즉발용 이벤트가 있는지 확인
        TokenTile mapTile = GameUtil.GetTileTokenFromMap(mapCoordi);
        //맵에 있는지 따져보고 반환
        TokenEvent enterEvent = mapTile.GetEneteranceEvent();

        return enterEvent;
    }

    public void OnTileArrive(TokenChar _char)
    {
        //플레이어 매인 캐릭터가 새로운 타일에 도착한 경우 
        TokenTile arriveTile = GameUtil.GetTileTokenFromMap(_char);

        //땅 속성값에 따라서 이벤트 발생 
        TokenEvent event1 = new TokenEvent(1,2);
        TokenEvent event2 = new TokenEvent(2, 2);
        TokenEvent event3 = new TokenEvent(3, 2);

        List<TokenEvent> eventList = new List<TokenEvent>() { event1, event2, event3 };
        if(m_PlayMaster.TempAdaptEvent)
        PlayerManager.GetInstance().OnTriggerEvent(eventList);
    }

    #endregion

    #region 이벤트 적용
    public void AdaptEvent(TokenEvent _event)
    {
        //룰북에서 월드 데이터상 등등 이벤트 적용하고
        //이벤트 발생 -> 플레이어에게 이벤트 선택창 띄움 -> 플레이어가 선택 -> 선택된 이벤트 룰북에 전달 -> 룰북에서 적용 -> 플레이어에게 적용된거알림

        //어차피 AI한텐 안하고 Player만 하니까
        PlayerManager.GetInstance().DoneAdaptEvent(); //마지막에 이벤트 적용끝났음을 플레이어에게 전달. 
    }
    #endregion

    #region 타일 액션
    public TokenAction[] RequestTileActions(TokenTile _tile)
    {
        //해당 타일을 가지고 가능한 액션을 뽑아줌. 
        List<TokenAction> ableList = new List<TokenAction>();
        //1.해당 타일의 pid를 확인
        int tileType = (int)_tile.tileType;
        //2. pid에 맞는 타일 데이터 가져옴
        TileTypeData tileData = MgMasterData.g_instance.GetTileData()[tileType]; //해당 타입의 타일데이터를 가져옴
        //3. 가능한 액션 토큰 배열을 순환
        for (int i = 0; i < tileData.AbleTileWork.Length; i++)
        {
            //4. 타일 데이터의 작업 가능한 액션 pid 리스트에 해당하는 타일액션들을 찾아서 반환
            TokenAction ableAction = m_tileActionDic[tileData.AbleTileWork[i]];
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
        TileActionType tileActionType = (TileActionType)_action.GetStat(TileActionStat.TileActionType);
        //tileActionType으로 행태 구별 
        //1 채집 
        if (tileActionType.Equals(TileActionType.Harvest))
        {
           // 서브 벨류 -> 캐피탈 인덱스
            Capital capitalCode = (Capital)_action.GetStat(TileActionStat.SubValue);
            int tempAgainValue = 50;
            PlayerCapitalData.g_instance.CalValue(capitalCode, tempAgainValue);
        }
        
        //3 재료 관련 -> 벨류값으로 변환, 합성
        if (tileActionType.Equals(TileActionType.CapitalChef))
        {
            CapitalAction doCode = (CapitalAction)_action.GetStat(TileActionStat.SubValue);
            //재료 변환 
            MgUI.GetInstance().ShowCapitalChefUI(doCode, _tile, _action);
        }
        else
        {
            MgUI.GetInstance().CancleLastUI();
        }
    }
    #endregion

    #region 캐릭 스텟 강화
    public void IntenseStat(TokenChar _targetChar, CharStat _targetStat)
    {
        _targetChar.CalStat(_targetStat, 100);
    }
    #endregion

    public void MixCapital(List<(Capital, int)> _resources)
    {
        PlayerCapitalData capitalData = PlayerCapitalData.g_instance;

        if (capitalData.IsEnough(_resources) == false)
        {
            Announcer.Instance.AnnounceState("합성 재료 부족");
            return;
        }
        capitalData.CalValue(_resources, false); //사용한만큼 감소 시키고
        (Capital, int) mixed = m_capitalRecipe.MixCapital(_resources);
        capitalData.CalValue(mixed.Item1, mixed.Item2); //얻은 만큼 추가 시키고 
    }

    public void ChangeCapital((Capital, int) _input, Capital _outCapital)
    {
        PlayerCapitalData capitalData = PlayerCapitalData.g_instance;
        if (capitalData.IsEnough(_input.Item1, _input.Item2) == false)
        {
            Announcer.Instance.AnnounceState("변환 재료 부족");
            return;
        }
        capitalData.CalValue(_input.Item1, -_input.Item2); //사용한만큼 감소 시키고
        (Capital, int) mixed = m_capitalRecipe.ChangeCapital(_input, _outCapital);
        capitalData.CalValue(mixed.Item1, mixed.Item2); //얻은 만큼 추가 시키고 
    }
}

public enum CapitalAction
{
    //ActionToken 3번의 재료 합성 타입에서 추가 분류
    MixCapital, ChageCapital
}
