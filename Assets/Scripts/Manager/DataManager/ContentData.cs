using System.Collections.Generic;
using UnityEngine;

public class ContentData
{
    //컨텐츠 데이터로 퀘스트 조건, 페널티, 보상안등의 정보를 담고 있음. 
    public int ContentPid;
    public EOrderType CondtionType;
    public List<TOrderItem> MainItemList;
    public List<TOrderItem> SubItemList;

    public ContentData(string[] _parsingData)
    {

    }
}
