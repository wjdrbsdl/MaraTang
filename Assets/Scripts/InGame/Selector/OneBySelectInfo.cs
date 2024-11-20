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

    public OneBySelectInfo(List<TOrderItem> _selectList, int _ableCount)
    {
        ItemList = _selectList;
        restSelectCount = _ableCount;
    }

    public void OpenSelectUI()
    {
        //열고
        MgUI.GetInstance().ShowOneByeSelectList(this);
    }

    private TokenType[] exception = {TokenType.Equipt };
    public void SelectOne(int _index, UIOneByeSelect _oneByUI)
    {
        TOrderItem selectedItem = ItemList[_index];
        //적용할것 골랐다.
        Debug.Log("고름");
        //적용시키고
        bool isAdapt = AdaptItem(selectedItem);
        
        if (isAdapt == false)
        {
            //해당 아이템이 적용 안되었으면 선택은 무효 
            Debug.Log("적용 못함");
            return;
        }
        Debug.Log("적용 함 UI 리셋");
        RemoveItem(selectedItem);
        //선택한 수 까고
        restSelectCount -= 1;
        
        if(CheckRest() == true)
        {
            //남은게 있으면 다시 UI 세팅
            _oneByUI.ResetSlot();
            return;
        }
        _oneByUI.ReqeustOff();
        //아니면 UI 종료
            
    }

    private bool AdaptItem(TOrderItem _item)
    {
        Debug.Log("적용");
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        switch (_item.Tokentype)
        {
            case TokenType.Capital:
                Debug.Log("자원올린다");
                Taker.PayCostData(new TItemListData(_item), false);
                return true; //따로 받은게 없음
            case TokenType.Equipt:
                Debug.Log("장비획득한다");
                EquiptItem equiptCopy = new EquiptItem(MgMasterData.GetInstance().GetEquiptData(_item.SubIdx));
                return mainChar.AquireEquipt(equiptCopy);
            case TokenType.CharStat:
                Debug.Log("스텟올린다");
                mainChar.CalStat((CharStat)_item.SubIdx, _item.Value);//형변환 안해두되는데 아쉽군. 
                return true;
        }
        //적용 항목이 없는 경우엔 적용 안된걸로
        return false;
    }

    private void RemoveItem(TOrderItem _item)
    {
        ItemList.Remove(_item);
    }

    private bool CheckRest()
    {
        return restSelectCount >= 1;
    }

}
