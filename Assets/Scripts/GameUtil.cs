using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
    RightUp, Right, RightDown, LeftDown, Left, LeftUp
}

public static class GameUtil 
{
    #region 타일 계산
    public static List<TokenTile> GetMinRoute(int _range, int _centerX, int _centerY, int _targetX, int _targetY, int _stopDistance = 0)
    {
        //현재 점에서 목표점 까지의 최소 루트를 위해 행할 방향 수 
        List<TokenTile> routeTileList = new List<TokenTile>();

        //현재 케릭이, 타겟까지 이동할 루트로 tokenTile을 찾아, 액션 토큰에 삽입. 

        int tempMoveCount = _range; //이동 루트 거리 
        //출발지
        int x = _centerX;
        int y = _centerY;
        //목적지
        int toX = _targetX;
        int toY = _targetY; 
        //목적지 까지 접근 거리
        int stopDistance = _stopDistance;

        TokenTile[,] maps = MgToken.g_instance.GetMaps();
        TMapIndex mapInfoes = new TMapIndex(x, y, toX, toY);

        for (int i = 1; i <= tempMoveCount; i++)
        {
            //1. 계속 찾을지 체크
            if (GameUtil.GetMinRange(mapInfoes) <= stopDistance)//현재 위치에서 목적지까지 사거리가 멈춰야하는 거리 이내라면 루트 찾기 종료 
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
            if (GameUtil.IsThereMap(mapInfoes.curX, mapInfoes.curY) == false)
            {
                //만약 없는 맵 위치를 찍었으면 돌리고 다시 방향 뽑기로 진행 (AI는 moveCount 기회만큼 올바른 길을 찾을 기회가 있는것) 
                mapInfoes.curX -= gapValue[0];
                mapInfoes.curY -= gapValue[1];
                continue;
            }
            //6. 올바른 좌표라면 이동확정 하고 해당 토큰타일을 추가 
            //메모리상으로 새로 객체 생성할필요없이 그냥 maps에 있는 클래스를 넣어도 되나? 어차피 참조중인가?
            TokenTile targetMap = maps[mapInfoes.curX, mapInfoes.curY];
            routeTileList.Add(targetMap); ;
        }



        return routeTileList; 
    }

    public static List<int[]> GetTileIdxListInRange(int _range, int _centerX, int _centerY)
    {
        //   _1 _2
        //  /6    |3  왼쪽을 스타트로 해당 사거리 _range 단계만큼 TileDirection enum 방향으로 진행하면서 상대적 좌표 도출
        //   |5  /4

        List<int[]> rangedTile = new List<int[]>();

        int[] startPoint = {_centerX , _centerY}; //기준이되는 시작점을 센터로 잡음
        //string resultLog = "";
        //한칸씩 주변을 돌면서 기록
        for(int range = 1; range <= _range; range++)
        {
            //주변을 도는 6 방향 - TileDirection으로 매칭 -> RightUp방향으로 먼저 진행하므로, -1(살피려는 범위의 사거리)로 시작점을 조정
            startPoint[0] = _centerX - range;
            for (int x = 0; x <= 5; x++)
            {
                //해당 방향으로 나아가는 횟수는 현재 살펴보는 range
                for (int i = 1; i <= range; i++) //6방향으로 돔
                {
                   // Debug.Log((TileDirection)x + "방향으로 " + i + "번 진행");
                   //1. 타일에서 특정방향으로 갈때 상대 좌표값 도출
                    int[] newTile = GetTileFromDirect(startPoint[1], (TileDirection)x); 

                    //2. 도출된 상대좌표에 기본 좌표값을 더하면 이동한 절대 좌표가 나옴. 
                    newTile[0] += startPoint[0];
                    newTile[1] += startPoint[1];

                    //3. 시작점을 이동한 절대좌표로 갱신
                    startPoint[0] = newTile[0];
                    startPoint[1] = newTile[1];
                    if (IsThereMap(newTile[0], newTile[1])) //그 절대좌표가 존재하는 좌표라면 값 추가
                    rangedTile.Add(newTile); //뽑힌건 넣고 
                    //resultLog += (startPoint[0] + _centerX).ToString() + "," + (startPoint[1] + _centerY).ToString() + "/";
                }
                //resultLog += "\n";
            }
            //6방향 모두 돌았으면 다시 Left 지점으로 돌아옴 여기서 한칸더 주변 범위를 돌기 때문에 왼쪽으로 이동
        
        }
        
        //Debug.Log("찾은 타일 갯수" + rangedTile.Count);

        return rangedTile;
    }

    public static List<TokenTile> GetTileTokenListInRange(int _range, int _centerX, int _centerY)
    {
       return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY).ConvertAll(new System.Converter<int[], TokenTile>(GetTileTokenFromMap)); // 사거리 내부 안의 타일 가져오기
    }

    public static List<ObjectTokenBase> GetTokenObjectInRange(int _range, int _centerX, int _centerY)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY).ConvertAll(new System.Converter<int[], ObjectTokenBase>(GetTokenObjectFromMap)); // 사거리 내부 안의 타일 가져오기
    }

    public static int[] GetTileFromDirect(int _centerY, TileDirection _direction)
    {
        int[] nextTileIndex = new int[2];
        int xPlus = 0;
        int yPlus = 0;
        //위아래 이동시 보정이 필요한 경우인지
        bool isEven = _centerY % 2 == 0;
        
        switch (_direction)
        {
            case TileDirection.RightUp:
                yPlus = 1;
                if (!isEven)
                    xPlus = 1;
                break;
            case TileDirection.Right:
                xPlus = 1;
                break;
            case TileDirection.RightDown:
                yPlus = -1;
                if (!isEven)
                    xPlus = 1;
                break;
            case TileDirection.LeftDown:
                yPlus = -1;
                if (isEven)
                    xPlus = -1;
                break;
            case TileDirection.Left:
                xPlus = -1;
                break;
            case TileDirection.LeftUp:
                yPlus = 1;
                if (isEven)
                    xPlus = -1;
                break;
        }
        nextTileIndex[0] += xPlus;
        nextTileIndex[1] += yPlus;

        return nextTileIndex;
    }

    //목적지까지 최단 타일 개수
    public static int GetMinRange(TMapIndex _tMapIndex)
    {
       
        
        int minY = Mathf.Abs(_tMapIndex.curY - _tMapIndex.toY); //최소 움직여야하는 Y 값 
        
        int gapX = Mathf.Abs(_tMapIndex.curX - _tMapIndex.toX); //같은 y축이 되었을 때 x 이동 값

        //같은 높이라면 gapX 만큼 이동하면됨
        if (minY == 0)
            return gapX;

        //다른 높이라면 최소 높낮이를 위한 이동 중, to좌표로 진행할텐데, 이때 해당방향으로 이동해도 gapX가 줄어들지 않으면 추가 이동이 필요
        if (gapX * 2 <= minY)
            return minY; //홀짝 높이 조건없이 최소 높이차이가 gapX보다 2이상이면, minY만큼 이동하면 목적지 도달가능. 

        int moveX = minY / 2; //최소 높이 나누기 2만큼은 목적지로 이동이 가능. 
        bool odd = (_tMapIndex.curY % 2 == 1); //현재 높이가 홀수 층인가
        bool directRight = 0 < (_tMapIndex.toX - _tMapIndex.curX); //to 방향이 오른쪽으로 인가 왼쪽인가
        //추가로 홀수 번째이동시 1 이동 가능여부 판단. 
        bool haveRemain = (minY % 2 == 1); //높낮이 1칸이 남은 경우
        if (haveRemain)
        {
            if (odd && directRight)
                moveX += 1;
            else if (!odd && !directRight)
                moveX += 1;
        }

        //moveX 는 해당 높이까지 우측, 좌측 대각선으로 진행했을 때의 x 변화값을 따짐 
        int movedX = _tMapIndex.curX;
        int minRange = minY; //최소 높낮이 이동해야하고 
        if (directRight)
        {
            movedX += moveX; //해당 높이까지 최대한 오른편으로 간거 이때 
            if (movedX < _tMapIndex.toX) //아직 가야할 X가 더 오른편에 있다면
                minRange += (_tMapIndex.toX - movedX);

        }
        else
        {
            movedX -= moveX;
            if (_tMapIndex.toX < movedX ) //아직 가야할 X가 더 왼편에 있다면
                minRange += (movedX - _tMapIndex.toX); //움직인 곳에서 더 왼쪽으로 이동해야할 거리를 구해서 추가
        }
            

        return minRange;
    }

    //목적지 타일 까지 최근접으로 가는 조건아래 갈수 있는 타일 포지션 반환
    public static TileDirection GetNextTileToGoal(TMapIndex _tMapIndex)
    {
        //목적지까지 최단경로로 갈때 현재 타일에서 갈수 있는 방향을 반환
        TileDirection direction = TileDirection.RightUp;

        if ((_tMapIndex.curY - _tMapIndex.toY) == 0)
        {
            //같은 높이에 있으면 최단경로 방향은 좌우 중 하나
            if ((_tMapIndex.toX - _tMapIndex.curX) > 0)
                return TileDirection.Right;
            else
                return TileDirection.Left;
        }

        //1. 현재 점에서, to 높이까지 우상으로 질렀을 때의 x 값
        int lineX = (int)((_tMapIndex.toY - _tMapIndex.curY) *0.5f); //높이 / 2. 즉 높이가 2오를때마다 기본적으로 x가 1 상승. 
        if (_tMapIndex.curY % 2 == 1 && (_tMapIndex.toY - _tMapIndex.curY)%2 == 1)
        {
            //만약 시작과 끝의 라인이 다르다면, 추가로 +1 해야하는 구간인지 알아야함.
            //높이가 홀수에서 짝수로 갈때 x가 1이 오름. 
            lineX += 1;
        }
        lineX += _tMapIndex.curX; //출발 x를 더해서 절대 x좌표 산출. 

        //2. 대각선 라인의 x를 기준 toX의 위치가 어디인지에 따라 이동가능한 방향 결정
        int gap = _tMapIndex.toX - lineX;
        int directX = 0; //최종 방향 표시할 변수들
        int directY = 0; //최종 방향 표시할 변수들
        int randomIndex = Random.Range(0,2); //0,1 둘중하나 뽑기
        if (gap < 0)
        {
            //toX가 왼편이면 갈수있는 방향은 좌상, 우상중 하나
            if(randomIndex == 0)
            {
                //임의로 0인경우 좌상으로 진행
                directX = -1;
                directY = 1;
            }
            else
            {
                directX = 1;
                directY = 1;
            }
        }
        else if(gap == 0)
        {
            //대각 라인에 toX가 겹친다면 무조건 우상해야 최단경로가 가능 
            directX = 1;
            directY = 1;
        }
        else
        {
            //toX가 오른편이면 갈수있는 방향은 우상, 오른편 중 하나
            if (randomIndex == 0)
            {
                //임의로 0인경우 우상으로 진행
                directX = 1;
                directY = 1;
            }
            else
            {
                directX = 1;
            }
        }

        //3. 다시 방향을 본래 방향대로 반전
        if((_tMapIndex.toX - _tMapIndex.curX) < 0)
        {
            //가야할 방향이 왼편이었다면
            directX *= -1; //x 방향을 꺾고
        }
        if ((_tMapIndex.toY - _tMapIndex.curY) < 0)
        {
            //가야할 방향이 아래였다면
            directY *= -1; //y 방향도 꺾고
        }

        //5. 최종 direct 값에 따라 방향 도출 
        if(directX < 0)
        {
            if (directY > 0)
            {
                direction = TileDirection.LeftUp;
            }
            else if(directY < 0)
            {
                direction = TileDirection.LeftDown;
            }
            else if(directY == 0)
            {
                direction = TileDirection.Left;
            }
        }
        else if (0 < directX)
        {
            if (directY > 0)
            {
                direction = TileDirection.RightUp;
            }
            else if (directY < 0)
            {
                direction = TileDirection.RightDown;
            }
            else if (directY == 0)
            {
                direction = TileDirection.Right;
            }
        }

        return direction;
    }

    private static void ReverseToPlus(int _centX, int _centY, int _targetX, int _targetY, ref int moveX, ref int moveY)
    {
        //현재 포인트 기준 목표점을 양의 방향으로 반전시키기 
        moveX = Mathf.Abs(_centX - _targetX);
        moveY = Mathf.Abs(_centY - _targetY);

        bool isAnotherLine = (_centY + _targetY) % 2 == 1; //둘의 높이합이 홀수면 둘은 다른 행
        if (isAnotherLine && 0 < (_centX - _targetX)) //다른라인인데 타겟이 왼쪽 측면에 있다면, 절대화 할때 x 좌표값을-1 해줘야함. 
            moveX -= 1;
    }
    #endregion

    public static void CharacterMove(TokenChar _char, TokenTile _target, int range = 3)
    {
        int x= _char.GetXIndex(), y=_char.GetYIndex(), toX= _target.GetXIndex(), toY=_target.GetYIndex(), r=range; //필요한 변수값들 임시

        TMapIndex toTest = new TMapIndex(x, y, toX, toY);
        for (int i = 0; i < r; i++)
        {
            TileDirection nextDirect = GameUtil.GetNextTileToGoal(toTest); //최단경로로 갈수 있는 방향 뽑고

            //그리고 실제로 그 방향으로 이동한 후의 좌표를 도출
            int[] gapValue = GameUtil.GetTileFromDirect(toTest.curY, nextDirect);
            //도출된 좌표는 상대좌표로 현재 좌표에 더해야 이동한 좌표가 됨
            toTest.curX += gapValue[0];
            toTest.curY += gapValue[1];
            Debug.Log("가야할 방향" + nextDirect + "이동후 좌표 " + toTest.curX + ", " + toTest.curY);
            //좌표를 수정해서 다시 진행 

        }
    }

    /*
    빌드세팅에 디파인 옵션 넣고 가져오는법
        PlayerSettings.SetScriptingDefineSymbolsForGroup(ebuildTarget, m_Defines.ToString());
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
    */

    public static bool IsThereMap(int _x, int _y)
    {
        int maxX = MgToken.g_instance.GetMaps().GetLength(0);
        int maxY = MgToken.g_instance.GetMaps().GetLength(1);
        
        if (_x < 0 || maxX <= _x)
            return false;
        if (_y < 0 || maxY <= _y)
            return false;
        //추후 사거리 이내의 맵이더라도 맵 특성상 없다고 체크되어있으면 false반환

        return true;
    }

    public static bool RemainActionCount(TokenChar _char)
    {
        bool remain = true;

        if (_char.GetActionCount() == 0)
            remain = false;

        return remain;
    }

    public static TokenTile GetTileTokenFromMap(int[] _index)
    {
        return MgToken.g_instance.GetMaps()[_index[0], _index[1]];
    }

    public static TokenTile GetTileTokenFromMap(TokenChar _char)
    {
        return MgToken.g_instance.GetMaps()[_char.GetXIndex(), _char.GetYIndex()];
    }

    public static ObjectTokenBase GetTokenObjectFromMap(int[] _index)
    {
        return MgToken.g_instance.GetMaps()[_index[0], _index[1]].GetObject();
    }

    #region 테이블 enum 변경기
    static string[]  originEnumString = { "Plus", "cheicken", "Scoop", "Vio" }; //기존 테이블에서 읽었다고 가정
    static int[] originEnumValues = { 10, 20, 30, 25 };
    static int[] changeEnumValues; //새로 생성된 객체 속성값 
    static int[] newIndex;
    enum NewMenu
    {
        Valong, Vio, Sionic, Plus
    }

    public static void LoadTable()
    {
        //저장된 값을 불러오는 부분 
        string[] loadEnumStr = LoadNameValue(); //불러왔을 때의 해당 enum key의 string값들
                                                //어떤 토큰 불러왔냐 따라서 타입 새로 생성 했다 치고 
                                                //기존 테이블에서 현재 테이블에 맞는 색인표를 만듬 현재 Vio가, 기존 테이블에선 몇번째인지, 없으면 maxValue로 처리
        newIndex = ToStringFromEnum(NewMenu.Plus, loadEnumStr, originEnumValues);

    }

    private static string[] LoadNameValue()
    {
        //enum name값들을 적어놓은 테이블에서 기존enum값들 가져오던
        //해당 저장 table 값에서 가져오던 어디서든 가져와서 string[] 로 생성해서 반환
        return originEnumString;
    }

    //해당 주석의 변경 인덱스를 짜기 - 아직 미완
    private static int[] ToStringFromEnum(System.Enum _curType, string[] _origin, int[] _oriValue)
    {
        Debug.Log(_curType + " , " + _curType.GetType().ToString());
        //enum 값중 하나를 가지고 해당 enum 모든 값 파악
        string[] curNames = System.Enum.GetNames(_curType.GetType());
        //기존 테이블에서 현재 테이블에 맞는 색인표를 만듬
        int[] loadIndex = new int[curNames.Length];
        for (int curIdx = 0; curIdx < curNames.Length; curIdx++)
        {
            //Debug.Log(curNames[curIdx]);
            string curId = curNames[curIdx];
            loadIndex[curIdx] = int.MaxValue;
            //bool isFind = false;
            for (int originIdx = 0; originIdx < _origin.Length; originIdx++)
            {
                //해당 아이디를 기존에서 갖고 있는지 체크
                string originId = _origin[originIdx];
                if (originId.Equals(curId))
                {
                    //현재 enumIdx 대로 기존 값을 파싱
                    loadIndex[curIdx] = _oriValue[originIdx];
                    //isFind = true;
                    break; //이번 루프는 끝
                }
            }
            //if(isFind == false)
            //Debug.Log(curId + "는 기존 에 없는 enum 값");
        }

        return loadIndex;
    }
    #endregion
}

public struct TMapIndex
{
    public int curX;
    public int curY;
    public int toX;
    public int toY;
    public TMapIndex(int _curX, int _curY, int _toX, int _toY)
    {
        curX = _curX;
        curY = _curY;
        toX = _toX;
        toY = _toY;
    }

    public TMapIndex(int[] _curIndex, int[] _toIndex)
    {
        curX = _curIndex[0];
        curY = _curIndex[1];
        toX = _toIndex[0];
        toY = _toIndex[1];
    }

    public TMapIndex(TokenBase _curToken, TokenBase _toToken)
    {
        curX = _curToken.GetXIndex();
        curY = _curToken.GetYIndex();
        toX = _toToken.GetXIndex();
        toY = _toToken.GetYIndex();
    }
}