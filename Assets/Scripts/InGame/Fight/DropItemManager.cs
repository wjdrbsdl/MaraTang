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
        List<TOrderItem> aquireList = new(); //드롭할 아이템
        List<TOrderItem> dropList = MgMasterData.GetInstance().GetDropItem(_monsterPid);
        //1. 고정 드랍템
        FixedDrop(aquireList);
        //2. 공용드랍으로 티어 드랍인 경우
        TierDrop(aquireList);
        //3. 드랍 아이템 획득
        AquireDropItem(aquireList);        
    }

    private void FixedDrop(List<TOrderItem> _aquireList)
    {

    }

    private void TierDrop(List<TOrderItem> _aquireList)
    {
        //1. 티어별 드랍확률 구함

        //2. 정해진 티어 품목들 나열

        //3. 결정된 품목 드랍률 적용 
    }

    private void AquireDropItem(List<TOrderItem> _aquireList)
    {
        List<TOrderItem> aquireList = _aquireList;
        List<TOrderItem> immediatelyList = new();
        List<TOrderItem> waitList = new();
        //바로 획득 적용 가능한 류와 불가능한류 나눔
        for (int i = 0; i < aquireList.Count; i++)
        {
            TOrderItem dropItem = aquireList[i];
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
