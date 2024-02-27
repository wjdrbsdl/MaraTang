using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class PlayerActionSlot : SlotBase
{
    [SerializeField] private TMP_Text m_remainCool;
    [SerializeField] private TMP_Text m_restCount;

    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        m_remainCool.gameObject.SetActive(false);
        m_restCount.text = _token.GetStat(CharActionStat.RemainCountInTurn).ToString();
        int cool = _token.GetStat(CharActionStat.RemainCool);
        if(cool != 0)
        {
            m_remainCool.gameObject.SetActive(true);
            m_remainCool.text = cool.ToString();
        }
    }

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
