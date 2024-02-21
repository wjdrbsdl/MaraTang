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
                    int[] newTile = GetTileFromDirect(startPoint[1], (TileDirection)direction); 

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

    public static List<TokenTile> GetTileTokenListInRange(int _range, int _centerX, int _centerY, int _minRange = 0)
    {
       return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY, _minRange).ConvertAll(new System.Converter<int[], TokenTile>(GetTileTokenFromMap)); // ��Ÿ� ���� ���� Ÿ�� ��������
    }

    public static List<ObjectTokenBase> GetTokenObjectInRange(int _range, int _centerX, int _centerY, int _minRange = 0)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY, _minRange).ConvertAll(new System.Converter<int[], ObjectTokenBase>(GetTokenObjectFromMap)); // ��Ÿ� ���� ���� Ÿ�� ��������
    }

    public static int[] GetTileFromDirect(int _centerY, TileDirection _direction)
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

    //���������� �ִ� Ÿ�� ����
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

    #endregion

    #region ûũ ���
    public static int GetChunkNum(int[] _coordi)
    {
        return GetTileTokenFromMap(_coordi).ChunkNum;
    }
    #endregion

    /*
    ���弼�ÿ� ������ �ɼ� �ְ� �������¹�
        PlayerSettings.SetScriptingDefineSymbolsForGroup(ebuildTarget, m_Defines.ToString());
        string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
    */

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
        string[] enumCodes = ParseEnumStrings(_codeEnum);
        List<int[]> matchCodeList = new();
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
                yield break;

            }

            process?.Invoke(true, curWork, req.downloadHandler.text);
            curWork += 1;
        }

        doneAct?.Invoke(); //������ GameManager�� �۾� �Ϸ������� �˸�. 
    }
    #endregion

    #region Token ���� �ε�
    //�������� Class ������ �ʿ��� ����Ƽ �ڷḦ �����ϴ°�, �� �ڷ���� �����۸� + icon, prefeb �������� ����. 
    public static void SetIconFromResource(TokenBase _token, string _resourcePath = null)
    {
        //Ŭ������ + Icon ���� ���̹����ؼ� �ҷ�����, sprite�� �غ����������쿡��, testIcon���� ��ü�ؼ� �����ϰ�, �̸��� �ٲ㼭, json���� �̺�� �κ��� Ȯ���� �� �ֵ��� �ϱ�
        string iconName = _token.GetItemName()+" Icon";
        Sprite sprite = Resources.Load<Sprite>(_resourcePath + iconName);
        if (sprite == null)
        {
            iconName = "TestBear";
            _token.TokenImage = Resources.Load<Sprite>(iconName);
        }
    }

    public static void SetPrefebFromResource(TokenBase _token, string _resourcePath = null)
    {
        //���ҽ� �������� ���� ������ ���� ��Ʈ�� ���������� �ش� ������ ��Ʈ�� �Է�
        //�ϴ��� CropPrefeb, SkillPrefeb �� ������ ���� �ϰ� ���� DB�� �����ϴ� DbItem.cs�� DbSkill.cs ���� �����Ҷ� �Է��ϵ��� ����. 
        string prefabName = _token.GetItemName() + " Prefeb";
        GameObject load = Resources.Load<GameObject>(_resourcePath + prefabName);
        if (load == null)
        {
            prefabName = "TestAttack";
            _token.Prefab = Resources.Load<GameObject>(prefabName);
        }
    }
    #endregion

    public static void DropMagnetItem(int[] mapIndex)
    {
        TokenTile tile = GetTileTokenFromMap(mapIndex);
        Vector3 dropPosition = tile.GetObject().gameObject.transform.position; // ��ġ �̰�
        MagnetItem magnetItem = MonoBehaviour.Instantiate(GamePlayMaster.GetInstance().testMangetSample);
        magnetItem.SetMagnetInfo(dropPosition);
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