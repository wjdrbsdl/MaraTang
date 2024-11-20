using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TileDistance
{
    //국가 소유 장소를 사용할때 사거리는 국가 수도로부터 거리에 수도 기능 범위로 
    //그 밖에 타일의 사용가능 거리는 해당 타일위에 캐릭터 자체 스텟

    public int CapitalFromChar(TokenTile _tile)
    {
        Nation nation = _tile.GetNation();
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        //국가 소유 아니면 해당 타일과의 거리는 해당 타일부터 캐릭터까진걸로
        if (nation == null)
        {
            int range = GameUtil.GetMinRange(_tile, mainChar);
        //    Debug.Log("평지 용 거리" + range);

            return range;
        }
            

        TokenTile captial = nation.GetCapital();
        int captialrange = GameUtil.GetMinRange(captial, mainChar);
    //    Debug.Log("수도와 거리" + captialrange);

        return captialrange;
    }

    public int AdaptDistanceStat(TokenTile _tile)
    {
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        int adaptDistance = mainChar.GetStat(CharStat.Dex); //캐릭터 기능사용 거리를 보완용으로 받고

        Nation nation = _tile.GetNation(); //국가 받아서
        if (nation == null)
            return adaptDistance; //국가 없으면 캐릭으로 반환하고

        int nationBonus = nation.GetStat(NationStatEnum.Sight); //국가 있으면 국가 보정을 받아서

        return adaptDistance + nationBonus;
    }
    //해당 타일에선 가능한지 여부만 받으면되겠지

    public bool AbleDistance(TokenTile _tile)
    {
        //타일과 메인캐릭터 거리를 구하고
        int distance = CapitalFromChar(_tile);
        int bonusDistance = AdaptDistanceStat(_tile); //보정 거리 구하고
   //     Debug.Log("보정거리 " + bonusDistance);
        if (distance <= bonusDistance)
            return true;

        return false;
    }
}
