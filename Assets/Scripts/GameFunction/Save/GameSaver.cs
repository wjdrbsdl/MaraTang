using UnityEngine;


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
        TokenTile[,] tils = MgToken.GetInstance().GetMaps();
        DBToJson.SaveTileToken((TokenTile[])tils.GetValue(0), GameLoad.Load);
        
    }
}
