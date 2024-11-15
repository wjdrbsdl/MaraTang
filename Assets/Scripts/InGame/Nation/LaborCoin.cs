using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LaborCoin
{
    public int ListIndex = 0; //할당된 pid
    public int[] Pos; //할당된 일자리
    public int NationNum;

    public LaborCoin(int _pid, Nation _nation)
    {
        ListIndex = _pid;
        NationNum = _nation.GetNationNum();
        SetPos( _nation.GetCapital().GetMapIndex()); //생성시 수도에 노동코인 저장
    }

    public void GoWork(TokenTile _tile)
    {
        SetPos(_tile.GetMapIndex());
    }

    public void BackCapital()
    {
        SetPos(MgNation.GetInstance().GetNation(NationNum).GetCapital().GetMapIndex());
    }


    private void SetPos(int[] _pos)
    {
        int[] prePos = Pos;
        GameUtil.GetTileTokenFromMap(prePos).TakeOutLaborCoin(this);
        Pos = _pos;
        //해당 토큰타일에 넣는걸로
        GameUtil.GetTileTokenFromMap(_pos).PutInLaborCoin(this);
    }
}
