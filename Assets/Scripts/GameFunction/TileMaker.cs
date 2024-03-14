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
        //y 간의 간격은 반지름 *1.5
        float yOffSet = tileRadius * 1.5f;
        //x 간의 간격은 정육각형이므로 정삼각형의 높이(반지름 * 루트3/2) * 2 ; 
        float xOffSet = tileRadius * Mathf.Sqrt(3);
        //탑 포인트 일 때는 홀수번째의 행의 너비가 반지름씩 옆으로 가있음
        float reviseOffSet = xOffSet * 0.5f;

        float[,] noisedMapping = MakeNoise(orderXLength, orderYLength, _mapOrder.t_seed, _mapOrder.t_noise, 1f);

        for (int curx = 0; curx < orderXLength; curx++)
        {
            float originXPos = curx * xOffSet; //원점
            float reviseXpos = originXPos + reviseOffSet;//홀수 번째 y에선 오른쪽으로 보정치
            for (int cury = 0; cury < orderYLength; cury++)
            {
                float yPos = cury * yOffSet;
                
                bool needRevise = cury % 2 == 1; //홀수번째는 보정이 필요하다
                float finalX = (needRevise == true)? reviseXpos : originXPos;

                int selectMap = SelectTokenEco(noisedMapping[curx, cury]);

                //오브젝트용
                ObjectTokenBase newTileObject = Instantiate(_mapOrder.t_tile).GetComponent<ObjectTokenBase>();
                HideTile newHideTile = Instantiate(_mapOrder.t_hideTile).GetComponent<HideTile>();
                //타일토큰용
                TokenTile newTokeTileInfo = new TokenTile().MakeTileToken();


                //서로 참조 세팅
                newTileObject.SetToken(newTokeTileInfo, TokenType.Tile);
                newTokeTileInfo.SetMapIndex(curx, cury); //토큰 자체에 자신의 인덱스 넣고
                
                newMap[curx, cury] = newTokeTileInfo; //맵 배열의 인덱스엔 만들어진 맵을 할당
                newTileObject.transform.SetParent(_mapOrder.t_box);
                newTileObject.transform.localPosition = new Vector2(finalX, yPos); //박스 안에서 로컬포지션으로 위치 

                newHideMap[curx, cury] = newHideTile; //맵 배열의 인덱스엔 만들어진 맵을 할당
                newHideTile.transform.SetParent(_mapOrder.t_hideBox);
                newHideTile.transform.localPosition = new Vector2(finalX, yPos); //박스 안에서 로컬포지션으로 위치 
                newHideTile.SetTileSprite();

                //환경 자원 Object 생성
                //if (selectMap.Equals(0))
                //    ObjTokenManager.GetInstance().RequestObjectToken(newTokeTileInfo);
            }
        }
        MgToken.GetInstance().SetMapTiles(newMap); //만들어진 맵 정보 전달
        MgToken.GetInstance().SetHideTiles(newHideMap); //만들어진 맵 정보 전달
    }

    public List<int[]> DivideChunk(int _chunkLength)
    {
        //_length 길이 묶음으로 타일을 묶어서 청크화 -> 해당 청크별로 이벤트 발생이나 세력권등 짤 수 있는 구획이 됨. 
        List<int[]> divided = new(); //int4 = x시작 x거리 y시작 y거리
        int xLength = GameUtil.GetMapLength(true);
        int yLength = GameUtil.GetMapLength(false);

        //시작점 포인트를 체크 - 해당 시작점에서 가로세로 _length가 해당 청크에 있는 타일좌표들
    
        for (int x = 0; x < xLength; x+=_chunkLength)
        {
            int xCode = x;
            int xWidth = _chunkLength;
            //끄트머리에 도달했다면 거리를 1~_length 사이로 조정
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

    //노이즈 방식의 절차적 생성
    private float[,] MakeNoise(int _xLength, int _yLength, int _seed, float _noise, float _height)
    {
        //index 맵 크기는 해당 맵마다 정해져있는 부분 
        //그 맵의 해당 좌표에 어떤 값이 들어갈지를 구해야함. 
        //이때 맵 크기의 변화는 고정이고 
        //노이즈들로 형태를 구별? 
        //seed는 전체적인 
        
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
        
        //최저 최저 높이 기준으로 해당 노이즈값을 보간.
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



    #region 평맵핑
    private void MakeFlatTypeMap(MgToken.TMapBluePrint _mapOrder)
    {

        int orderXLength = _mapOrder.t_xLength;
        int orderYLength = _mapOrder.t_yLength;
        float tileRadius = _mapOrder.t_rLength;

        //플랫 다운 형식
        //x 간의 간격은 반지름 *1.5
        float xOffSet = tileRadius * 1.5f;
        //y 간의 간격은 정육각형이므로 정삼각형의 높이(반지름 * 루트3/2) * 2 ; 
        float yOffSet = tileRadius * Mathf.Sqrt(3);
        //플랫 다운일 때는 홀수번째의 높이가 반지름씩 올라가있음
        float reviseYOffSet = yOffSet*0.5f;

        for (int curx = 0; curx < orderXLength; curx++)
        {
            bool needRevise = curx % 2 == 1; //홀수번째는 보정이 필요하다
            float xPos = curx * xOffSet; //원점 + x보정
            for (int cury = 0; cury < orderYLength; cury++)
            {
                float yPos = cury * yOffSet;
                if (needRevise)
                    yPos += reviseYOffSet; //보정 필요시 수치 더함


                GameObject newTile = Instantiate(_mapOrder.t_tile);
                newTile.transform.SetParent(_mapOrder.t_box);
                newTile.transform.localPosition = new Vector2(xPos, yPos); //박스 안에서 로컬포지션으로 위치 
            }
        }
    }
    #endregion
}

