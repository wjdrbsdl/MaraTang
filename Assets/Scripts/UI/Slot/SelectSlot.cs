using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectSlot : SlotBase
{
    //아무기능없이 보여주기만 하는 슬랏
    public TMP_Text selectedText; //나중에 아이콘으로 대체될부분
    public TMP_Text selectedValue;
    private ISelectCustomer callBackCustomer;
    private int slotIndex;
    public void SetSlot(TOrderItem _item, ISelectCustomer _customer, int _index)
    {
        callBackCustomer = _customer;
        slotIndex = _index;
        selectedText.text = ((Capital)_item.SubIdx).ToString();
        selectedValue.text = _item.Value.ToString();
    }

    public void SetSelectState()
    {
        selectedText.text += "선택";
    }

    public override void OnLeftClick()
    {
        base.OnLeftClick();
        if(callBackCustomer != null)
        callBackCustomer.OnSelectCallBack(slotIndex);
    }

}
