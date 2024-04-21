using System.Collections.Generic;
using UnityEngine;

public class ContentData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public EOrderType ConditionType;
    public List<TOrderItem> MainItemList;
    public List<TOrderItem> SubItemList;

    public ContentData(string[] _parsingData)
    {
        ContentPid = int.Parse(_parsingData[0]);
        ConditionType = (EOrderType)int.Parse(_parsingData[1]);
        if (_parsingData.Length >= 3)
        {
            MainItemList = new();
            string[] itemOrderSplit = _parsingData[2].Split(" ");
            for (int i = 0; i < itemOrderSplit.Length; i++)
            {
                string[] itemSplit = itemOrderSplit[i].Split(FixedValue.PARSING_DIVIDE);
                //0_1_2 로 구성 0은 tokenGroup, 1은 pid, 2는 벨류
                TOrderItem newItem = new(int.Parse(itemSplit[0]), int.Parse(itemSplit[1]), int.Parse(itemSplit[2]));
                MainItemList.Add(newItem);
            }
        }


    }
}
