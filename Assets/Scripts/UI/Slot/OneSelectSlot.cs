using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class OneSelectSlot : SlotBase
{
    //아무기능없이 보여주기만 하는 슬랏
    public TMP_Text selectedText; //나중에 아이콘으로 대체될부분
    public TMP_Text selectedValue;
    private UIOneByeSelect m_OneByeUI;
    private int slotIndex;


    public void SetSlot(TOrderItem _item, UIOneByeSelect _oneSelectUI, int _index)
    {
        m_OneByeUI = _oneSelectUI;
        slotIndex = _index;
        DetailSet(_item);
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
        ConversationData sentenece = MgMasterData.GetInstance().GetConversationData((ConversationThemeEnum)_item.SubIdx, _item.Value);
        selectedText.text = (slotIndex + 1).ToString();
        selectedValue.text = sentenece.GetScript();
    }

    private void SetIcon(TOrderItem _item)
    {
        selectedText.text = ((Capital)_item.SubIdx).ToString();
        selectedValue.text = _item.Value.ToString();
    }


    public override void OnLeftClick()
    {
        m_OneByeUI.OnSelectItem(slotIndex); //아이템 선택시
    }

}
