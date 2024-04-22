using System.Collections.Generic;
using UnityEngine;

public class ContentData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public EOrderType ConditionType;
    public List<TOrderItem> ConditionMainItemList;
    public List<List<TOrderItem>> ConditionSubItemList;
    public List<TOrderItem> RewardMainItemList;
    public List<TOrderItem> RewardSubItemList;
    public List<TOrderItem> PenaltyMainItemList;
    public List<TOrderItem> PenaltySubItemList;

    public ContentData(string[] _parsingData)
    {
        ContentPid = int.Parse(_parsingData[0]);
        ConditionType = (EOrderType)int.Parse(_parsingData[1]);

        //컨디션 데이터가 없으면 패쓰? 이럴린 없는데
        if (_parsingData.Length < 3)
            return;

        ConditionMainItemList = new();
        string[] itemOrderSplit = _parsingData[2].Split(" ");
        for (int i = 0; i < itemOrderSplit.Length; i++)
        {
            string[] itemSplit = itemOrderSplit[i].Split(FixedValue.PARSING_DIVIDE);
            //0_1_2 로 구성 0은 tokenGroup, 1은 pid, 2는 벨류
            TOrderItem newItem = new(int.Parse(itemSplit[0]), int.Parse(itemSplit[1]), int.Parse(itemSplit[2]));
            ConditionMainItemList.Add(newItem);
        }

        if (_parsingData.Length < 4)
            return;

        ConditionSubItemList = new();
        //1. 엔터로 분할
        string[] subitemOrderSplit = _parsingData[3].Split(" ");
        for (int i = 0; i < subitemOrderSplit.Length; i++)
        {
            //2. Main 1개에 들어갈 Sub들의 분할
            string[] lineSplit = subitemOrderSplit[i].Split("/");
            List<TOrderItem> lineList = new();
            for (int x = 0; x < lineSplit.Length; x++)
            {
                //3. sub 한줄의 한 당락씩 아이템 구조 생성
                string[] itemSplit = lineSplit[x].Split(FixedValue.PARSING_DIVIDE);
                //0_1_2 로 구성 0은 tokenGroup, 1은 pid, 2는 벨류
                TOrderItem newItem = new(int.Parse(itemSplit[0]), int.Parse(itemSplit[1]), int.Parse(itemSplit[2]));

                //4. 라인리스트에 추가
                lineList.Add(newItem);
            }
            //5. 라인리스트를 전체 서브 리스트에 추가 
            ConditionSubItemList.Add(lineList);

            
        }

    }
}
