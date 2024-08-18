using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectSlot : SlotBase
{
    //�ƹ���ɾ��� �����ֱ⸸ �ϴ� ����
    public TMP_Text selectedText; //���߿� ���������� ��ü�ɺκ�
    public TMP_Text selectedValue;
    public TMP_InputField tmpInput; //���� �Էºκ�
    private SelectItemInfo selectInfo;
    private int slotIndex;

    private void Awake()
    {
        tmpInput.onEndEdit.AddListener(ChangeText);
    }
    public void SetSlot(TOrderItem _item, SelectItemInfo _selectInfo, int _index)
    {
        selectInfo = _selectInfo;
        slotIndex = _index;
        DetailSet(_item);
        tmpInput.gameObject.SetActive(false); //���� �Է� �޴� �κ� ����
    }

    private void DetailSet(TOrderItem _item)
    {
        //�����ַ��� ������ Ÿ�Կ� ���� �����ϴ°� 
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
        ConversationData sentenece = MgMasterData.GetInstance().GetConversationData((ConversationEnum)_item.SubIdx, _item.Value);
        selectedText.text = (slotIndex + 1).ToString();
        selectedValue.text = sentenece.GetScript();
    }

    private void SetIcon(TOrderItem _item)
    {
        selectedText.text = ((Capital)_item.SubIdx).ToString();
        selectedValue.text = _item.Value.ToString();
    }

    public void SetSelectState(int _selectValue, bool _isFixed)
    {
        selectedText.text += "����";
        if(_isFixed == false)
        {
            tmpInput.gameObject.SetActive(true);
            tmpInput.text = _selectValue.ToString();
        }
        
    }

    public void SetSelectValue(int _value)
    {
    //    Debug.Log(_value + "�� ��ǲ���� ����");
        tmpInput.text = _value.ToString();
    }

    public override void OnLeftClick()
    {
        base.OnLeftClick();
        if(selectInfo != null)
        selectInfo.OnSelectCallBack(slotIndex);
    }

    public void ChangeText(string _text)
    {
        if (selectInfo != null)
            selectInfo.OnChangeValueCallBack(slotIndex, int.Parse(_text));
    }
}
