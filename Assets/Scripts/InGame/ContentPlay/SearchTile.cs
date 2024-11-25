using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SearchTile
{
    public void FindSomething(TokenTile _tile)
    {
        //타일에서 무언갈 찾아서 반환 혹은 플레이어 재산에 포함시키는 곳 
        Debug.Log("타일에서 뭔가좀 찾아봄");

    }


    private void HarvestTile(TokenTile _tile)
    {
        List<(Capital, int)> mineResult = GamePlayMaster.GetInstance().RuleBook.MineResource(_tile).GetResourceAmount();
        for (int i = 0; i < mineResult.Count; i++)
        {
            //  Debug.Log(mineResult[i].Item1 + " 자원 채취" + mineResult[i].Item2);
            PlayerCapitalData.g_instance.CalCapital(mineResult[i].Item1, mineResult[i].Item2);
        }
    }

}
