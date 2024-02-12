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

    public override void InitiUI()
    {
        base.InitiUI();
        m_playerActionSlots = m_box.GetComponentsInChildren<PlayerActionSlot>();
    }

    public void SetActionSlot(TokenChar _charToken)
    {
        m_box.gameObject.SetActive(true);
        //Debug.Log(_charToken.GetItemName() + "액션 토큰 세팅해보기");
        
        List<TokenAction> actionList = _charToken.GetActionList();
        int charActionCount = actionList.Count;
        if (m_playerActionSlots.Length < charActionCount)
        {
            //부족한 슬롯 수 만큼 생성
            for (int i = 1; i <= charActionCount - m_playerActionSlots.Length; i++)
            {
                GameObject newSlot = Instantiate(m_sample).gameObject;
                newSlot.SetActive(true);
                newSlot.transform.SetParent(m_box);
            }
            m_playerActionSlots = m_box.GetComponentsInChildren<PlayerActionSlot>();
        }

        for (int i = 0; i < charActionCount; i++)
        {
            m_playerActionSlots[i].SetSlot(actionList[i]);
        }

    }

}
