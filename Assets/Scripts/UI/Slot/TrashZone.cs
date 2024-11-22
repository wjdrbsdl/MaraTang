using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrashZone : MonoBehaviour, IDropHandler
{

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("뭔가 드롭이 되었따?");
        
        if (DragSlot.instance.dragslot != null)
        {
            TokenBase dragToken = DragSlot.instance.dragslot.GetTokenBase();
            if (dragToken != null)
                ThrowAway(dragToken);
        }
    }

    private void ThrowAway(TokenBase _token)
    {
        TokenType tokenType = _token.GetTokenType();
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        switch (tokenType)
        {
            case TokenType.Equipt:
                mainChar.RemoveEquipt((EquiptItem)_token);
                break;
                
        }
    }
}
