using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class AIPlayer : PlayerRule
{
    public enum Priority
    {
        Enemy, Mineral
    }
    private List<TokenChar> m_npcList;
    private int m_turnNumber = 0;

    public void SetInitial()
    {
        m_npcList = MgToken.g_instance.GetNpcPlayerList();
        m_turnNumber = 0;
    }

    #region 인터페이스 플레이어가 할 행동
    public void PlayTurn()
    {
        // Ai가 캐릭터 조작하는 방법

        GamePlayMaster.g_instance.AnnounceState("AI플레이어 진행");
        //1. 순서대로 조작할 char를 뽑는다 - 이곳에서 액션 행동 카운트까지 남아있는 캐릭터만 받음.
        TokenChar turnChar =  SelectCharactor(); //행동할 녀석 뽑는걸로 행동 시작
        if(turnChar == null)
        {
            //---1단계에서 수행할 캐릭터를 뽑지 못했다면 ai 턴종료 선언
            EndTurn();
            m_turnNumber = 0; //차례 번호 초기화
            return;
        }

        //2. 사용 가능한 액션 토큰이 있으면 세팅 - 공격, 이동 로직등 
        SelectActionLogic(turnChar);
        if(turnChar.GetNextActionList() == null)
        {
            //만약 해당 캐릭이 수행가능한 액션이 없으면 다시 캐릭뽑기부터 시작
            SetNextNpcNum(); //차례 번호 넘기고
            PlayTurn();  //함수 재호출
            return;
        }

        //3. 해당 액션 수행을 위한 내용을 채움 
        FillContentLogic(turnChar);
        if(turnChar.GetNextActionToken().GetTargetList().Count == 0)
        {
            //만약 해당 캐릭이 해당 액션을 수행 할 수 없는 상황이라면 다음 캐릭터로 넘기진 말고 다시 진행
            //FillContent 부분에서 해당 액션을 선택하지 못하도록 처리 필요. 
            PlayTurn();
            return;
        }

        //4.내용까지 채워졌다면 겜마에게 수행하도록 요청 
        GamePlayMaster.g_instance.PlayCharAction(turnChar);

    }

    //5. 게임마스터로 부터 액션 수행 전달받았으면 다시 진행.
    public void DoneCharAction(TokenChar _char)
    {
        PlayTurn(); 
    }

    public void EndTurn()
    {
        GamePlayMaster.g_instance.EndPlayerTurn();
    }
    #endregion

    #region 액션 수행 로직
    //1. 차례번호 올리기
    private void SetNextNpcNum()
    {
        m_turnNumber += 1;
    }
    //2. 플레이할 캐릭터 뽑기
    private TokenChar SelectCharactor()
    {
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
    //3. 캐릭터의 액션 고르기
    private void SelectActionLogic(TokenChar _char)
    {
        //캐릭터 상태 그밖에 조건등으로 현재 캐릭터가 취해야할 액션을 선택하는 로직
        _char.ClearNextActionList();

        int tempEyeSight = 3; //캐릭터의 시야거리
        List<TokenTile> inRangedTiles = GameUtil.GetTileTokenListInRange(tempEyeSight, _char.GetXIndex(), _char.GetYIndex());
        bool isEnemyFind = false;
        for (int i = 0; i < inRangedTiles.Count; i++)
        {
            //타일 돌면서 내부 적 확인 
            TokenTile tile = inRangedTiles[i];
            for (int tileIndex = 0; tileIndex < tile.GetCharsInTile().Count; tileIndex++)
            {
                //적 발견했으면 해당 포문 종료
                TokenChar enemy = tile.GetCharsInTile()[tileIndex];
                _char.SetTarget(enemy);
                Debug.Log("적발 견" + enemy.GetXIndex() + ", " + enemy.GetYIndex());
                isEnemyFind = true;
                if (isEnemyFind)
                    break;
                
            }
            if (isEnemyFind)
                break;
        }
        //Tokenchar 가 사거리 이내 캐릭터들을 다 파악해놓는다? 
        //최적화 부분으로서 자신의 주변 생태를 어떻게 파악할지는 나중에 생각해보기로 하고, 일단 주변반경 ~5 정도 까지 타일 갯수 파악하는걸로
        //토큰 리스트
        //캐릭 리스트를 해놓자. 

        if (isEnemyFind)
        {
            //1 찾은 적이 사거리 이내라면 공격 액션 아니라면 이동액션 목적지는 그대로 
            //대적 행위를 하고 
            
            TMapIndex tMapIndex = new TMapIndex(_char, _char.GetTarget());
            int enemyRange = GameUtil.GetMinRange(tMapIndex);
            Debug.Log("몬스터 까지 거리 " + enemyRange);
            int charRange = 1; //일단 캐릭터의 공격 사거리를 1로 지정
            if (enemyRange <= charRange)
            {
                _char.SetNextAction(_char.GetActionList()[1]); //사거리이내라면 공격으로
                return;
            }
            else
            {
                _char.SetNextAction(_char.GetActionList()[0]); //아니라면 이동으로
            }
        }
        else
        {
            //그밖에 행위 일단 이동으로 
            _char.SetNextAction(_char.GetActionList()[0]);
        }



    }
    //4. 캐릭터의 액션 내용 채우기
    private void FillContentLogic(TokenChar _char)
    {
        //선택한 액션에 따라 타겟을 설정하는 부분 
        //pid 설명에 따라 타겟을 몇개 고를지 - 몇번 타겟이 몇번타겟에게 어떤행동을 하는지등 테이블로 정의해놓기 

        TokenAction choiceAction = _char.GetNextActionToken(); //복합 액션이 가능할진 모르지만 일단 첫번째, 그럼 행동 하나로 하자. 
        ActionType actionType = choiceAction.GetActionType();
        choiceAction.ClearTarget(); //해당 액션의 내용을 삭제 (타겟리스트)
        if (actionType == ActionType.Attack)
        {
            //이동 액션을 제외한 모든 액션은 해당 액션을 취하기 위한 사거리 이내의 적을 발견한 상태일것. 
        //    Debug.Log("어택 내용 채운다");
            choiceAction.AddTarget(_char.GetTarget());
        }
        else if (actionType == ActionType.Move)
        {
        //    Debug.Log("이동 내용 채운다");
            int tempStopDistance = 0; //목적지 까지 멈추는 거리 0 이면 해당 타일 위로 
            TokenTile targetTile = new TokenTile(); //목적지 타일 
            TokenBase charTarget = _char.GetTarget(); //캐릭터가 쫓고있는 타겟이 있는지 확인
            //1. 타겟이 있는데, 캐릭터 즉 공격 대상이라면, 공격 사거리만큼 스탑 디스턴스 조정
            if (charTarget != null && charTarget.GetTokenType() == TokenType.Char)
            {
                int tempAttackRange = 1; //임시 현재 타겟의 사거리를 1로 조정
                tempStopDistance = tempAttackRange; //멈출거리 1로 조정
                targetTile = MgToken.g_instance.GetMaps()[charTarget.GetXIndex(), charTarget.GetYIndex()]; //해당 타겟이 있는 타일을 목적지로 설정
                
            }
            //2. 따로 타겟이 없으면 범위 내 타일중 이동할 곳 랜덤으로 뽑기
            else
            {
                int tempMoveRange = 2;
                // 사거리 내부 안의 타일값을 int[]좌표로 가져온후, 해당 인덱스의 tileToken으로 반환
                List<TokenTile> inRangedTiles = GameUtil.GetTileTokenListInRange(tempMoveRange, _char.GetXIndex(), _char.GetYIndex());
                int ran = Random.Range(0, inRangedTiles.Count);
                targetTile = inRangedTiles[ran]; //이동할 타겟을 구했으면, 해당 타겟 까지 가기 위한 중간 타겟을 산출
            }
            RouteFindLogic(_char, choiceAction, targetTile, tempStopDistance); //타겟 까지의 루트 타일을 이동 횟수만큼 담아놓음. 
            _char.SetTarget(null); //타겟을 쫓긴 쫓되 이동을 했다면, 이동후 다시 공격액션에 따라 타겟을 찾으므로 일단 타겟 초기화. 
        }
    }
    //5. 캐릭터가 이동 길 찾기
    private void RouteFindLogic(TokenChar _char, TokenAction _actionToken, TokenTile _target, int stopDistance = 0)
    {
        //현재 케릭이, 타겟까지 이동할 루트로 tokenTile을 찾아, 액션 토큰에 삽입. 

        int tempMoveCount = 3; //이동횟수 겟 함수
   
        TokenTile[,] maps = MgToken.g_instance.GetMaps();
        TMapIndex mapInfoes = new TMapIndex(_char, _target);
        
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
            int[] gapValue = GameUtil.GetTileFromDirect(mapInfoes.curY, nextDirect);
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
            //6. 올바른 좌표라면 이동확정 하고 해당 토큰타일을 추가 
            //메모리상으로 새로 객체 생성할필요없이 그냥 maps에 있는 클래스를 넣어도 되나? 어차피 참조중인가?
            TokenTile targetMap = maps[mapInfoes.curX, mapInfoes.curY];
            _actionToken.AddTarget(targetMap);;
        }
    }
    #endregion
}


//리얼 플레이어와, ai플레이어가 모두 갖춰야할 기능들. 
public interface PlayerRule
{
    //액션 수행을 하도록 명 받았을 때
    public void PlayTurn();

    //액션이 완료됨을 콜백 받았을 때
    public void DoneCharAction(TokenChar _char);

    public void EndTurn();
}