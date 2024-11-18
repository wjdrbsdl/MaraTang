using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnInputResource : MonoBehaviour
{
    private TokenTile m_tile;
    private UITileInfo m_motherUI;

    public void SetButton(TokenTile _tile, UITileInfo _motherUI)
    {
        m_tile = _tile;
        m_motherUI = _motherUI;
    }
    public void OnClickPut()
    {
        WorkOrder workOrder = m_tile.GetWorkOrder();
        if (workOrder == null)
            return;
        MGContent.GetInstance().SendActionCode(new TOrderItem(TokenType.Conversation, (int)ConversationEnum.Response, (int)ResponseEnum.WorkSupprot));
        workOrder.PutResource(PlayerCapitalData.g_instance);

        m_motherUI.ResetUI();
    }

    public void SetActive(bool _on)
    {
        gameObject.SetActive(_on);
    }
}
