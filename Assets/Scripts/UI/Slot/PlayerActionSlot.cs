using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class PlayerActionSlot : SlotBase
{
    public override void OnLeftClick()
    {
        SelectAction();
    }

    private void SelectAction()
    {
        //플레이어가 선택한 액션을 예약하는 부분
        PlayerManager.g_instance.SelectActionToken(m_token);
    }

}
