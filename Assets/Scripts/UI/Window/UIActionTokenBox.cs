using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIActionTokenBox : UIBase
{
    [SerializeField]
    private Transform m_box;
    [SerializeField]
    private PlayerActionSlot m_sample;
    [SerializeField]
    private PlayerActionSlot[] m_playerActionSlots;

    public void SetActionSlot(TokenChar _charToken)
    {
        UISwitch(true);

        List<TokenAction> haveActionList = PlayerManager.GetInstance().GetMainChar().GetActionList();
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<PlayerActionSlot>(ref m_playerActionSlots, m_sample.gameObject, haveActionList.Count, m_box);
        //2. 리스트대로 슬랏에 표기
        SetSlots(haveActionList);
    }

    private void SetSlots(List<TokenAction> _equiptList)
    {
        int itemCount = _equiptList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            int index = i;
            m_playerActionSlots[i].gameObject.SetActive(true);
            m_playerActionSlots[i].SetSlot(_equiptList[i]);
        }
        for (int i = itemCount; i < m_playerActionSlots.Length; i++)
        {
            m_playerActionSlots[i].gameObject.SetActive(false);
        }
    }


}
