using System.Collections.Generic;
using UnityEngine;

public class ContentData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public EOrderType ConditionType;
    public List<TOrderItem> ConditionMainItemList;
    public List<List<TOrderItem>> ConditionSubItemList;
    public ERewardType RewardType;
    public List<TOrderItem> RewardMainItemList;
    public List<TOrderItem> RewardSubItemList;
    public List<TOrderItem> PenaltyMainItemList;
    public List<TOrderItem> PenaltySubItemList;

    public ContentData(string[] _parsingData)
    {
        ContentPid = int.Parse(_parsingData[0]);
        ConditionType = (EOrderType)int.Parse(_parsingData[1]);

        //1. 컨디션의 매인 아이템 리스트 파싱
        int mainItemListIdx = 2;
        ConditionMainItemList = new();
        GameUtil.ParseOrderItemList(ConditionMainItemList, _parsingData, mainItemListIdx);
      

        //1. 컨디션의 서브 아이템 리스트 파싱
        int subItemListIdx = 3;
        ConditionSubItemList = new();
        if ( subItemListIdx < _parsingData.Length)
        {
            //재료가 한줄마다 있는게 아닌, 한줄에 여러 리스트가 나열되어 있어서 한번더 split해야함. 
            //1. 엔터로 분할
            string[] subitemOrderSplit = _parsingData[3].Split(" ");
            for (int i = 0; i < subitemOrderSplit.Length; i++)
            {
                //2. Main 1개에 들어갈 Sub들의 분할
                string[] lineSplit = subitemOrderSplit[i].Split("/");
                List<TOrderItem> lineList = new();
                for (int x = 0; x < lineSplit.Length; x++)
                {
                    TOrderItem newItem = GameUtil.ParseOrderItem(lineSplit[x]);
                    if (newItem.IsVaridTokenType())
                    {
                        lineList.Add(newItem);
                    }
                }
                ConditionSubItemList.Add(lineList);
            }

        }
   
    }
}
