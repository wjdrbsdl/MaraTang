using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectSlot : SlotBase
{
    //아무기능없이 보여주기만 하는 슬랏
    public TMP_Text selectedText; //나중에 아이콘으로 대체될부분
    public TMP_Text selectedValue;
    public TMP_InputField tmpInput; //수량 입력부분
    private ISelectCustomer callBackCustomer;
    private int slotIndex;

    private void Awake()
    {
        tmpInput.onEndEdit.AddListener(ChangeText);
    }
    public void SetSlot(TOrderItem _item, ISelectCustomer _customer, int _index)
    {
        callBackCustomer = _customer;
        slotIndex = _index;
        DetailSet(_item);
        tmpInput.gameObject.SetActive(false); //일단 오프
    }

    private void DetailSet(TOrderItem _item)
    {
        //보여주려는 아이템 타입에 따라 세팅하는거 
        TokenType type = _item.Tokentype;
        switch (type)
        {
            case TokenType.Conversation:
                SetText(_item);
                break;
            default:
                SetIcon(_item);
                break;
        }
    }

    private void SetText(TOrderItem _item)
    {
        ConversationData sentenece = MgMasterData.GetInstance().GetConversationData((ConversationTheme)_item.SubIdx, _item.Value);
        selectedValue.text = sentenece.GetScript();
    }

    private void SetIcon(TOrderItem _item)
    {
        selectedText.text = ((Capital)_item.SubIdx).ToString();
        selectedValue.text = _item.Value.ToString();
    }

    public void SetSelectState(int _selectValue, bool _isFixed)
    {
        selectedText.text += "선택";
        if(_isFixed == false)
        {
            tmpInput.gameObject.SetActive(true);
            tmpInput.text = _selectValue.ToString();
        }
        
    }

    public void SetSelectValue(int _value)
    {
    //    Debug.Log(_value + "로 인풋값을 수정");
        tmpInput.text = _value.ToString();
    }

    public override void OnLeftClick()
    {
        base.OnLeftClick();
        if(callBackCustomer != null)
        callBackCustomer.OnSelectCallBack(slotIndex);
    }

    public void ChangeText(string _text)
    {
        if (callBackCustomer != null)
            callBackCustomer.OnChangeValueCallBack(slotIndex, int.Parse(_text));
    }
}
