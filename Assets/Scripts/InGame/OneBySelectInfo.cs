using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class OneBySelectInfo
{
    //선택지가 제공되고, 취소가능하면 취소 
    //선택 즉시 그 효과가 적용되고, 리스트에서 해당 아이템을 제거후 다시 선택 진행
    //남은 선택 가능 수가 0이 될때 까지 선택
        
    private bool ableCancle = false; //해당 선택지를 취소할 수있는건가. 
    public int restSelectCount; 
    public int SerialNum = FixedValue.No_INDEX_NUMBER; //사용안할때는 none 넘버로 
    public ITradeCustomer Giver; //제공자
    public ITradeCustomer Taker; //받는자
    public List<TOrderItem> ItemList; //남은 선택 리스트



}
