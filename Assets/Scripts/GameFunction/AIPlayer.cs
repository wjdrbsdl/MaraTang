using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class AIPlayer : PlayerRule
{
    private List<TokenChar> m_npcList; //mgToken에서 생성된 npcList 참조
    private int m_turnNumber = 0;

    public void SetInitial()
    {
        m_npcList = MgToken.GetInstance().GetCharList();
        m_turnNumber = 0;
    }

    #region 인터페이스 플레이어가 할 행동
    public void PlayTurn()
    {
        // Ai가 캐릭터 조작하는 방법
        Announcer.Instance.AnnounceState("AI 플레이어 진행");
        //1. 순서대로 조작할 char를 뽑는다 - 이곳에서 액션 행동 카운트까지 남아있는 캐릭터만 받음.
  
        TokenChar turnChar =  SelectCharactor(); //행동할 녀석 뽑는걸로 행동 시작
        if(turnChar == null)
        {
            //---수행할 캐릭터를 뽑지 못했다면 ai 턴종료 선언
            EndTurn();
            return;
        }

        //2. 사용 가능한 액션 토큰이 있으면 세팅 - 공격, 이동 로직등 
        TokenAction nextAction = SelectActionLogic(turnChar);
        if(nextAction == null)
        {
            //만약 해당 캐릭이 수행가능한 액션이 없으면 다시 캐릭뽑기부터 시작
            SetNextNpcNum(); //차례 번호 넘기고
            PlayTurn();  //함수 재호출
            return;
        }

        //3. 액션이 있으면 액션 할당하고 
        turnChar.SetNextAction(nextAction);

        //4.게임 마스터에게 수행하도록 요청 
        GamePlayMaster.GetInstance().PlayCharAction(turnChar);

    }

    //5. 게임마스터로 부터 액션 수행 전달받았으면 다시 진행.
    public void DoneCharAction(TokenChar _char)
    {
        PlayTurn(); 
    }

    public void EndTurn()
    {
        m_turnNumber = 0; //캐릭터 카운팅 번호 초기화
        GamePlayMaster.GetInstance().EndPlayerTurn();
    }
    #endregion

    #region AI 로직
    //1. 차례번호 올리기
    private void SetNextNpcNum()
    {
        m_turnNumber += 1;
    }
    //2. 플레이할 캐릭터 뽑기
    private TokenChar SelectCharactor()
    {
        //Debug.Log(m_turnNumber + "캐릭터 체크");
        if (m_npcList.Count <= m_turnNumber)
        {
            return null;
        }
        TokenChar turnChar = m_npcList[m_turnNumber]; //이번 수행할 녀석으로 뽑은 다음     
        if (GameUtil.RemainActionCount(turnChar) == false || turnChar.IsPlayerChar())
        {
            //해당 캐릭터의 액션 횟수가 남은게 없거나 플레이어 조종아래있으면
            SetNextNpcNum(); //뽑은횟수 올리고 
            return SelectCharactor(); //다시 캐릭터뽑기 진행
        }
        //만약 남은 횟수가 있으면 해당 캐릭터로 로직 수행
        return turnChar;
        
        
    }
    //3. 캐릭터의 액션 고르기 (액션의 내용까지 채움)
    private TokenAction SelectActionLogic(TokenChar _char)
    {
        //해당 캐릭터가 수행할 수 있는 액션을 골라서 반환 
        /*
         * 공통적으로 모든 액션을 취할 때 사용이 가능한지 타겟까지 따져서 반환
         * 우선은 공격 부터 살피고 끝에 이동 - 추후 캐릭터의 성향, 상태, 액션타입의 추가로 어느 액션타입을 우선 살필지 로직 추가
         */

        _char.ClearNextAction();
 
        //캐릭이 자고있는 상태라면 그냥넘김.
        if (_char.GetState().Equals(CharState.Sleep))
        {
          //  Debug.Log(m_turnNumber + "캐릭 잠");
            return null;
        }
            

        //일단 타겟과 사거리를 구함 
        TokenChar  enemy = FindEnemy(_char);
        //타겟없으면 패스
        if (enemy == null)
        {
            //Debug.Log(m_turnNumber + "캐릭 적없음");
            return null;
        }
        
        //1.캐릭터가 가진 액션을 타입별로 나눈다
        // 액션 타입 종류만큼 배열을 생성
        List<TokenAction>[] actionTable = new List<TokenAction>[GameUtil.EnumLength(ActionType.Attack)];
        List<TokenAction> charActionList = _char.GetActionList();
        for (int i = 0; i < charActionList.Count; i++)
        {
            ActionType actionType = charActionList[i].GetActionType();
            //해당 타입에 이 액션을 넣을건데, 맨 처음넣는 거라 비어있으면 
            if (actionTable[(int)actionType] == null)
                actionTable[(int)actionType] = new List<TokenAction>(); //생성

            actionTable[(int)actionType].Add(charActionList[i]); //추가
        }
        //악마라면 부정부터 
        if (_char.GetCharType().Equals(CharType.Devil))
        {
            TokenAction wrongAction = SelectWrongFul(_char, actionTable[(int)ActionType.Wrongfulness], enemy);
            if(wrongAction != null)
            {
                return wrongAction;
            }
        }

        //2. 공격 가능한지 본다 
        TokenAction attactAction = SelectAttack(_char, actionTable[(int)ActionType.Attack], enemy);
        if (attactAction != null)
        {
          //  Debug.Log(m_turnNumber + " 공격 수행");
            return attactAction;
        }
            
            
        //마지막으로 이동 액션을 살펴서 반환
        return SelectMove(_char, actionTable[(int)ActionType.Move], GameUtil.GetTileTokenFromMap(enemy.GetMapIndex()));

    }

    #region 액션 선택 로직
    private TokenChar FindEnemy(TokenChar _char)
    {
        if (_char.GetTargetChar() != null)
            return _char.GetTargetChar();

        int tempEyesight = 5;
        for (int x = 1; x <= tempEyesight; x++)
        {
            //각 범위별로 타일을 가져와서 확인
            //ex : 범위가 10일때, 1~10 타일 모두가 아니라 각 범위별로 확인해서 시간 줄이기 
            List<TokenTile> inRangedTiles = GameUtil.GetTileTokenListInRange(x, _char.GetMapIndex(),x);
            for (int i = 0; i < inRangedTiles.Count; i++)
            {
                //타일 돌면서 내부 적 확인 
                TokenTile tile = inRangedTiles[i];
                for (int tileIndex = 0; tileIndex < tile.GetCharsInTile().Count; tileIndex++)
                {
                    //적 발견했으면 해당 포문 종료
                    TokenChar enemy = tile.GetCharsInTile()[tileIndex];
                    if (enemy != _char && enemy.IsPlayerChar()) //플레이어 차르인 경우 
                    {
                        return enemy;
                    }

                }
            }
        }
        
        
        
        return null;
    }

    private TokenAction SelectAttack(TokenChar _char, List<TokenAction> _attackList, TokenChar _enemy)
    {
        //만약 공격 리스트가 없다면 null 반환
        if (_attackList == null)
        {
        //    Debug.Log(m_turnNumber + " 공격스킬없음");
            return null;
        }

        int enemyDistance = GameUtil.GetMinRange(new TMapIndex(_char, _enemy));

        List<int> randomAt = GameUtil.GetRandomNum(_attackList.Count, _attackList.Count);
        for (int i = 0; i < randomAt.Count; i++)
        {
            //뽑은 공격리스트를 돌면서 가능한 공격 액션인지 - 사거리가 되는지 둘다 맞으면 타겟으로 삼고 이동 
            TokenAction attackAction = _attackList[randomAt[i]];
            string failMessage = "";
            //사용 가능한 액션인지 보고
            if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(_char, attackAction, ref failMessage) == false)
            {
            //    Debug.Log(m_turnNumber + " :"+failMessage);
                continue;
            }
          
            //해당 액션 사거리가 적에 닿는지 체크
            if (attackAction.GetFinalRange(_char) < enemyDistance)
                continue;

         
            //액션 내용을 채워서 반환
            attackAction.ClearTarget(); //타겟 리셋후
            attackAction.SetTargetCoordi(_enemy.GetMapIndex()); //적 위치를 타겟으로 하고 반환

            return attackAction; //이녀석을 반환
        }

        return null;
    }

    private TokenAction SelectWrongFul(TokenChar _devilChar, List<TokenAction> _wrongList, TokenBase _target)
    {
        //부정 액션 없으면 null
        if (_wrongList == null)
        {
            return null;
        }

        int enemyDistance = GameUtil.GetMinRange(new TMapIndex(_devilChar, _target));
        //사용할 부정 순서 섞기
        List<int> randomAt = GameUtil.GetRandomNum(_wrongList.Count, _wrongList.Count);
        for (int i = 0; i < randomAt.Count; i++)
        {
            TokenAction wrongAction = _wrongList[randomAt[i]];
            string failMessage = "";
            //사용 가능한 액션인지 보고
            if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(_devilChar, wrongAction, ref failMessage) == false)
            {
                //    Debug.Log(m_turnNumber + " :"+failMessage);
                continue;
            }

            //해당 액션 사거리가 적에 닿는지 체크
            if (wrongAction.GetFinalRange(_devilChar) < enemyDistance)
                continue;


            //액션 내용을 채워서 반환
            wrongAction.ClearTarget(); //타겟 리셋후
            wrongAction.SetTargetCoordi(_target.GetMapIndex()); //적 위치를 타겟으로 하고 반환

            return wrongAction; //이녀석을 반환
        }

        return null;
    }

    private TokenAction SelectMove(TokenChar _char, List<TokenAction> _moveList, TokenTile _enemy)
    {
        if (_moveList == null)
        {
         //   Debug.Log(m_turnNumber + " 이동스킬없음");
            return null;
        }

        //대상까지 사거리가 1이면 이동 안함 
        int enemyDistance = GameUtil.GetMinRange(new TMapIndex(_char, _enemy));
        if (enemyDistance == 1)
        {
            //Debug.Log(m_turnNumber + " 적과 거리가 1");
            return null;
        }

        //Debug.Log("타겟 까지 이동");
        int tempStopDistance = 1; //목적지 까지 멈추는 거리 0 이면 해당 타일 위로 
        TokenTile targetTile = MgToken.GetInstance().GetMaps()[_enemy.GetXIndex(), _enemy.GetYIndex()];  //목적지 타일 

        //여러가지 이동 수단중에 뽑아서 진행 
        List<int> randomMove = GameUtil.GetRandomNum(_moveList.Count, _moveList.Count);
        for (int i = 0; i < randomMove.Count; i++)
        {
            //뽑은 공격리스트를 돌면서 가능한 공격 액션인지 - 사거리가 되는지 둘다 맞으면 타겟으로 삼고 이동 
            TokenAction moveAction = _moveList[randomMove[i]];
            string failMessage = "";
            //사용 가능한 액션인지 보고
            if (GamePlayMaster.g_instance.RuleBook.IsAbleAction(_char, moveAction, ref failMessage) == false)
            {
            //    Debug.Log(m_turnNumber + " :" + failMessage);
                continue;
            }

            //타겟 까지 해당 이동액션으로 이동해봄
            int[] coordi = RouteFindLogic(_char, moveAction, targetTile, tempStopDistance); 
            if(_char.GetXIndex() != coordi[0] || _char.GetYIndex() != coordi[1])
            {
                //만약 이동 후 좌표가 지금과 다른곳이라면 해당 위치로 좌표 찍고
                moveAction.SetTargetCoordi(coordi); //적 위치를 타겟으로 하고 반환
                return moveAction;
            }
        }

        //사용할 이동기가 없으면
        return null;

      
    }

    private int[] RouteFindLogic(TokenChar _char, TokenAction _moveAction, TokenTile _target, int stopDistance = 0)
    {
        //현재 케릭이, 타겟까지 이동할 루트로 tokenTile을 찾아, 액션 토큰에 삽입. 

        int tempMoveCount = 1; //이동횟수 겟 함수
        TMapIndex mapInfoes = new TMapIndex(_char, _target);
        
        //캐릭터가 이동가능 거리를 기반으로 한칸씩 로직을 받음 
        for (int i = 1; i <= tempMoveCount; i++)
        {
            //1. 계속 찾을지 체크
            if(GameUtil.GetMinRange(mapInfoes) <= stopDistance)//현재 위치에서 목적지까지 사거리가 멈춰야하는 거리 이내라면 루트 찾기 종료 
            {
                break;
            }

            //2. 최단경로로 가능한 방향을 뽑고
            TileDirection nextDirect = GameUtil.GetNextTileToGoal(mapInfoes);

            //3. 현재 좌표에서 해당 경로로 이동시 상대 좌표를 산출
            int[] gapValue = GameUtil.GetGapCoordiFromDirect(mapInfoes.curY, nextDirect);
            //4. 현재 좌표에 상대 좌표를 더해서 이동후 좌표값 계산
            mapInfoes.curX += gapValue[0];
            mapInfoes.curY += gapValue[1];
            //5. 이동후 좌표가 이동불가 혹은 맵정보에 없는 좌표면 넘김
            if(GameUtil.IsThereMap(mapInfoes.curX, mapInfoes.curY)== false)
            {
                //만약 없는 맵 위치를 찍었으면 돌리고 다시 방향 뽑기로 진행 (AI는 moveCount 기회만큼 올바른 길을 찾을 기회가 있는것) 
                mapInfoes.curX -= gapValue[0];
                mapInfoes.curY -= gapValue[1];
                continue;
            }
        }
        return new int[]{ mapInfoes.curX, mapInfoes.curY };
    }
    #endregion

    #endregion

    public CharTurnStep GetCurPlayStep()
    {
        return CharTurnStep.ChooseChar;
    }
}


//리얼 플레이어와, ai플레이어가 모두 갖춰야할 기능들. 
public interface PlayerRule
{
    //액션 수행을 하도록 명 받았을 때
    public void PlayTurn();

    //액션이 완료됨을 콜백 받았을 때
    public void DoneCharAction(TokenChar _char);

    public void EndTurn();

    public CharTurnStep GetCurPlayStep();
}