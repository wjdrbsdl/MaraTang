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
    public static List<TokenTile> GetMinRoute(int _range, int _centerX, int _centerY, int _targetX, int _targetY, int _stopDistance = 0)
    {
        //���� ������ ��ǥ�� ������ �ּ� ��Ʈ�� ���� ���� ���� �� 
        List<TokenTile> routeTileList = new List<TokenTile>();

        //���� �ɸ���, Ÿ�ٱ��� �̵��� ��Ʈ�� tokenTile�� ã��, �׼� ��ū�� ����. 

        int tempMoveCount = _range; //�̵� ��Ʈ �Ÿ� 
        //�����
        int x = _centerX;
        int y = _centerY;
        //������
        int toX = _targetX;
        int toY = _targetY; 
        //������ ���� ���� �Ÿ�
        int stopDistance = _stopDistance;

        TokenTile[,] maps = MgToken.g_instance.GetMaps();
        TMapIndex mapInfoes = new TMapIndex(x, y, toX, toY);

        for (int i = 1; i <= tempMoveCount; i++)
        {
            //1. ��� ã���� üũ
            if (GameUtil.GetMinRange(mapInfoes) <= stopDistance)//���� ��ġ���� ���������� ��Ÿ��� ������ϴ� �Ÿ� �̳���� ��Ʈ ã�� ���� 
            {
                break;
            }

            //2. �ִܰ�η� ������ ������ �̰�
            TileDirection nextDirect = GameUtil.GetNextTileToGoal(mapInfoes);

            //3. ���� ��ǥ���� �ش� ��η� �̵��� ��� ��ǥ�� ����
            int[] gapValue = GameUtil.GetTileFromDirect(mapInfoes.curY, nextDirect);
            //4. ���� ��ǥ�� ��� ��ǥ�� ���ؼ� �̵��� ��ǥ�� ���
            mapInfoes.curX += gapValue[0];
            mapInfoes.curY += gapValue[1];
            //5. �̵��� ��ǥ�� �̵��Ұ� Ȥ�� �������� ���� ��ǥ�� �ѱ�
            if (GameUtil.IsThereMap(mapInfoes.curX, mapInfoes.curY) == false)
            {
                //���� ���� �� ��ġ�� ������� ������ �ٽ� ���� �̱�� ���� (AI�� moveCount ��ȸ��ŭ �ùٸ� ���� ã�� ��ȸ�� �ִ°�) 
                mapInfoes.curX -= gapValue[0];
                mapInfoes.curY -= gapValue[1];
                continue;
            }
            //6. �ùٸ� ��ǥ��� �̵�Ȯ�� �ϰ� �ش� ��ūŸ���� �߰� 
            //�޸𸮻����� ���� ��ü �������ʿ���� �׳� maps�� �ִ� Ŭ������ �־ �ǳ�? ������ �������ΰ�?
            TokenTile targetMap = maps[mapInfoes.curX, mapInfoes.curY];
            routeTileList.Add(targetMap); ;
        }



        return routeTileList; 
    }

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
        TileDirection direction = TileDirection.RightUp;

        if ((_tMapIndex.curY - _tMapIndex.toY) == 0)
        {
            //���� ���̿� ������ �ִܰ�� ������ �¿� �� �ϳ�
            if ((_tMapIndex.toX - _tMapIndex.curX) > 0)
                return TileDirection.Right;
            else
                return TileDirection.Left;
        }

        //1. ���� ������, to ���̱��� ������� ������ ���� x ��
        int lineX = (int)((_tMapIndex.toY - _tMapIndex.curY) *0.5f); //���� / 2. �� ���̰� 2���������� �⺻������ x�� 1 ���. 
        if (_tMapIndex.curY % 2 == 1 && (_tMapIndex.toY - _tMapIndex.curY)%2 == 1)
        {
            //���� ���۰� ���� ������ �ٸ��ٸ�, �߰��� +1 �ؾ��ϴ� �������� �˾ƾ���.
            //���̰� Ȧ������ ¦���� ���� x�� 1�� ����. 
            lineX += 1;
        }
        lineX += _tMapIndex.curX; //��� x�� ���ؼ� ���� x��ǥ ����. 

        //2. �밢�� ������ x�� ���� toX�� ��ġ�� ��������� ���� �̵������� ���� ����
        int gap = _tMapIndex.toX - lineX;
        int directX = 0; //���� ���� ǥ���� ������
        int directY = 0; //���� ���� ǥ���� ������
        int randomIndex = Random.Range(0,2); //0,1 �����ϳ� �̱�
        if (gap < 0)
        {
            //toX�� �����̸� �����ִ� ������ �»�, ����� �ϳ�
            if(randomIndex == 0)
            {
                //���Ƿ� 0�ΰ�� �»����� ����
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
            //�밢 ���ο� toX�� ��ģ�ٸ� ������ ����ؾ� �ִܰ�ΰ� ���� 
            directX = 1;
            directY = 1;
        }
        else
        {
            //toX�� �������̸� �����ִ� ������ ���, ������ �� �ϳ�
            if (randomIndex == 0)
            {
                //���Ƿ� 0�ΰ�� ������� ����
                directX = 1;
                directY = 1;
            }
            else
            {
                directX = 1;
            }
        }

        //3. �ٽ� ������ ���� ������ ����
        if((_tMapIndex.toX - _tMapIndex.curX) < 0)
        {
            //������ ������ �����̾��ٸ�
            directX *= -1; //x ������ ����
        }
        if ((_tMapIndex.toY - _tMapIndex.curY) < 0)
        {
            //������ ������ �Ʒ����ٸ�
            directY *= -1; //y ���⵵ ����
        }

        //5. ���� direct ���� ���� ���� ���� 
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
        //���� ����Ʈ ���� ��ǥ���� ���� �������� ������Ű�� 
        moveX = Mathf.Abs(_centX - _targetX);
        moveY = Mathf.Abs(_centY - _targetY);

        bool isAnotherLine = (_centY + _targetY) % 2 == 1; //���� �������� Ȧ���� ���� �ٸ� ��
        if (isAnotherLine && 0 < (_centX - _targetX)) //�ٸ������ε� Ÿ���� ���� ���鿡 �ִٸ�, ����ȭ �Ҷ� x ��ǥ����-1 �������. 
            moveX -= 1;
    }
    #endregion

    public static void CharacterMove(TokenChar _char, TokenTile _target, int range = 3)
    {
        int x= _char.GetXIndex(), y=_char.GetYIndex(), toX= _target.GetXIndex(), toY=_target.GetYIndex(), r=range; //�ʿ��� �������� �ӽ�

        TMapIndex toTest = new TMapIndex(x, y, toX, toY);
        for (int i = 0; i < r; i++)
        {
            TileDirection nextDirect = GameUtil.GetNextTileToGoal(toTest); //�ִܰ�η� ���� �ִ� ���� �̰�

            //�׸��� ������ �� �������� �̵��� ���� ��ǥ�� ����
            int[] gapValue = GameUtil.GetTileFromDirect(toTest.curY, nextDirect);
            //����� ��ǥ�� �����ǥ�� ���� ��ǥ�� ���ؾ� �̵��� ��ǥ�� ��
            toTest.curX += gapValue[0];
            toTest.curY += gapValue[1];
            Debug.Log("������ ����" + nextDirect + "�̵��� ��ǥ " + toTest.curX + ", " + toTest.curY);
            //��ǥ�� �����ؼ� �ٽ� ���� 

        }
    }

    /*
    ���弼�ÿ� ������ �ɼ� �ְ� �������¹�
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