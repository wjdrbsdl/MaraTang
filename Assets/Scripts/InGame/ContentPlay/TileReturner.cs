using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileReturner
{
    public TokenTile NationBoundaryTile()
    {
        TokenTile tile = null;
        int playerInNationNum = GameUtil.GetTileTokenFromMap(PlayerManager.GetInstance().GetMainChar().GetMapIndex()).GetNationNum();
        if (playerInNationNum == FixedValue.NO_NATION_NUMBER)
            playerInNationNum = 0;
        Nation nation =  MgNation.GetInstance().GetNation(playerInNationNum); //
        List<TokenTile> nationTiles = nation.GetTerritorry();

        for (int i = 0; i < nationTiles.Count; i++)
        {
           List<TokenTile> roundTile = GameUtil.GetTileTokenListInRange(1, nationTiles[i].GetMapIndex(), 1);
            for (int r = 0; r < roundTile.Count; r++)
            {
                //주변 타일의 넘버가 같은 국가가 하나라도 국가가 아니면 얘는 외각
                int tileInRange = roundTile[r].GetStat(ETileStat.Nation);
                if(tileInRange == FixedValue.NO_NATION_NUMBER)
                {
                    tile = nationTiles[i];
                    return tile;
                }
            }
        }
        //국가 영역 중 경계선에 있는 타일 하나를 반환 
        //이를 토대로 국가 주변에 어떠한 것들을 스폰시킴. 
        return null;
    }
}
