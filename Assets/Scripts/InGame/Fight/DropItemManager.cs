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

    private TokenType[] waitArray = { TokenType.CharStat, TokenType.Equipt };
    public void DropItem(int _monsterPid)
    {
        //드랍된 아이템을 구별하여 선택이 필요한건 선택해서 하도록
        //퀘스트 보상이나 상황 진행에서 쓰이는 상황과는 별도
        //드롭아이템은 모두 선택가능하다는 상황아래 언제나 적용이 가능, 유리한 경우는 무조건 적용 플레이어의 추가 조작이 필요한 경우엔 따로 집행.
        //그밖에 선택수에 제한이 있는 경우는 다른것들과 경쟁해야하기에 예외없이 모두 선택지에 표기해야함. 

        List<TOrderItem> dropList = MgMasterData.GetInstance().GetDropItem(_monsterPid);
        List<TOrderItem> immediatelyList = new();
        List<TOrderItem> waitList = new();
        //바로 획득 적용 가능한 류와 불가능한류 나눔
        for (int i = 0; i < dropList.Count; i++)
        {
            TOrderItem dropItem = dropList[i];
            if (Array.IndexOf(waitArray, dropItem.Tokentype) != -1)
            {
               // Debug.Log(dropItem.Tokentype + "선택류에 추가");
                waitList.Add(dropItem);
            }
            else
            {
               // Debug.Log(dropItem.Tokentype + "즉시 효과에 추가");
                immediatelyList.Add(dropItem);
            }
        }
       // Debug.Log("선택류 리스트로 선택 정보 생성");
        OneBySelectInfo oneBySelectInfo = new OneBySelectInfo(waitList, waitList.Count);
        oneBySelectInfo.OpenSelectUI();

    }

}
