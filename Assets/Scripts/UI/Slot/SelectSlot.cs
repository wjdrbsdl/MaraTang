using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectSlot : SlotBase
{
    //�ƹ���ɾ��� �����ֱ⸸ �ϴ� ����
    public TMP_Text selectedText; //���߿� ���������� ��ü�ɺκ�
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
        selectedText.text += "����";
    }

    public override void OnLeftClick()
    {
        base.OnLeftClick();
        if(callBackCustomer != null)
        callBackCustomer.OnSelectCallBack(slotIndex);
    }

}
