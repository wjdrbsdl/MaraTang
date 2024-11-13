using UnityEngine;
using System.Collections.Generic;

public class GameLoader
{
    // 1. 타일 토큰들 - 상태 위치 작업서 상황
    // -> 그 안에 캐릭토큰들

    //+ 국가 구성 타일, 국가 스텟, 국가 상황
    //+ 컨텐츠 진행 상황
    //+ 플레이어 상황
    
    public void LoadGame()
    {
        Debug.Log("게임 로드");
        LoadTileTokens(); //작업서 이슈
        LoadCharTokens();
        LoadPlayerMg(); //메인캐릭터를 다시 세팅하기 때문에 위에 케릭 로드를 먼저해야함. 
        LoadContentMg(); //생성된 토큰으로 청크 구역을 나누기때문에 타일보다 뒤에
    }

    private void LoadTileTokens()
    {
        TileTokenJson tileJson = DBToJson.LoadToJson<TileTokenJson>(JsonName.TileTokenJson, GameLoad.Load);
        int row = tileJson.rowCount;
        int cul = tileJson.culCount;
        TokenTile[,] tiles = new TokenTile[cul, row];

        int countRow = 0;
        int countCul = 0;
        for (int i = 0; i < tileJson.tileTokens.Length; i++)
        {
            TokenTile tile = tileJson.tileTokens[i];
            tiles[countCul, countRow] = tile;

            //열은 1씩 증가하면서 cul 주기마다 0으로 갱신
            countCul += 1;
            //행은 1씩 증가하면서 cul 주기마다 +1
            if (countCul == cul)
            {
                countCul = 0; // 초기화
                countRow += 1;
            }

        }
        MgToken.GetInstance().LoadTileToken(tiles);
    }

    private void LoadCharTokens()
    {
        CharTokenJson charJson = DBToJson.LoadToJson<CharTokenJson>(JsonName.CharTokenJson, GameLoad.Load);
        MgToken.GetInstance().LoadCharTokens(charJson.charTokens);
    }

    private void LoadPlayerMg()
    {
        PlayerManager.GetInstance().LoadPlayer();
    }

    private void LoadContentMg()
    {
        MGContent.GetInstance().LoadGame();
    }
}
