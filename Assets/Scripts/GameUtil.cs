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

    public static List<HideTile> GetHideTileListInRange(int _range, int _centerX, int _centerY)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY).ConvertAll(new System.Converter<int[], HideTile>(GetHideTileFromMap)); // 사거리 내부 안의 타일 가져오기
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
        //cur높이에서 to높이까지 쭉 좌, 우로 진행했을 때의 x 값을 도출하여 진행가능한 방향을 알아본다.
        TileDirection direction = TileDirection.RightUp;

        //0. 예외 같은 높이라면 2방향중 하나
        if(_tMapIndex.curY.Equals(_tMapIndex.toY))
        {
            //같은 높이에 있으면 최단경로 방향은 좌우 중 하나
            if ((_tMapIndex.toX - _tMapIndex.curX) > 0)
                return TileDirection.Right;
            else
                return TileDirection.Left;
        }

        //1. 기울기 계산을 위해 현재 높이의 x 값을 소수점으로 수정 - 현재 대각선 기울기는 2로서 2칸 오를때마다 x값이 1씩 변함.  
        int reviseCurX = _tMapIndex.curX * 10; //소수점 0.5 대신 *10을하고 5를 더하는식으로 수행
        if (_tMapIndex.curY % 2 == 1)
        {
            //홀수 높이라면
            reviseCurX += 5; //5를 추가로 소수점 대신 *10 늘린거. 
        }
        //2. 오른편인경우 기울기는 2고, reviseToX = 0.5(reviseToY-reviseCurY)+reviseCurX 가 되는데 y를 revise하는대신 0.5 배율에 10을 곱해서 진행 
        int yGap = _tMapIndex.toY - _tMapIndex.curY; // == reviseToY-reviseCurY
        int reviseRightX = (int)((5 * (yGap) + reviseCurX) * 0.1f);
        int reviseLeftX = (int)((-5 * (yGap) + reviseCurX) * 0.1f); //*10 했던걸 다시 0.1로, 15 25 같은 홀수 좌표는 1,2 가 되며 알아서 보정됨 
        if(yGap < 0)
        {
            //아래로 향하는경우 2 기울기가 leftX가 되고 -2 기울기가 rightX가 됨
            int tempLeftX = reviseLeftX;
            reviseLeftX = reviseRightX;
            reviseRightX = tempLeftX;
        }
        // 위 좌표는 toY좌표까지 쭉진행했을때의 x 좌표들 값으로 이 사이에 있다면 좌우 어디든 가능, 해당 좌표 이하 이상이라면 해당방향으로만 가야함 

        //3. 좌우로 어느정도 차이가 나는지 구별
        int xGap = 0; // -2 왼쪽진행도 필요, -1 왼쪽대각으로만 진행 해야함, 0 좌우 대각 어디로든, 1 오른대각으로만 진행, 2 오른쪽도 필요 
        if (reviseRightX < _tMapIndex.toX)
            xGap = 2;
        else if (reviseRightX == _tMapIndex.toX)
            xGap = 1;
        else if (_tMapIndex.toX == reviseLeftX)
            xGap = -1;
        else if (_tMapIndex.toX < reviseLeftX)
            xGap = -2;

        //Debug.Log(_tMapIndex.curX + "," + _tMapIndex.curY + "에서 " + _tMapIndex.toY + "높이에서 x값들\n" + reviseLeftX + " " + reviseRightX+"xGap:"+xGap);

        //4. 높이와 좌우 간격에 따라 진행가능한 방향 도출
        int random = Random.Range(0, 2); //두가지 갈래가 가능한 경우 대비 랜덤수 뽑아놓기
        if(0 < yGap)
        {
            //위로 향해야하는 경우에서
            if (xGap.Equals(-2))
            {
                //왼쪽 이동도 필요한 경우에는 leftUp 혹은 left를 진행하면됨. 
                if (random.Equals(0))
                    return TileDirection.Left;
                else
                    return TileDirection.LeftUp;
            }
            if(xGap.Equals(-1))
                return TileDirection.LeftUp;
            if(xGap.Equals(0))
            {
                if (random.Equals(0))
                    return TileDirection.LeftUp;
                else
                    return TileDirection.RightUp;
            }    
            if (xGap.Equals(1))
                return TileDirection.RightUp;
            if (xGap.Equals(2))
            {
                if (random.Equals(0))
                    return TileDirection.Right;
                else
                    return TileDirection.RightUp;
            }
        }
        else if (yGap < 0)
        {
            //아래로 향해야하는 경우에서
            if (xGap.Equals(-2))
            {
                //왼쪽 이동도 필요한 경우에는 leftUp 혹은 left를 진행하면됨. 
                if (random.Equals(0))
                    return TileDirection.Left;
                else
                    return TileDirection.LeftDown;
            }
            if (xGap.Equals(-1))
                return TileDirection.LeftDown;
            if (xGap.Equals(0))
            {
                if (random.Equals(0))
                    return TileDirection.LeftDown;
                else
                    return TileDirection.RightDown;
            }
            if (xGap.Equals(1))
                return TileDirection.RightDown;
            if (xGap.Equals(2))
            {
                if (random.Equals(0))
                    return TileDirection.Right;
                else
                    return TileDirection.RightDown;
            }
        }



        return direction;
    }

    #endregion

    /*
    빌드세팅에 디파인 옵션 넣고 가져오는법
        PlayerSettings.SetScriptingDefineSymbolsForGroup(ebuildTarget, m_Defines.ToString());
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
    */

    public static bool IsThereMap(int _x, int _y)
    {
        int maxX = MgToken.GetInstance().GetMaps().GetLength(0);
        int maxY = MgToken.GetInstance().GetMaps().GetLength(1);
        
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

    public static HideTile GetHideTileFromMap(int[] _index)
    {
        return MgToken.GetInstance().GetHideMaps()[_index[0], _index[1]];
    }

    public static TokenTile GetTileTokenFromMap(int[] _index)
    {
        return MgToken.GetInstance().GetMaps()[_index[0], _index[1]];
    }

    public static TokenTile GetTileTokenFromMap(TokenChar _char)
    {
        return MgToken.GetInstance().GetMaps()[_char.GetXIndex(), _char.GetYIndex()];
    }

    public static ObjectTokenBase GetTokenObjectFromMap(int[] _index)
    {
        return MgToken.GetInstance().GetMaps()[_index[0], _index[1]].GetObject();
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

    public TMapIndex(TokenBase _curToken, TokenBase _toToken)
    {
        curX = _curToken.GetXIndex();
        curY = _curToken.GetYIndex();
        toX = _toToken.GetXIndex();
        toY = _toToken.GetYIndex();
    }
}