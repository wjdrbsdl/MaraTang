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
        //�ڿ� �й� ����׿�
        int[] resourceMain = new int[GameUtil.EnumLength(TokenTile.MainResource.House)];
        for (int curx = 0; curx < orderXLength; curx++)
        {
            float originXPos = curx * xOffSet; //����
            float reviseXpos = originXPos + reviseOffSet;//Ȧ�� ��° y���� ���������� ����ġ
            for (int cury = 0; cury < orderYLength; cury++)
            {
                float yPos = cury * yOffSet;
                
                bool needRevise = cury % 2 == 1; //Ȧ����°�� ������ �ʿ��ϴ�
                float finalX = (needRevise == true)? reviseXpos : originXPos;

                //1. Ÿ����ū, ������ū, Ÿ�Ͽ�����Ʈ ����
                TokenTile newTile = new TokenTile().MakeTileToken();
                HideTile newHideTile = Instantiate(_mapOrder.t_hideTile).GetComponent<HideTile>();
                ObjectTokenBase newTileObject = Instantiate(_mapOrder.t_tile).GetComponent<ObjectTokenBase>();
                
                //2. Ÿ�� ��ū ���� ����                
                newTile.SetMapIndex(curx, cury); //��ū ��ü�� �ڽ��� �ε��� �ְ�
                newTile.SetEcoValue(noisedMapping[curx, cury]);
                newTile.SetResourceValue();

                //3. Ÿ�� ������Ʈ ����
                newTileObject.SetObjectToken(newTile, TokenType.Tile);
                newTileObject.transform.SetParent(_mapOrder.t_box);
                newTileObject.transform.localPosition = new Vector2(finalX, yPos); //�ڽ� �ȿ��� �������������� ��ġ 
                newTile.SetEcoSprite();

                //4. ���� Ÿ�� ����
                newHideTile.transform.SetParent(_mapOrder.t_hideBox);
                newHideTile.transform.localPosition = new Vector2(finalX, yPos); //�ڽ� �ȿ��� �������������� ��ġ 
                newHideTile.SetTileSprite();

                //5. ���� �Ҵ�
                newMap[curx, cury] = newTile; //�� �迭�� �ε����� ������� ���� �Ҵ�
                newHideMap[curx, cury] = newHideTile; //�� �迭�� �ε����� ������� ���� �Ҵ�

                //6. ����׿� ���� ���ҽ� ��ġ �߰�
                int mainResourceIdx = newTile.GetStat(TileStat.MainResource);
                resourceMain[mainResourceIdx] += 1;
            }
        }
        MgToken.GetInstance().SetMapTiles(newMap); //������� �� ���� ����
        MgToken.GetInstance().SetHideTiles(newHideMap); //������� �� ���� ����

        //�����
        //for (int i = 0; i < resourceMain.Length; i++)
        //{
        //    Debug.Log((TokenTile.MainResource)i + "�� �� Ÿ�� �� :" + resourceMain[i]);
        //}
    }

    public List<int[]> DivideChunk(int _chunkLength)
    {
        //_length ���� �������� Ÿ���� ��� ûũȭ -> �ش� ûũ���� �̺�Ʈ �߻��̳� ���±ǵ� © �� �ִ� ��ȹ�� ��. 
        List<int[]> divided = new(); //int4 = x���� x�Ÿ� y���� y�Ÿ�
        int xLength = GameUtil.GetMapLength(true);
        int yLength = GameUtil.GetMapLength(false);

        //������ ����Ʈ�� üũ - �ش� ���������� ���μ��� _length�� �ش� ûũ�� �ִ� Ÿ����ǥ��
    
        for (int x = 0; x < xLength; x+=_chunkLength)
        {
            int xCode = x;
            int xWidth = _chunkLength;
            //��Ʈ�Ӹ��� �����ߴٸ� �Ÿ��� 1~_length ���̷� ����
            if (xCode + xWidth >= xLength)
            {
                xWidth = xLength - xCode;
            }

            for (int y = 0; y < yLength; y+=_chunkLength)
            {
                int yCode = y;
                int yWidth = _chunkLength;
                if (yCode + yWidth >= yLength)
                {
                    yWidth = yLength - yCode;
                }
                divided.Add(new int[] { xCode, xWidth, yCode, yWidth });
     
            }
        }
      
        return divided;
    }

    public List<Chunk> MakeChunk(List<int[]> _chunkedRecipe)
    {
        List<Chunk> ChunkLIst = new();

        for (int i = 0; i < _chunkedRecipe.Count; i++)
        {
            
            int startX = _chunkedRecipe[i][0];
            int widthX = _chunkedRecipe[i][1];
            int startY = _chunkedRecipe[i][2];
            int widthY = _chunkedRecipe[i][3];
            TokenTile[,] chunkedTile = new TokenTile[widthX, widthY];
            for (int x = 0; x < widthX; x++)
            {
                for (int y = 0; y < widthY; y++)
                {
                    TokenTile tile = GameUtil.GetTileTokenFromMap(new int[] {startX+x, startY+y });
                    tile.ChunkNum = i;
                    chunkedTile[x, y] = tile;
                }
            }
            Chunk newChunk = new Chunk(chunkedTile, i);
            ChunkLIst.Add(newChunk);
        }


        return ChunkLIst;
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


                GameObject newTile = Instantiate(_mapOrder.t_tile);
                newTile.transform.SetParent(_mapOrder.t_box);
                newTile.transform.localPosition = new Vector2(xPos, yPos); //�ڽ� �ȿ��� �������������� ��ġ 
            }
        }
    }
    #endregion
}

