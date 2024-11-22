using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkOrderExcutor
{
    //작업 타입에 따른 집행
    //작업 타입에 따른 선행조건등을 따져주는곳

    public void Excute(WorkOrder _order)
    {
        WorkType workType = _order.workType;
        int wokrPid = _order.WorkPid; //작업류에 따라 Pid의 대상이 달라짐
        switch (workType)
        {
            case WorkType.ChangeBuild:
                TokenTile workTile = GameUtil.GetTileTokenFromMap(_order.WorkPlacePos);
                workTile.ChangePlace((TileType)wokrPid);
                break;
        }
    }
}
