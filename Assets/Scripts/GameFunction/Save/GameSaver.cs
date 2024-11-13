using UnityEngine;
using System.Collections.Generic;

public class GameSaver
{
    // 1. 타일 토큰들 - 상태 위치 작업서 상황
    // -> 그 안에 캐릭토큰들

    //+ 국가 구성 타일, 국가 스텟, 국가 상황
    //+ 컨텐츠 진행 상황
    //+ 플레이어 상황
    
    public void SaveGame()
    {
        Debug.Log("게임 세입");
        SaveTiles();
        SaveChars();
    }

    private void SaveTiles()
    {
        TokenTile[,] tils = MgToken.GetInstance().GetMaps();
        List<TokenTile> list = new();
        int cul = tils.GetLength(0);
        int row = tils.GetLength(1);
        for (int i = 0; i < row; i++)
        {
            for (int x = 0; x < cul; x++)
            {
                list.Add(tils[x, i]);
            }

        }
        TokenTile[] saveTils = list.ToArray();
        DBToJson.SaveTileToken(saveTils, row, cul, GameLoad.Load);
    }

    private void SaveChars()
    {
        TokenChar[] chars = MgToken.GetInstance().GetCharList().ToArray();
        DBToJson.SaveCharToken(chars, GameLoad.Load);
    }
}
