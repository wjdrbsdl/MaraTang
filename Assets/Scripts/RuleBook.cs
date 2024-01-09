using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBook
{
    public static GamePlayMaster m_PlayMaster;
    private RouteDisplay m_routeDisplayTool = new();
  
    #region 액션 수행 절차
    public void PlayAction(TokenChar _char)
    {
        //룰북에서 실행단계
        //0. 해당 액션이 몇단계일지 넘긴다
        //1. 해당 액션을 수행하기 위한 예약을 건다
        //2. 예약된 녀석은 바로 실행이 된다.
        //3. 룰북에선 액션을 계속 예약을 건다. 
        //4. 진행하던 코루틴이 종료되면 마지막 스텝인지 체크하여 DoneAction을 호출한다. 

        TokenAction action = _char.GetNextActionToken();
        ActionType actionType = action.GetActionType();
        int curStep = 0;
        if (actionType == ActionType.Attack)
        {
            Debug.Log("어택 내용 수행한다");
            //파악해둔 캐릭리스트 들 순서대로 혹은 그 중에 랜덤으로 뽑기
            List<TokenChar> charList = action.GetTargetList().ConvertAll(tokenBase => (TokenChar)tokenBase);//모든 요소를 Char로 전환
            m_PlayMaster.RservereInfo(_char, 0);
            for (int i = 0; i < charList.Count; i++)
            {
                //1. _char가 공격자 _charList[i]가 피해자로 공격 규칙 적용 
           
            }
        }
        else if (actionType == ActionType.Move)
        {
            //Debug.Log("이동 내용 수행한다"+_char.charNum);
            //타겟리스트로 뽑은 타일로 실제 이동이 일어나는 부분 
            
            List<TokenTile> targetTile = action.GetTargetList().ConvertAll(tokenBase => (TokenTile)tokenBase);//모든 요소를 Tile로 전환
            ShowRouteNumber(targetTile);
            m_PlayMaster.RservereInfo(_char, targetTile.Count);
            int tempTiming = 2;
            for (int i = 0; i < targetTile.Count; i++)
            {
                curStep += 1;
                m_PlayMaster.ReservateMove(_char, targetTile[i], tempTiming, curStep, Migrate);
                
            }
         
        }
        m_PlayMaster.DoneReserve(curStep); //아무 예약도 되지 않은 상황을 대비 아니지 그냥 DoneReserve 호출이 낫겠다.
    }

    public static void Migrate(TokenChar _char, TokenTile _targetTile)
    {
        //해당 타일로 해당 캐릭터를 이주 시키기 
        MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()].Immigrate(_char); //이사 보내고
        _targetTile.Migrate(_char); //이사 넣고 
    }

    public void ShowRouteNumber(List<TokenTile> _tiles)
    {
        m_routeDisplayTool.ShowRoute(_tiles);
    }

    public void OffRouteNumber()
    {
        m_routeDisplayTool.ResetPreRoute();
    }
    #endregion

    #region 조건 체크 
    public bool IsInRangeTarget(TokenChar _char, TokenAction _action, TokenBase _target)
    {
        TMapIndex mapIndex = new TMapIndex(_char, _target);
        int targetRange = GameUtil.GetMinRange(mapIndex);

        //Debug.Log(_char.GetXIndex() + "," + _char.GetYIndex() + "에서 " + _target.GetXIndex() + "," + _target.GetYIndex() + "거리는 " + targetRange);
        if (_action.GetStat(ActionStat.Range) < targetRange)
           return false;

        return true;
    }

    public bool IsMatchTargetType(TokenAction _action, TokenBase _target)
    {
        if (_action.GetTargetType().Equals(_target.GetTokenType()) == false)
            return false;

        return true;
    }

    public bool CheckUsableToken(TokenChar _char, TokenAction _action)
    {
        if(_char.GetActionCount()<= 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckActionContent(TokenChar _char, TokenAction _action)
    {
        if (_action.GetTargetList().Count == 0)
        {
            m_PlayMaster.AnnounceState("타겟이 부정확");
            return false;
        }
            

        return true;
    }
    #endregion

    #region 이벤트 발생 

    public void OnTileArrive(TokenChar _char)
    {
        //플레이어 매인 캐릭터가 새로운 타일에 도착한 경우 
        TokenTile arriveTile = GameUtil.GetTileTokenFromMap(_char);

        //땅 속성값에 따라서 이벤트 발생 
        TokenEvent event1 = new TokenEvent(1,2);
        TokenEvent event2 = new TokenEvent(2, 2);
        TokenEvent event3 = new TokenEvent(3, 2);

        List<TokenEvent> eventList = new List<TokenEvent>() { event1, event2, event3 };
        if(m_PlayMaster.AdaptEvent)
        PlayerManager.g_instance.OnTriggerEvent(eventList);
    }

    #endregion

    #region 이벤트 적용
    public void AdaptEvent(TokenEvent _event)
    {
        //룰북에서 월드 데이터상 등등 이벤트 적용하고
        // 이벤트 발생 -> 플레이어에게 이벤트 선택창 띄움 -> 플레이어가 선택 -> 선택된 이벤트 룰북에 전달 -> 룰북에서 적용 -> 플레이어에게 적용된거알림


        PlayerManager.g_instance.OnAdaptEvent(); //마지막에 이벤트 적용끝났음을 플레이어에게 전달. 
    }
    #endregion
}

