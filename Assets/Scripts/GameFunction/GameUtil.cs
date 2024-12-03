using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum TileDirection
{
    RightUp, Right, RightDown, LeftDown, Left, LeftUp
}

public static class GameUtil 
{
    #region 타일 계산
    public static List<int[]> GetTileIdxListInRange(int _range, int _centerX, int _centerY, int _minRange = 0)
    {
        //   _1 _2
        //  /6    |3  왼쪽을 스타트로 해당 사거리 _range 단계만큼 TileDirection enum 방향으로 진행하면서 상대적 좌표 도출
        //   |5  /4

        List<int[]> rangedTile = new List<int[]>();


        int[] startPoint = {_centerX , _centerY}; //기준이되는 시작점을 센터로 잡음
        //최소 사거리가 0 일때만 원점 추가 
        if(_minRange== 0)
            rangedTile.Add(new int[] { _centerX, _centerY });
        //string resultLog = "";
        //한칸씩 주변을 돌면서 기록
        for (int range = _minRange; range <= _range; range++)
        {
            //주변을 도는 6 방향 - TileDirection으로 매칭 -> RightUp방향으로 먼저 진행하므로, -1(살피려는 범위의 사거리)로 시작점을 조정
            startPoint[0] = _centerX - range;
            for (int direction = 0; direction <= 5; direction++)
            {
                //해당 방향으로 나아가는 횟수는 현재 살펴보는 range
                for (int i = 1; i <= range; i++) //6방향으로 돔
                {
                   // Debug.Log((TileDirection)x + "방향으로 " + i + "번 진행");
                   //1. 타일에서 특정방향으로 갈때 상대 좌표값 도출
                    int[] newTile = GetGapCoordiFromDirect(startPoint[1], (TileDirection)direction); 

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

    public static List<TokenTile> GetTileTokenListInRange(int _range, int[] _center, int _minRange = 0)
    {
       return GameUtil.GetTileIdxListInRange(_range, _center[0], _center[1], _minRange).ConvertAll(new System.Converter<int[], TokenTile>(GetTileTokenFromMap)); // 사거리 내부 안의 타일 가져오기
    }

    public static List<ObjectTokenBase> GetTileObjectInRange(int _range, int _centerX, int _centerY, int _minRange = 0)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY, _minRange).ConvertAll(new System.Converter<int[], ObjectTokenBase>(GetTileObjectFromMapCoordi)); // 사거리 내부 안의 타일 가져오기
    }

    public static int[] GetGapCoordiFromDirect(int _centerY, TileDirection _direction)
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

    public static int[] GetPosFromDirect(int[] _center, TileDirection _direction)
    {
        //1. 움직일 좌표
        int[] directCoordi = GetGapCoordiFromDirect(_center[1], _direction);
        //2. 움직인 좌표
        int[] movedCoordi = new int[] { _center[0] + directCoordi[0], _center[1] + directCoordi[1] };

        return movedCoordi;
    }

    public static int[] GetPosEmptyChar(int[] _center)
    {
        //기준점을 중심으로 몬스터가 없는 타일을 반환
        int cur = 0;
        int max = 5;
        while (cur <= max)
        {
            List<TokenTile> rangeList = GetTileTokenListInRange(cur, _center, cur);
            for (int i = 0; i < rangeList.Count; i++)
            {
                TokenTile curTile = rangeList[i];
                if (curTile.GetCharsInTile().Count == 0)
                    return curTile.GetMapIndex();
            }
            cur += 1;
        }

        return null;
    }

    //목적지까지 최단 타일 개수
    public static int GetMinRange(TokenBase _fromToken, TokenBase _toToken)
    {
        TMapIndex mapIndex = new TMapIndex(_fromToken, _toToken);
        return GetMinRange(mapIndex);
    }

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
        int random = UnityEngine.Random.Range(0, 2); //두가지 갈래가 가능한 경우 대비 랜덤수 뽑아놓기
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

    //2차배열을 1자리 _num 인덱스로 찾는경우
    public static int[] GetXYPosFromIndex(int _xLength, int _num)
    {
        //몇행 몇열인지를 빼기. 
        int height = _num / _xLength;
        int width = _num % _xLength;
        int[] pos = new int[] { width, height };
        return pos;
    }

    public static bool IsRight(int[] _curPos, int[] _targetPos)
    {
        //보고있는게 오른편인가 
        bool isRight = false;
        //좌표 y값이 홀수면 0.5 이동한 보정을 해서 x값의 비교로 오른편인지 판단
        //1. 0.5 대신 5를 더하기 위해 기존 x값에 10을 곱함
        int curX = _curPos[0] * 10;
        //2. 이후 y값이 홀수인지 판단하여 보정값 적용
        if (_curPos[1] % 2 == 1)
            curX += 5;
        //3. 마찬가지
        int targetX = _targetPos[0] * 10;
        if (_targetPos[1] % 2 == 1)
            targetX += 5;
        //4. x위치값 비교 - 같은경우도 오른편인걸로. 
        if (curX <= targetX)
            isRight = true;

        return isRight;
    }

    #endregion

    #region 스폰 관련
    public static List<int[]> GetSpawnPos(ESpawnPosType _spawnPosType, int _spawnCount, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        List<int[]> spawnPos = new();
        int chunkNum = _chunkNum;
        if (chunkNum.Equals(MGContent.NO_CHUNK_NUM))
        {
            //만약 없는 청크 구역이라면 매인케릭이 있는 청크로 진행 
            chunkNum = GetMainCharChunkNum();
        }
        switch (_spawnPosType)
        {
            case ESpawnPosType.Random: //청크 내부에서 랜덤
                List<int> randomPosList = GameUtil.GetRandomNum(25, _spawnCount);
                Chunk madeChunk = MGContent.GetInstance().GetChunk(chunkNum);
                for (int i = 0; i < randomPosList.Count; i++)
                {
                    int chunkTileNum = randomPosList[i]; //청크 내부에서 해당 타일의 idx
                    int[] tilePos = GameUtil.GetXYPosFromIndex(madeChunk.tiles.GetLength(0), chunkTileNum);//청크 기준으로 좌표 도출 
                    int[] spawnCoord = madeChunk.tiles[tilePos[0], tilePos[1]].GetMapIndex();//청크 좌표를 월드 좌표로 전환
                    spawnPos.Add(spawnCoord);
                }
                break;
            case ESpawnPosType.CharRound:
                int[] b = MgToken.GetInstance().GetMainChar().GetMapIndex();
                int[] rightUp = GameUtil.GetPosFromDirect(b, TileDirection.RightUp);
                int[] rightDown = GameUtil.GetPosFromDirect(b, TileDirection.RightDown);
                int[] left = GameUtil.GetPosFromDirect(b, TileDirection.Left);
                spawnPos = new List<int[]>() { rightUp, rightDown, left };
                break;
        }

        return spawnPos;
    }
    #endregion

    #region 청크 계산
    public static int GetChunkNum(int[] _coordi)
    {
        return GetTileTokenFromMap(_coordi).ChunkNum;
    }

    public static int GetMainCharChunkNum()
    {
        return GetChunkNum(PlayerManager.GetInstance().GetMainChar().GetMapIndex());
    }

    public static int GetTileCountInChunk(int _chunkNum)
    {
        Chunk chunk = MGContent.GetInstance().GetChunk(_chunkNum);

        if (chunk == null)
            return 0;

        int x = chunk.tiles.GetLength(0);
        int y = chunk.tiles.GetLength(1);
        return x * y;
    }
    #endregion

    /*
    빌드세팅에 디파인 옵션 넣고 가져오는법
        PlayerSettings.SetScriptingDefineSymbolsForGroup(ebuildTarget, m_Defines.ToString());
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
    */

    public static void TestFindTileWithWork(Func<bool> testFunc)
    {
        bool isDone = testFunc();
    }

    #region 좌표에 해당하는 타일 얻기
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

    public static bool IsThereMap(int[] _coord)
    {
        return IsThereMap(_coord[0], _coord[1]);
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
        if (IsThereMap(_index) == false)
            return null;
        return MgToken.GetInstance().GetHideMaps()[_index[0], _index[1]];
    }

    public static TokenTile GetTileTokenFromMap(int[] _index)
    {
        if (IsThereMap(_index) == false)
            return null;
        return MgToken.GetInstance().GetMaps()[_index[0], _index[1]];
    }

    public static TokenTile GetTileTokenFromMap(TokenChar _char)
    {
        if (IsThereMap(_char.GetMapIndex()) == false)
            return null;
        return MgToken.GetInstance().GetMaps()[_char.GetXIndex(), _char.GetYIndex()];
    }

    public static ObjectTokenBase GetTileObjectFromMapCoordi(int[] _index)
    {
        if (IsThereMap(_index) == false)
            return null;
        return MgToken.GetInstance().GetMaps()[_index[0], _index[1]].GetObject();
    }

    public static int GetMapLength(bool _isX)
    {
        if (_isX)
        {
            return MgToken.GetInstance().GetMaps().GetLength(0);
        }

        return MgToken.GetInstance().GetMaps().GetLength(1);
    }
    #endregion

    #region enum 관련 함수
    public static int ParseEnumValue(System.Enum _enumValue)
    {
        int enumIntValue = (int)System.Enum.Parse(_enumValue.GetType(), _enumValue.ToString());

        //  Debug.Log("들어옴");
        return enumIntValue;
    }

    public static int EnumLength(System.Enum _enumValue)
    {
        int enumLength = (int)System.Enum.GetValues(_enumValue.GetType()).Length;

        //  Debug.Log("들어옴");
        return enumLength;
    }

    public static int[] EnumLengthArray(Type _enumValue)
    {
        int[] enumLengthArray = new int[(int)System.Enum.GetValues(_enumValue).Length];
        return enumLengthArray;
    }

    public static string[] ParseEnumStrings(System.Enum _enumValue)
    {
        return System.Enum.GetNames(_enumValue.GetType());
    }
    #endregion

    #region 스프레드 파싱 함수
    public static List<int[]> MakeMatchCode(System.Enum _codeEnum, string[] _dbCodes)
    {
        //db의 벨류Code 값과 인게임의 enumCode 값을 매칭
        //벨류코드의 0번째가 enumCode의 몇 번째인지 짜서 반환
        List<int[]> matchCodeList = new();

        if (_codeEnum == null)
            return matchCodeList;

        string[] enumCodes = ParseEnumStrings(_codeEnum);
        //거의 풀 매치 돌려야하네
        for (int dbIndex = 0; dbIndex < _dbCodes.Length; dbIndex++)
        {
            string dbCode = _dbCodes[dbIndex];
            for (int enumIndex = 0; enumIndex < enumCodes.Length; enumIndex++)
            {
                string enumCode = enumCodes[enumIndex];
                if(dbCode == enumCode)
                {
                    int[] match = { dbIndex, enumIndex }; //디비 x번째의 값이 이넘y번째 값인걸로 코드 산출
                    matchCodeList.Add(match);
                    break;
                }
            }
        }
        return matchCodeList;
    }

    public static void InputMatchValue(ref int[] _valueArray, List<int[]> _matchCode, string[] valueCode)
    {
        for (int i = 0; i < _matchCode.Count; i++)
        {
            int dbIndex = _matchCode[i][0];
            int tokenIndex = _matchCode[i][1];

            _valueArray[tokenIndex] = int.Parse(valueCode[dbIndex]);
        }
    }

    public static IEnumerator GetSheetDataCo(string documentID, string[] sheetID, Action doneAct = null, 
        Action<bool, int, string> process = null)
    {
        int doneWork = sheetID.Length; //처리해야할 숫자
        int curWork = 0;//처리한 숫자
        while (curWork < doneWork)
        {
            string url = $"https://docs.google.com/spreadsheets/d/{documentID}/export?format=tsv&gid={sheetID[curWork]}";

            UnityWebRequest req = UnityWebRequest.Get(url);

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.ConnectionError || req.responseCode != 200)
            {

                process?.Invoke(false, curWork, null);
            }
            else
            {
                process?.Invoke(true, curWork, req.downloadHandler.text);
            }
            
            curWork += 1;
        }

        doneAct?.Invoke(); //보통은 GameManager에 작업 완료했음을 알림. 
    }

    public static bool TryIndexMatchNum(string[] _valueNames, string _findIndexStr, out int _findNum)
    {
        for( int i = 0; i < _valueNames.Length; i++)
        {
            if (_valueNames[i] == _findIndexStr)
            {
                _findNum = i;
                return true;
            }
        }
        _findNum = -1;
        return false;
    }

    #region TOderItem 변환
    public static TItemListData ParseCostDataArray(string[] _parsingData, int _costIndex, bool _noneDataAdd = false)
    {
        TItemListData orderCostData = new TItemListData();
        //파싱 데이터 배열 해당 index에 자료가 있는지 체크 없으면 반환. 
        if(_parsingData.Length <= _costIndex)
        {
            return orderCostData;
        }
        string[] costArray = _parsingData[_costIndex].Split(FixedValue.PARSING_LIST_DIVIDE); //아이템 리스트를 항목으로 구별
        for (int i = 0; i < costArray.Length; i++)
        {
            TOrderItem orderItem = ParseOrderItem(costArray[i]);  //[0] : 토큰타입 [1] :항목에서 pid [2] : 수량 으로 이뤄진 문자를 보내 Torder로 변환.
            if (_noneDataAdd == true || orderItem.IsVaridTokenType())
            {
                orderCostData.Add(orderItem); //유효한 재료면 추가
            }
        }

        return orderCostData;
    }

    public static void ParseOrderItemList(List<TOrderItem> _itemList, string[] _parsingData, int _costIndex, bool _noneDataAdd = false)
    {
        //파싱 된 열을 통째로 전달시 
        if (_parsingData.Length <= _costIndex)
        {
            return;
        }
        string[] costArray = _parsingData[_costIndex].Split(FixedValue.PARSING_LIST_DIVIDE); //아이템 리스트를 항목으로 구별
        for (int i = 0; i < costArray.Length; i++)
        {
            TOrderItem orderItem = ParseOrderItem(costArray[i]);  //[0] : 토큰타입 [1] :항목에서 pid [2] : 수량 으로 이뤄진 문자를 보내 Torder로 변환.
            if (_noneDataAdd == true || orderItem.IsVaridTokenType())
            {
                _itemList.Add(orderItem); //유효한 재료면 추가
            }
        }

    }

    public static void ParseIntList(List<int> _itemList, string[] _parsingData, int _costIndex, bool _noneDataAdd = false)
    {
        //파싱 된 열을 통째로 전달시 
        if (_parsingData.Length <= _costIndex)
        {
            return;
        }
        string[] costArray = _parsingData[_costIndex].Split(FixedValue.PARSING_LIST_DIVIDE); //아이템 리스트를 항목으로 구별
        for (int i = 0; i < costArray.Length; i++)
        {
            if(int.TryParse(costArray[i], out int parseInt))
                _itemList.Add(parseInt);
        }

    }

    public static void ParseOrderItemList(List<TOrderItem> _itemList, string _parsingStrData, bool _noneDataAdd = false)
    {
       // 해당 열을 직접 전달시
        string[] costArray = _parsingStrData.Split(FixedValue.PARSING_LIST_DIVIDE); //아이템 리스트를 항목으로 구별
        for (int i = 0; i < costArray.Length; i++)
        {
            TOrderItem orderItem = ParseOrderItem(costArray[i]);  //[0] : 토큰타입 [1] :항목에서 pid [2] : 수량 으로 이뤄진 문자를 보내 Torder로 변환.
            if (_noneDataAdd == true || orderItem.IsVaridTokenType()) //넌데이터라도 추가 요청했거나 유효한 거면 추가 
            {
                _itemList.Add(orderItem); //유효한 재료면 추가
            }
        }

    }
   
    public static TOrderItem ParseOrderItem(string costData)
    {
        //토큰그룹_pid_수량 의 string으로 넘어온 코스트 데이터
        string[] divideType = costData.Split(FixedValue.PARSING_TYPE_DIVIDE);
        TOrderItem noneData = new TOrderItem(TokenType.None, 0, 0);
        //[0] : 토큰타입 [1] :항목에서 pid [2] : 수량
        if(divideType.Length != 3)
        {
            //규격에 맞지 않는 경우엔 리턴 
            return noneData;
        }

        if (System.Enum.IsDefined(typeof(TokenType), divideType[0]))
        {
            //재료의 토큰그룹은 파싱.
            TokenType tokenType = System.Enum.Parse<TokenType>(divideType[0]);

            //두번째 pid가 숫자로 되있는지 해당 enum의 문자값으로 되어있는지 확인
            if (int.TryParse(divideType[1], out int enumIndex) == true)
            {
                //바로 pid로 되어있다면
                if (int.TryParse(divideType[2], out int needAmount) == true)
                {
                    //마지막 수량까지 잘 적혀있으면 데이터에 추가
                    return new TOrderItem(tokenType, enumIndex, needAmount);
                }
                //수량이 이상하면 넌데이터 리턴
                return noneData;
            }

            //enum의 문자값으로 기록되어있으면 index를 산출
            System.Type findEnum = FindEnum(tokenType);

            //토큰 타입에 따라 적절한 enum 타입을 정의하고
            if (findEnum == null)
            {
                //적절한 enum 그룹이 없으면 넘김
                return noneData;
            }
            //해당 enum타입에서 [1] 값이 있는지 확인
            if (System.Enum.IsDefined(findEnum, divideType[1]))
            {
                //[1] 세부 pid파싱 
                int enumPid = (int)System.Enum.Parse(findEnum, (divideType[1]));
                if (int.TryParse(divideType[2], out int needAmount) == true)
                {
                    //마지막 수량까지 잘 적혀있으면 데이터에 추가
                    return new TOrderItem(tokenType, enumPid, needAmount);
                }
                if (divideType[2].Equals(FixedValue.PARSING_VALUE_ALL))
                {
                    return new TOrderItem(tokenType, enumPid, FixedValue.ALL);
                }
            }
        }
    
        return noneData;
    }

    public static System.Type FindEnum(TokenType _tokenType)
    {
        System.Type findEnum = null;
        switch (_tokenType)
        {
            case TokenType.Capital:
                findEnum = typeof(Capital);
                break;
            case TokenType.NationStat:
                findEnum = typeof(NationStatEnum);
                break;
            case TokenType.Conversation:
                findEnum = typeof(ConversationEnum);
                break;
            case TokenType.Content:
                findEnum = typeof(ContentEnum);
                break;
            case TokenType.OnChange:
                findEnum = typeof(OnChangeEnum);
                break;
            case TokenType.Nation:
                findEnum = typeof(NationEnum);
                break;
            case TokenType.CharStat:
                findEnum = typeof(CharStat);
                break;
            case TokenType.Bless:
                findEnum = typeof(BlessSynergeCategoryEnum);
                break;
            case TokenType.UIOpen:
                findEnum = typeof(UICodeEnum);
                break;
            case TokenType.EventPlaceNationSpawn:
            case TokenType.EventPlaceChunkSpawn:
                findEnum = typeof(TileType);
                break;
        }
        return findEnum;
    }

    public static List<TOrderItem> ReverseItemList(List<TOrderItem> _originList)
    {
        List<TOrderItem> reverseList = new();
        for (int i = 0; i < _originList.Count; i++)
        {
            TOrderItem reverse = ReverseItemValue(_originList[i]);
            reverseList.Add(reverse);
        }
        return reverseList;
    }

    public static TOrderItem ReverseItemValue(TOrderItem _item)
    {
        TOrderItem reverse = _item;
        reverse.Value *= -1;
        return reverse;
    }
    #endregion
    #endregion

    #region Token 에셋 로드
    //오리지널 Class 생성시 필요한 유니티 자료를 연계하는곳, 그 자료명은 아이템명에 + icon, prefeb 형식으로 지정. 
    public static Sprite GetIconFromResource(TokenBase _token, string _resourcePath = null)
    {
        //클래스명 + Icon 으로 네이밍을해서 불러오며, sprite가 준비되지않은경우에는, testIcon으로 대체해서 진행하고, 이름도 바꿔서, json으로 미비된 부분을 확인할 수 있도록 하기
        string iconName = _token.GetItemName()+" Icon";
        Sprite sprite = Resources.Load<Sprite>(_resourcePath + iconName);
        
        return sprite;
    }

    public static GameObject GetPrefebFromResource(TokenBase _token, string _resourcePath = null)
    {
        //리소스 폴더내에 따로 폴더를 통해 루트를 수정했으면 해당 수정된 루트를 입력
        //일단은 CropPrefeb, SkillPrefeb 두 폴더를 상정 하고 최초 DB를 생성하는 DbItem.cs와 DbSkill.cs 에서 리셋할때 입력하도록 진행. 
        string prefabName = _token.GetItemName() + " Prefeb";
        GameObject load = Resources.Load<GameObject>(_resourcePath + prefabName);
        return load;
    }
    #endregion

    public static bool IsInMainChar(TokenTile _tile)
    {
        //해당 타일에 메인케릭이 있는지 체크 

        List<TokenChar> chars = _tile.GetCharsInTile();
        for (int i = 0; i < chars.Count; i++)
        {
            if (chars[i].isMainChar)
                return true;
        }
        return false;

    }

    public static List<int> GetRandomNum(int _length, int _randomCount, List<int> exceptList = null)
    {
        //범위에서 랜덤으로 숫자 count만큼 뽑기
        List<int> randomList = new();
        List<int> rangeList = new();
        if (exceptList == null)
            exceptList = new();
        for (int i = 0; i < _length; i++)
        {
            //제외 리스트에 있는 숫자는 포함 안함 
            if (exceptList.IndexOf(i) >= 0)
                continue;
            rangeList.Add(i); //범위 만큼 숫자 넣기
        }

        for (int i = 1; i <= _randomCount; i++)
        {
            int ranIndex = UnityEngine.Random.Range(0, rangeList.Count);
            int randomNum = rangeList[ranIndex]; //범위 리스트의 인덱스의 숫자를 빼고
            randomList.Add(randomNum); //랜덤리스트에 넣고
            rangeList.RemoveAt(ranIndex); //범위리스트의 해당 인덱스 아이템 제거 
        }

        return randomList;
    }

    public static void LookTargetTile(TokenChar _char, TokenTile _targetTile)
    {
        bool isRight = IsRight(_char.GetMapIndex(), _targetTile.GetMapIndex());
        _char.GetObject().m_charIcon.flipX = isRight;
    }
 
    public static void DropMagnetItem(int[] mapIndex)
    {
        TokenTile tile = GetTileTokenFromMap(mapIndex);
        Vector3 dropPosition = tile.GetObject().gameObject.transform.position; // 위치 뽑고
        MagnetItem magnetItem = MonoBehaviour.Instantiate(GamePlayMaster.GetInstance().testMangetSample);
        magnetItem.SetMagnetInfo(dropPosition);
    }

    private static void MakeFence(Chunk _fenceChunk)
    {
        Sprite fenceSprite = TempSpriteBox.GetInstance().GetTileSprite(TileType.Nomal);

        int xLength = _fenceChunk.tiles.GetLength(0);
        int yLength = _fenceChunk.tiles.GetLength(1);

        //외곽인경우만 스프라이트 바꾸기

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (x == 0 || x == xLength - 1)
                {
                    //x축이 0이거나 맨 끝인경우 y 0~max 달리고
                    _fenceChunk.tiles[x, y].SetSprite(fenceSprite);
                }
                else
                {
                    //x값이 1~어느 사이인 경우엔 y 처음과 끝만 색칠하고 해당 열은 패스 
                    _fenceChunk.tiles[x, 0].SetSprite(fenceSprite);
                    _fenceChunk.tiles[x, yLength - 1].SetSprite(fenceSprite);
                    break;
                }
            }
        }

    }
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