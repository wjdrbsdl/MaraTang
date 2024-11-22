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
        int workPid = _order.WorkPid; //작업류에 따라 Pid의 대상이 달라짐
        TokenTile workTile = GameUtil.GetTileTokenFromMap(_order.WorkPlacePos);
        switch (workType)
        {
            case WorkType.ChangeBuild:
                //  Debug.Log("집행자가 토지변경");
                int placeNum = workPid;
                workTile.ChangePlace((TileType)placeNum);
                break;
            case WorkType.InterBuild:
                int InteriorNum = workPid;
                workTile.CompleteInterBuild(InteriorNum);
                break;
            case WorkType.ExpandLand:
              //  Debug.Log("집행자가 땅확장");
                PolicyExpandLand expandPolicy = new();
                int nationNum = workPid;
                expandPolicy.ExpandLand(workTile, nationNum); //확장에서 작업pid는 확장하려는 국가 Num
                break;
            case WorkType.NationLvUp:
              //  Debug.Log("집행자가 레벨업");
                PolicyLevelUp levelUpPolicy = new();
                levelUpPolicy.LevelUp(workTile);
                break;
            case WorkType.Spawn:
                int monsterPid = workPid;
                MgToken.GetInstance().SpawnCharactor(workTile.GetMapIndex(), monsterPid);
                break;

        }
    }
}
