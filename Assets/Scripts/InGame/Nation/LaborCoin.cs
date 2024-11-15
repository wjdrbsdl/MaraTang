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
        SetPos( _nation.GetCapital().GetMapIndex());
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
        Pos = _pos;
    }
}
