using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMaker : MonoBehaviour
{
    public void MakeTopTypeMap(MgToken.TMapBluePrint _mapOrder)
    {
        int orderXLength = _mapOrder.t_xLength;
        int orderYLength = _mapOrder.t_yLength;
        float tileRadius = _mapOrder.t_rLength;

        TokenTile[,] newMap = new TokenTile[orderXLength, orderYLength];
        HideTile[,] newHideMap = new HideTile[orderXLength, orderYLength];
        //y ���� ������ ������ *1.5
        float yOffSet = tileRadius * 1.5f;
        //x ���� ������ ���������̹Ƿ� ���ﰢ���� ����(������ * ��Ʈ3/2) * 2 ; 
        float xOffSet = tileRadius * Mathf.Sqrt(3);
        //ž ����Ʈ �� ���� Ȧ����°�� ���� �ʺ� �������� ������ ������
        float reviseOffSet = xOffSet * 0.5f;

        float[,] noisedMapping = MakeNoise(orderXLength, orderYLength, _mapOrder.t_seed, _mapOrder.t_noise, 1f);

        for (int curx = 0; curx < orderXLength; curx++)
        {
            float originXPos = curx * xOffSet; //����
            float reviseXpos = originXPos + reviseOffSet;//Ȧ�� ��° y���� ���������� ����ġ
            for (int cury = 0; cury < orderYLength; cury++)
            {
                float yPos = cury * yOffSet;
                
                bool needRevise = cury % 2 == 1; //Ȧ����°�� ������ �ʿ��ϴ�
                float finalX = (needRevise == true)? reviseXpos : originXPos;

                int selectMap = SelectTokenEco(noisedMapping[curx, cury]);

                //������Ʈ��
                ObjectTokenBase newTileObject = Instantiate(_mapOrder.t_tiles[selectMap]).GetComponent<ObjectTokenBase>();
                HideTile newHideTile = Instantiate(_mapOrder.t_hideTile).GetComponent<HideTile>();
                //Ÿ����ū��
                TokenTile newTokeTileInfo = new TokenTile().MakeTileToken();


                //���� ���� ����
                newTileObject.SetToken(newTokeTileInfo, TokenType.Tile);
                newTokeTileInfo.SetMapIndex(curx, cury); //��ū ��ü�� �ڽ��� �ε��� �ְ�
                
                newMap[curx, cury] = newTokeTileInfo; //�� �迭�� �ε����� ������� ���� �Ҵ�
                newTileObject.transform.SetParent(_mapOrder.t_box);
                newTileObject.transform.localPosition = new Vector2(finalX, yPos); //�ڽ� �ȿ��� �������������� ��ġ 

                newHideMap[curx, cury] = newHideTile; //�� �迭�� �ε����� ������� ���� �Ҵ�
                newHideTile.transform.SetParent(_mapOrder.t_hideBox);
                newHideTile.transform.localPosition = new Vector2(finalX, yPos); //�ڽ� �ȿ��� �������������� ��ġ 


                //ȯ�� �ڿ� Object ����
                if (selectMap.Equals(0))
                    ObjTokenManager.GetInstance().RequestObjectToken(newTokeTileInfo);
            }
        }
        MgToken.GetInstance().SetMapTiles(newMap); //������� �� ���� ����
        MgToken.GetInstance().SetHideTiles(newHideMap); //������� �� ���� ����
    }

    public List<int[]> DivideChunk(int _length)
    {
        //_length ���� �������� Ÿ���� ��� ûũȭ -> �ش� ûũ���� �̺�Ʈ �߻��̳� ���±ǵ� © �� �ִ� ��ȹ�� ��. 
        List<int[]> divided = new(); //
        int xLength = GameUtil.GetMapLength(true);
        int yLength = GameUtil.GetMapLength(false);
        
        //������ ����Ʈ�� üũ - �ش� ���������� ���μ��� _length�� �ش� ûũ�� �ִ� Ÿ����ǥ��
        for (int x = 0; x < xLength; x+=_length)
        {
            int xCode = x;
            for (int y = 0; y < yLength; y+=_length)
            {
                int yCode = y;
                divided.Add(new int[] { xCode, yCode });
                //���� ������ ��ȹ�̿��ٸ�
                if(yCode + _length >= yLength)
                {
                    if(yCode != yLength - 1) //�������� �� �߶����� ���� ���Ҵٸ�
                    {
                        divided.Add(new int[] { xCode, yLength - 1 }); //������ y �������� �߰�
                    }

                }
            }


        }
        return null;
    }

    //������ ����� ������ ����
    private float[,] MakeNoise(int _xLength, int _yLength, int _seed, float _noise, float _height)
    {
        //index �� ũ��� �ش� �ʸ��� �������ִ� �κ� 
        //�� ���� �ش� ��ǥ�� � ���� ������ ���ؾ���. 
        //�̶� �� ũ���� ��ȭ�� �����̰� 
        //�������� ���¸� ����? 
        //seed�� ��ü���� 
        
        float[,] box = new float[_xLength, _yLength];
        float min = 0;
        float max = 0;
        for (int xIndex = 0; xIndex < _xLength; xIndex++)
        {
            for (int yIndex = 0; yIndex < _yLength; yIndex++)
            {
                float calNoise = Mathf.PerlinNoise(_seed + xIndex * _noise, _seed + yIndex * _noise) * _height;

                if (max < calNoise)
                    max = calNoise;
                else if (min > calNoise)
                    min = calNoise;

                box[xIndex, yIndex] = calNoise;
            }
        }
        
        //���� ���� ���� �������� �ش� ������� ����.
        for (int xIndex = 0; xIndex < _xLength; xIndex++)
        {
            for (int yIndex = 0; yIndex < _yLength; yIndex++)
            {
                float calNoise = box[xIndex, yIndex];

                float inverseValue = Mathf.InverseLerp(min, max, calNoise);
                box[xIndex, yIndex] = inverseValue;
            }
        }

        return box;
    }

    private int SelectTokenEco(float _noiseValue)
    {
        int selectMap = 0;
        if (_noiseValue < 0.33f)
            selectMap = 0;
        else if (_noiseValue < 0.66f)
            selectMap = 1;
        else
            selectMap = 2;

        return selectMap;
    }



    #region �����
    private void MakeFlatTypeMap(MgToken.TMapBluePrint _mapOrder)
    {

        int orderXLength = _mapOrder.t_xLength;
        int orderYLength = _mapOrder.t_yLength;
        float tileRadius = _mapOrder.t_rLength;

        //�÷� �ٿ� ����
        //x ���� ������ ������ *1.5
        float xOffSet = tileRadius * 1.5f;
        //y ���� ������ ���������̹Ƿ� ���ﰢ���� ����(������ * ��Ʈ3/2) * 2 ; 
        float yOffSet = tileRadius * Mathf.Sqrt(3);
        //�÷� �ٿ��� ���� Ȧ����°�� ���̰� �������� �ö�����
        float reviseYOffSet = yOffSet*0.5f;

        for (int curx = 0; curx < orderXLength; curx++)
        {
            bool needRevise = curx % 2 == 1; //Ȧ����°�� ������ �ʿ��ϴ�
            float xPos = curx * xOffSet; //���� + x����
            for (int cury = 0; cury < orderYLength; cury++)
            {
                float yPos = cury * yOffSet;
                if (needRevise)
                    yPos += reviseYOffSet; //���� �ʿ�� ��ġ ����

                int randomIdx = Random.Range(0, _mapOrder.t_tiles.Length);
                GameObject newTile = Instantiate(_mapOrder.t_tiles[randomIdx]);
                newTile.transform.SetParent(_mapOrder.t_box);
                newTile.transform.localPosition = new Vector2(xPos, yPos); //�ڽ� �ȿ��� �������������� ��ġ 
            }
        }
    }
    #endregion
}
