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
    #region Ÿ�� ���
    public static List<int[]> GetTileIdxListInRange(int _range, int _centerX, int _centerY, int _minRange = 0)
    {
        //   _1 _2
        //  /6    |3  ������ ��ŸƮ�� �ش� ��Ÿ� _range �ܰ踸ŭ TileDirection enum �������� �����ϸ鼭 ����� ��ǥ ����
        //   |5  /4

        List<int[]> rangedTile = new List<int[]>();


        int[] startPoint = {_centerX , _centerY}; //�����̵Ǵ� �������� ���ͷ� ����
        //�ּ� ��Ÿ��� 0 �϶��� ���� �߰� 
        if(_minRange== 0)
            rangedTile.Add(new int[] { _centerX, _centerY });
        //string resultLog = "";
        //��ĭ�� �ֺ��� ���鼭 ���
        for (int range = _minRange; range <= _range; range++)
        {
            //�ֺ��� ���� 6 ���� - TileDirection���� ��Ī -> RightUp�������� ���� �����ϹǷ�, -1(���Ƿ��� ������ ��Ÿ�)�� �������� ����
            startPoint[0] = _centerX - range;
            for (int direction = 0; direction <= 5; direction++)
            {
                //�ش� �������� ���ư��� Ƚ���� ���� ���캸�� range
                for (int i = 1; i <= range; i++) //6�������� ��
                {
                   // Debug.Log((TileDirection)x + "�������� " + i + "�� ����");
                   //1. Ÿ�Ͽ��� Ư���������� ���� ��� ��ǥ�� ����
                    int[] newTile = GetGapCoordiFromDirect(startPoint[1], (TileDirection)direction); 

                    //2. ����� �����ǥ�� �⺻ ��ǥ���� ���ϸ� �̵��� ���� ��ǥ�� ����. 
                    newTile[0] += startPoint[0];
                    newTile[1] += startPoint[1];

                    //3. �������� �̵��� ������ǥ�� ����
                    startPoint[0] = newTile[0];
                    startPoint[1] = newTile[1];
                    if (IsThereMap(newTile[0], newTile[1])) //�� ������ǥ�� �����ϴ� ��ǥ��� �� �߰�
                    rangedTile.Add(newTile); //������ �ְ� 
                    //resultLog += (startPoint[0] + _centerX).ToString() + "," + (startPoint[1] + _centerY).ToString() + "/";
                }
                //resultLog += "\n";
            }
            //6���� ��� �������� �ٽ� Left �������� ���ƿ� ���⼭ ��ĭ�� �ֺ� ������ ���� ������ �������� �̵�
        
        }
        
        //Debug.Log("ã�� Ÿ�� ����" + rangedTile.Count);

        return rangedTile;
    }

    public static List<TokenTile> GetTileTokenListInRange(int _range, int[] _center, int _minRange = 0)
    {
       return GameUtil.GetTileIdxListInRange(_range, _center[0], _center[1], _minRange).ConvertAll(new System.Converter<int[], TokenTile>(GetTileTokenFromMap)); // ��Ÿ� ���� ���� Ÿ�� ��������
    }

    public static List<ObjectTokenBase> GetTileObjectInRange(int _range, int _centerX, int _centerY, int _minRange = 0)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY, _minRange).ConvertAll(new System.Converter<int[], ObjectTokenBase>(GetTileObjectFromMapCoordi)); // ��Ÿ� ���� ���� Ÿ�� ��������
    }

    public static int[] GetGapCoordiFromDirect(int _centerY, TileDirection _direction)
    {
        int[] nextTileIndex = new int[2];
        int xPlus = 0;
        int yPlus = 0;
        //���Ʒ� �̵��� ������ �ʿ��� �������
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
        //1. ������ ��ǥ
        int[] directCoordi = GetGapCoordiFromDirect(_center[1], _direction);
        //2. ������ ��ǥ
        int[] movedCoordi = new int[] { _center[0] + directCoordi[0], _center[1] + directCoordi[1] };

        return movedCoordi;
    }

    public static int[] GetPosEmptyChar(int[] _center)
    {
        //�������� �߽����� ���Ͱ� ���� Ÿ���� ��ȯ
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

    //���������� �ִ� Ÿ�� ����
    public static int GetMinRange(TokenBase _fromToken, TokenBase _toToken)
    {
        TMapIndex mapIndex = new TMapIndex(_fromToken, _toToken);
        return GetMinRange(mapIndex);
    }

    public static int GetMinRange(TMapIndex _tMapIndex)
    {
        int minY = Mathf.Abs(_tMapIndex.curY - _tMapIndex.toY); //�ּ� ���������ϴ� Y �� 
        int gapX = Mathf.Abs(_tMapIndex.curX - _tMapIndex.toX); //���� y���� �Ǿ��� �� x �̵� ��

        //���� ���̶�� gapX ��ŭ �̵��ϸ��
        if (minY == 0)
            return gapX;

        //�ٸ� ���̶�� �ּ� �����̸� ���� �̵� ��, to��ǥ�� �������ٵ�, �̶� �ش�������� �̵��ص� gapX�� �پ���� ������ �߰� �̵��� �ʿ�
        if (gapX * 2 <= minY)
            return minY; //Ȧ¦ ���� ���Ǿ��� �ּ� �������̰� gapX���� 2�̻��̸�, minY��ŭ �̵��ϸ� ������ ���ް���. 

        int moveX = minY / 2; //�ּ� ���� ������ 2��ŭ�� �������� �̵��� ����. 
        bool odd = (_tMapIndex.curY % 2 == 1); //���� ���̰� Ȧ�� ���ΰ�
        bool directRight = 0 < (_tMapIndex.toX - _tMapIndex.curX); //to ������ ���������� �ΰ� �����ΰ�
        //�߰��� Ȧ�� ��°�̵��� 1 �̵� ���ɿ��� �Ǵ�. 
        bool haveRemain = (minY % 2 == 1); //������ 1ĭ�� ���� ���
        if (haveRemain)
        {
            if (odd && directRight)
                moveX += 1;
            else if (!odd && !directRight)
                moveX += 1;
        }

        //moveX �� �ش� ���̱��� ����, ���� �밢������ �������� ���� x ��ȭ���� ���� 
        int movedX = _tMapIndex.curX;
        int minRange = minY; //�ּ� ������ �̵��ؾ��ϰ� 
        if (directRight)
        {
            movedX += moveX; //�ش� ���̱��� �ִ��� ���������� ���� �̶� 
            if (movedX < _tMapIndex.toX) //���� ������ X�� �� ������ �ִٸ�
                minRange += (_tMapIndex.toX - movedX);

        }
        else
        {
            movedX -= moveX;
            if (_tMapIndex.toX < movedX ) //���� ������ X�� �� ���� �ִٸ�
                minRange += (movedX - _tMapIndex.toX); //������ ������ �� �������� �̵��ؾ��� �Ÿ��� ���ؼ� �߰�
        }
            

        return minRange;
    }

    //������ Ÿ�� ���� �ֱ������� ���� ���ǾƷ� ���� �ִ� Ÿ�� ������ ��ȯ
    public static TileDirection GetNextTileToGoal(TMapIndex _tMapIndex)
    {
        //���������� �ִܰ�η� ���� ���� Ÿ�Ͽ��� ���� �ִ� ������ ��ȯ
        //cur���̿��� to���̱��� �� ��, ��� �������� ���� x ���� �����Ͽ� ���డ���� ������ �˾ƺ���.
        TileDirection direction = TileDirection.RightUp;

        //0. ���� ���� ���̶�� 2������ �ϳ�
        if(_tMapIndex.curY.Equals(_tMapIndex.toY))
        {
            //���� ���̿� ������ �ִܰ�� ������ �¿� �� �ϳ�
            if ((_tMapIndex.toX - _tMapIndex.curX) > 0)
                return TileDirection.Right;
            else
                return TileDirection.Left;
        }

        //1. ���� ����� ���� ���� ������ x ���� �Ҽ������� ���� - ���� �밢�� ����� 2�μ� 2ĭ ���������� x���� 1�� ����.  
        int reviseCurX = _tMapIndex.curX * 10; //�Ҽ��� 0.5 ��� *10���ϰ� 5�� ���ϴ½����� ����
        if (_tMapIndex.curY % 2 == 1)
        {
            //Ȧ�� ���̶��
            reviseCurX += 5; //5�� �߰��� �Ҽ��� ��� *10 �ø���. 
        }
        //2. �������ΰ�� ����� 2��, reviseToX = 0.5(reviseToY-reviseCurY)+reviseCurX �� �Ǵµ� y�� revise�ϴ´�� 0.5 ������ 10�� ���ؼ� ���� 
        int yGap = _tMapIndex.toY - _tMapIndex.curY; // == reviseToY-reviseCurY
        int reviseRightX = (int)((5 * (yGap) + reviseCurX) * 0.1f);
        int reviseLeftX = (int)((-5 * (yGap) + reviseCurX) * 0.1f); //*10 �ߴ��� �ٽ� 0.1��, 15 25 ���� Ȧ�� ��ǥ�� 1,2 �� �Ǹ� �˾Ƽ� ������ 
        if(yGap < 0)
        {
            //�Ʒ��� ���ϴ°�� 2 ���Ⱑ leftX�� �ǰ� -2 ���Ⱑ rightX�� ��
            int tempLeftX = reviseLeftX;
            reviseLeftX = reviseRightX;
            reviseRightX = tempLeftX;
        }
        // �� ��ǥ�� toY��ǥ���� �������������� x ��ǥ�� ������ �� ���̿� �ִٸ� �¿� ���� ����, �ش� ��ǥ ���� �̻��̶�� �ش�������θ� ������ 

        //3. �¿�� ������� ���̰� ������ ����
        int xGap = 0; // -2 �������൵ �ʿ�, -1 ���ʴ밢���θ� ���� �ؾ���, 0 �¿� �밢 ���ε�, 1 �����밢���θ� ����, 2 �����ʵ� �ʿ� 
        if (reviseRightX < _tMapIndex.toX)
            xGap = 2;
        else if (reviseRightX == _tMapIndex.toX)
            xGap = 1;
        else if (_tMapIndex.toX == reviseLeftX)
            xGap = -1;
        else if (_tMapIndex.toX < reviseLeftX)
            xGap = -2;

        //Debug.Log(_tMapIndex.curX + "," + _tMapIndex.curY + "���� " + _tMapIndex.toY + "���̿��� x����\n" + reviseLeftX + " " + reviseRightX+"xGap:"+xGap);

        //4. ���̿� �¿� ���ݿ� ���� ���డ���� ���� ����
        int random = UnityEngine.Random.Range(0, 2); //�ΰ��� ������ ������ ��� ��� ������ �̾Ƴ���
        if(0 < yGap)
        {
            //���� ���ؾ��ϴ� ��쿡��
            if (xGap.Equals(-2))
            {
                //���� �̵��� �ʿ��� ��쿡�� leftUp Ȥ�� left�� �����ϸ��. 
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
            //�Ʒ��� ���ؾ��ϴ� ��쿡��
            if (xGap.Equals(-2))
            {
                //���� �̵��� �ʿ��� ��쿡�� leftUp Ȥ�� left�� �����ϸ��. 
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

    //2���迭�� 1�ڸ� _num �ε����� ã�°��
    public static int[] GetXYPosFromIndex(int _xLength, int _num)
    {
        //���� ������� ����. 
        int height = _num / _xLength;
        int width = _num % _xLength;
        int[] pos = new int[] { width, height };
        return pos;
    }

    public static bool IsRight(int[] _curPos, int[] _targetPos)
    {
        //�����ִ°� �������ΰ� 
        bool isRight = false;
        //��ǥ y���� Ȧ���� 0.5 �̵��� ������ �ؼ� x���� �񱳷� ���������� �Ǵ�
        //1. 0.5 ��� 5�� ���ϱ� ���� ���� x���� 10�� ����
        int curX = _curPos[0] * 10;
        //2. ���� y���� Ȧ������ �Ǵ��Ͽ� ������ ����
        if (_curPos[1] % 2 == 1)
            curX += 5;
        //3. ��������
        int targetX = _targetPos[0] * 10;
        if (_targetPos[1] % 2 == 1)
            targetX += 5;
        //4. x��ġ�� �� - ������쵵 �������ΰɷ�. 
        if (curX <= targetX)
            isRight = true;

        return isRight;
    }

    #endregion

    #region ���� ����
    public static List<int[]> GetSpawnPos(ESpawnPosType _spawnPosType, int _spawnCount, int _chunkNum = MGContent.NO_CHUNK_NUM)
    {
        List<int[]> spawnPos = new();
        int chunkNum = _chunkNum;
        if (chunkNum.Equals(MGContent.NO_CHUNK_NUM))
        {
            //���� ���� ûũ �����̶�� �����ɸ��� �ִ� ûũ�� ���� 
            chunkNum = GetMainCharChunkNum();
        }
        switch (_spawnPosType)
        {
            case ESpawnPosType.Random: //ûũ ���ο��� ����
                List<int> randomPosList = GameUtil.GetRandomNum(25, _spawnCount);
                Chunk madeChunk = MGContent.GetInstance().GetChunk(chunkNum);
                for (int i = 0; i < randomPosList.Count; i++)
                {
                    int chunkTileNum = randomPosList[i]; //ûũ ���ο��� �ش� Ÿ���� idx
                    int[] tilePos = GameUtil.GetXYPosFromIndex(madeChunk.tiles.GetLength(0), chunkTileNum);//ûũ �������� ��ǥ ���� 
                    int[] spawnCoord = madeChunk.tiles[tilePos[0], tilePos[1]].GetMapIndex();//ûũ ��ǥ�� ���� ��ǥ�� ��ȯ
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

    #region ûũ ���
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
    ���弼�ÿ� ������ �ɼ� �ְ� �������¹�
        PlayerSettings.SetScriptingDefineSymbolsForGroup(ebuildTarget, m_Defines.ToString());
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
    */

    public static void TestFindTileWithWork(Func<bool> testFunc)
    {
        bool isDone = testFunc();
    }

    #region ��ǥ�� �ش��ϴ� Ÿ�� ���
    public static bool IsThereMap(int _x, int _y)
    {
        int maxX = MgToken.GetInstance().GetMaps().GetLength(0);
        int maxY = MgToken.GetInstance().GetMaps().GetLength(1);
        
        if (_x < 0 || maxX <= _x)
            return false;
        if (_y < 0 || maxY <= _y)
            return false;
        //���� ��Ÿ� �̳��� ���̴��� �� Ư���� ���ٰ� üũ�Ǿ������� false��ȯ

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

    #region enum ���� �Լ�
    public static int ParseEnumValue(System.Enum _enumValue)
    {
        int enumIntValue = (int)System.Enum.Parse(_enumValue.GetType(), _enumValue.ToString());

        //  Debug.Log("����");
        return enumIntValue;
    }

    public static int EnumLength(System.Enum _enumValue)
    {
        int enumLength = (int)System.Enum.GetValues(_enumValue.GetType()).Length;

        //  Debug.Log("����");
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

    #region �������� �Ľ� �Լ�
    public static List<int[]> MakeMatchCode(System.Enum _codeEnum, string[] _dbCodes)
    {
        //db�� ����Code ���� �ΰ����� enumCode ���� ��Ī
        //�����ڵ��� 0��°�� enumCode�� �� ��°���� ¥�� ��ȯ
        List<int[]> matchCodeList = new();

        if (_codeEnum == null)
            return matchCodeList;

        string[] enumCodes = ParseEnumStrings(_codeEnum);
        //���� Ǯ ��ġ �������ϳ�
        for (int dbIndex = 0; dbIndex < _dbCodes.Length; dbIndex++)
        {
            string dbCode = _dbCodes[dbIndex];
            for (int enumIndex = 0; enumIndex < enumCodes.Length; enumIndex++)
            {
                string enumCode = enumCodes[enumIndex];
                if(dbCode == enumCode)
                {
                    int[] match = { dbIndex, enumIndex }; //��� x��°�� ���� �̳�y��° ���ΰɷ� �ڵ� ����
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
        int doneWork = sheetID.Length; //ó���ؾ��� ����
        int curWork = 0;//ó���� ����
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

        doneAct?.Invoke(); //������ GameManager�� �۾� �Ϸ������� �˸�. 
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

    #region TOderItem ��ȯ
    public static TItemListData ParseCostDataArray(string[] _parsingData, int _costIndex, bool _noneDataAdd = false)
    {
        TItemListData orderCostData = new TItemListData();
        //�Ľ� ������ �迭 �ش� index�� �ڷᰡ �ִ��� üũ ������ ��ȯ. 
        if(_parsingData.Length <= _costIndex)
        {
            return orderCostData;
        }
        string[] costArray = _parsingData[_costIndex].Split(FixedValue.PARSING_LIST_DIVIDE); //������ ����Ʈ�� �׸����� ����
        for (int i = 0; i < costArray.Length; i++)
        {
            TOrderItem orderItem = ParseOrderItem(costArray[i]);  //[0] : ��ūŸ�� [1] :�׸񿡼� pid [2] : ���� ���� �̷��� ���ڸ� ���� Torder�� ��ȯ.
            if (_noneDataAdd == true || orderItem.IsVaridTokenType())
            {
                orderCostData.Add(orderItem); //��ȿ�� ���� �߰�
            }
        }

        return orderCostData;
    }

    public static void ParseOrderItemList(List<TOrderItem> _itemList, string[] _parsingData, int _costIndex, bool _noneDataAdd = false)
    {
        //�Ľ� �� ���� ��°�� ���޽� 
        if (_parsingData.Length <= _costIndex)
        {
            return;
        }
        string[] costArray = _parsingData[_costIndex].Split(FixedValue.PARSING_LIST_DIVIDE); //������ ����Ʈ�� �׸����� ����
        for (int i = 0; i < costArray.Length; i++)
        {
            TOrderItem orderItem = ParseOrderItem(costArray[i]);  //[0] : ��ūŸ�� [1] :�׸񿡼� pid [2] : ���� ���� �̷��� ���ڸ� ���� Torder�� ��ȯ.
            if (_noneDataAdd == true || orderItem.IsVaridTokenType())
            {
                _itemList.Add(orderItem); //��ȿ�� ���� �߰�
            }
        }

    }

    public static void ParseIntList(List<int> _itemList, string[] _parsingData, int _costIndex, bool _noneDataAdd = false)
    {
        //�Ľ� �� ���� ��°�� ���޽� 
        if (_parsingData.Length <= _costIndex)
        {
            return;
        }
        string[] costArray = _parsingData[_costIndex].Split(FixedValue.PARSING_LIST_DIVIDE); //������ ����Ʈ�� �׸����� ����
        for (int i = 0; i < costArray.Length; i++)
        {
            if(int.TryParse(costArray[i], out int parseInt))
                _itemList.Add(parseInt);
        }

    }

    public static void ParseOrderItemList(List<TOrderItem> _itemList, string _parsingStrData, bool _noneDataAdd = false)
    {
       // �ش� ���� ���� ���޽�
        string[] costArray = _parsingStrData.Split(FixedValue.PARSING_LIST_DIVIDE); //������ ����Ʈ�� �׸����� ����
        for (int i = 0; i < costArray.Length; i++)
        {
            TOrderItem orderItem = ParseOrderItem(costArray[i]);  //[0] : ��ūŸ�� [1] :�׸񿡼� pid [2] : ���� ���� �̷��� ���ڸ� ���� Torder�� ��ȯ.
            if (_noneDataAdd == true || orderItem.IsVaridTokenType()) //�͵����Ͷ� �߰� ��û�߰ų� ��ȿ�� �Ÿ� �߰� 
            {
                _itemList.Add(orderItem); //��ȿ�� ���� �߰�
            }
        }

    }
   
    public static TOrderItem ParseOrderItem(string costData)
    {
        //��ū�׷�_pid_���� �� string���� �Ѿ�� �ڽ�Ʈ ������
        string[] divideType = costData.Split(FixedValue.PARSING_TYPE_DIVIDE);
        TOrderItem noneData = new TOrderItem(TokenType.None, 0, 0);
        //[0] : ��ūŸ�� [1] :�׸񿡼� pid [2] : ����
        if(divideType.Length != 3)
        {
            //�԰ݿ� ���� �ʴ� ��쿣 ���� 
            return noneData;
        }

        if (System.Enum.IsDefined(typeof(TokenType), divideType[0]))
        {
            //����� ��ū�׷��� �Ľ�.
            TokenType tokenType = System.Enum.Parse<TokenType>(divideType[0]);

            //�ι�° pid�� ���ڷ� ���ִ��� �ش� enum�� ���ڰ����� �Ǿ��ִ��� Ȯ��
            if (int.TryParse(divideType[1], out int enumIndex) == true)
            {
                //�ٷ� pid�� �Ǿ��ִٸ�
                if (int.TryParse(divideType[2], out int needAmount) == true)
                {
                    //������ �������� �� ���������� �����Ϳ� �߰�
                    return new TOrderItem(tokenType, enumIndex, needAmount);
                }
                //������ �̻��ϸ� �͵����� ����
                return noneData;
            }

            //enum�� ���ڰ����� ��ϵǾ������� index�� ����
            System.Type findEnum = FindEnum(tokenType);

            //��ū Ÿ�Կ� ���� ������ enum Ÿ���� �����ϰ�
            if (findEnum == null)
            {
                //������ enum �׷��� ������ �ѱ�
                return noneData;
            }
            //�ش� enumŸ�Կ��� [1] ���� �ִ��� Ȯ��
            if (System.Enum.IsDefined(findEnum, divideType[1]))
            {
                //[1] ���� pid�Ľ� 
                int enumPid = (int)System.Enum.Parse(findEnum, (divideType[1]));
                if (int.TryParse(divideType[2], out int needAmount) == true)
                {
                    //������ �������� �� ���������� �����Ϳ� �߰�
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

    #region Token ���� �ε�
    //�������� Class ������ �ʿ��� ����Ƽ �ڷḦ �����ϴ°�, �� �ڷ���� �����۸� + icon, prefeb �������� ����. 
    public static Sprite GetIconFromResource(TokenBase _token, string _resourcePath = null)
    {
        //Ŭ������ + Icon ���� ���̹����ؼ� �ҷ�����, sprite�� �غ����������쿡��, testIcon���� ��ü�ؼ� �����ϰ�, �̸��� �ٲ㼭, json���� �̺�� �κ��� Ȯ���� �� �ֵ��� �ϱ�
        string iconName = _token.GetItemName()+" Icon";
        Sprite sprite = Resources.Load<Sprite>(_resourcePath + iconName);
        
        return sprite;
    }

    public static GameObject GetPrefebFromResource(TokenBase _token, string _resourcePath = null)
    {
        //���ҽ� �������� ���� ������ ���� ��Ʈ�� ���������� �ش� ������ ��Ʈ�� �Է�
        //�ϴ��� CropPrefeb, SkillPrefeb �� ������ ���� �ϰ� ���� DB�� �����ϴ� DbItem.cs�� DbSkill.cs ���� �����Ҷ� �Է��ϵ��� ����. 
        string prefabName = _token.GetItemName() + " Prefeb";
        GameObject load = Resources.Load<GameObject>(_resourcePath + prefabName);
        return load;
    }
    #endregion

    public static bool IsInMainChar(TokenTile _tile)
    {
        //�ش� Ÿ�Ͽ� �����ɸ��� �ִ��� üũ 

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
        //�������� �������� ���� count��ŭ �̱�
        List<int> randomList = new();
        List<int> rangeList = new();
        if (exceptList == null)
            exceptList = new();
        for (int i = 0; i < _length; i++)
        {
            //���� ����Ʈ�� �ִ� ���ڴ� ���� ���� 
            if (exceptList.IndexOf(i) >= 0)
                continue;
            rangeList.Add(i); //���� ��ŭ ���� �ֱ�
        }

        for (int i = 1; i <= _randomCount; i++)
        {
            int ranIndex = UnityEngine.Random.Range(0, rangeList.Count);
            int randomNum = rangeList[ranIndex]; //���� ����Ʈ�� �ε����� ���ڸ� ����
            randomList.Add(randomNum); //��������Ʈ�� �ְ�
            rangeList.RemoveAt(ranIndex); //��������Ʈ�� �ش� �ε��� ������ ���� 
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
        Vector3 dropPosition = tile.GetObject().gameObject.transform.position; // ��ġ �̰�
        MagnetItem magnetItem = MonoBehaviour.Instantiate(GamePlayMaster.GetInstance().testMangetSample);
        magnetItem.SetMagnetInfo(dropPosition);
    }

    private static void MakeFence(Chunk _fenceChunk)
    {
        Sprite fenceSprite = TempSpriteBox.GetInstance().GetTileSprite(TileType.Nomal);

        int xLength = _fenceChunk.tiles.GetLength(0);
        int yLength = _fenceChunk.tiles.GetLength(1);

        //�ܰ��ΰ�츸 ��������Ʈ �ٲٱ�

        for (int x = 0; x < xLength; x++)
        {
            for (int y = 0; y < yLength; y++)
            {
                if (x == 0 || x == xLength - 1)
                {
                    //x���� 0�̰ų� �� ���ΰ�� y 0~max �޸���
                    _fenceChunk.tiles[x, y].SetSprite(fenceSprite);
                }
                else
                {
                    //x���� 1~��� ������ ��쿣 y ó���� ���� ��ĥ�ϰ� �ش� ���� �н� 
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