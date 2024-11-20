using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DropItemManager : Mg<DropItemManager>
{
    //동적으로 보상받을 아이템들이 정해졌고, 적용 혹은 선택을 기다릴 때 오는 매니저
    //로딩시 선택부터 진행해야하는 경우 이곳에 체크를 해놓고, 아이템 리스트나 연결시킬 컨텐츠등을 기록해놓는다. 


    #region 초기화
    public DropItemManager()
    {
        InitiSet();
    }

    public override void InitiSet()
    {
        g_instance = this;
    }
    #endregion

    public void DropItem(int _monsterPid)
    {
        List<TOrderItem> dropList = MgMasterData.GetInstance().GetDropItem(_monsterPid);
        for (int i = 0; i < dropList.Count; i++)
        {
            Debug.Log(dropList[i].Tokentype + "종류 드랍");
        }

    }

}
