using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileDirection
{
    RightUp, Right, RightDown, LeftDown, Left, LeftUp
}

public static class GameUtil 
{
    #region Ÿ�� ���
    public static List<int[]> GetTileIdxListInRange(int _range, int _centerX, int _centerY)
    {
        //   _1 _2
        //  /6    |3  ������ ��ŸƮ�� �ش� ��Ÿ� _range �ܰ踸ŭ TileDirection enum �������� �����ϸ鼭 ����� ��ǥ ����
        //   |5  /4

        List<int[]> rangedTile = new List<int[]>();

        int[] startPoint = {_centerX , _centerY}; //�����̵Ǵ� �������� ���ͷ� ����
        //string resultLog = "";
        //��ĭ�� �ֺ��� ���鼭 ���
        for(int range = 1; range <= _range; range++)
        {
            //�ֺ��� ���� 6 ���� - TileDirection���� ��Ī -> RightUp�������� ���� �����ϹǷ�, -1(���Ƿ��� ������ ��Ÿ�)�� �������� ����
            startPoint[0] = _centerX - range;
            for (int x = 0; x <= 5; x++)
            {
                //�ش� �������� ���ư��� Ƚ���� ���� ���캸�� range
                for (int i = 1; i <= range; i++) //6�������� ��
                {
                   // Debug.Log((TileDirection)x + "�������� " + i + "�� ����");
                   //1. Ÿ�Ͽ��� Ư���������� ���� ��� ��ǥ�� ����
                    int[] newTile = GetTileFromDirect(startPoint[1], (TileDirection)x); 

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

    public static List<TokenTile> GetTileTokenListInRange(int _range, int _centerX, int _centerY)
    {
       return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY).ConvertAll(new System.Converter<int[], TokenTile>(GetTileTokenFromMap)); // ��Ÿ� ���� ���� Ÿ�� ��������
    }

    public static List<HideTile> GetHideTileListInRange(int _range, int _centerX, int _centerY)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY).ConvertAll(new System.Converter<int[], HideTile>(GetHideTileFromMap)); // ��Ÿ� ���� ���� Ÿ�� ��������
    }


    public static List<ObjectTokenBase> GetTokenObjectInRange(int _range, int _centerX, int _centerY)
    {
        return GameUtil.GetTileIdxListInRange(_range, _centerX, _centerY).ConvertAll(new System.Converter<int[], ObjectTokenBase>(GetTokenObjectFromMap)); // ��Ÿ� ���� ���� Ÿ�� ��������
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
        int random = Random.Range(0, 2); //�ΰ��� ������ ������ ��� ��� ������ �̾Ƴ���
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

    /*
    ���弼�ÿ� ������ �ɼ� �ְ� �������¹�
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

    #region ���̺� enum �����
    static string[]  originEnumString = { "Plus", "cheicken", "Scoop", "Vio" }; //���� ���̺��� �о��ٰ� ����
    static int[] originEnumValues = { 10, 20, 30, 25 };
    static int[] changeEnumValues; //���� ������ ��ü �Ӽ��� 
    static int[] newIndex;
    enum NewMenu
    {
        Valong, Vio, Sionic, Plus
    }

    public static void LoadTable()
    {
        //����� ���� �ҷ����� �κ� 
        string[] loadEnumStr = LoadNameValue(); //�ҷ����� ���� �ش� enum key�� string����
                                                //� ��ū �ҷ��Գ� ���� Ÿ�� ���� ���� �ߴ� ġ�� 
                                                //���� ���̺��� ���� ���̺� �´� ����ǥ�� ���� ���� Vio��, ���� ���̺��� ���°����, ������ maxValue�� ó��
        newIndex = ToStringFromEnum(NewMenu.Plus, loadEnumStr, originEnumValues);

    }

    private static string[] LoadNameValue()
    {
        //enum name������ ������� ���̺��� ����enum���� ��������
        //�ش� ���� table ������ �������� ��𼭵� �����ͼ� string[] �� �����ؼ� ��ȯ
        return originEnumString;
    }

    //�ش� �ּ��� ���� �ε����� ¥�� - ���� �̿�
    private static int[] ToStringFromEnum(System.Enum _curType, string[] _origin, int[] _oriValue)
    {
        Debug.Log(_curType + " , " + _curType.GetType().ToString());
        //enum ���� �ϳ��� ������ �ش� enum ��� �� �ľ�
        string[] curNames = System.Enum.GetNames(_curType.GetType());
        //���� ���̺��� ���� ���̺� �´� ����ǥ�� ����
        int[] loadIndex = new int[curNames.Length];
        for (int curIdx = 0; curIdx < curNames.Length; curIdx++)
        {
            //Debug.Log(curNames[curIdx]);
            string curId = curNames[curIdx];
            loadIndex[curIdx] = int.MaxValue;
            //bool isFind = false;
            for (int originIdx = 0; originIdx < _origin.Length; originIdx++)
            {
                //�ش� ���̵� �������� ���� �ִ��� üũ
                string originId = _origin[originIdx];
                if (originId.Equals(curId))
                {
                    //���� enumIdx ��� ���� ���� �Ľ�
                    loadIndex[curIdx] = _oriValue[originIdx];
                    //isFind = true;
                    break; //�̹� ������ ��
                }
            }
            //if(isFind == false)
            //Debug.Log(curId + "�� ���� �� ���� enum ��");
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