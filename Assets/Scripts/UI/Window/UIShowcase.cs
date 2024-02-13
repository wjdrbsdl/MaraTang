using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShowcase : UIBase
{
    [SerializeField]
    private ShowcaseSlot m_showcaseSample;
    [SerializeField]
    private Transform m_box;
    [SerializeField]
    private ShowcaseSlot[] m_showcaseSlots;

    private Action<List<ShowcaseSlot>> m_selectAction;
    private int m_selectMaxCount;
    private List<ShowcaseSlot> m_selectedSlotList;

    public void OpenWindow(Action<List<ShowcaseSlot>> _selecttAction, int _selectMaxCount)
    {
        base.OpenWindow();
        //0. 요청한 스타일 할당
        m_selectAction = _selecttAction;
        m_selectMaxCount = _selectMaxCount;
        m_selectedSlotList = new();

        //1. 캐릭터가 보유한 자원 리스트를 가져온다. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapital();
     
        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_box);
        
       int setCount = 0; //정보 설정한 수

        foreach (KeyValuePair<Capital, TokenBase> item in haveCapitals)
        {
            m_showcaseSlots[setCount].gameObject.SetActive(true);
            m_showcaseSlots[setCount].ShowCaseSet(item.Value, this); 
            setCount += 1; //세팅한 숫자 올리고
        }

        //세팅 된 숫자부터 그 뒤까진 비활성
        for (int i = setCount; i < haveCapitals.Count; i++)
        {
            m_showcaseSlots[i].gameObject.SetActive(false);
        }
    }

    public void SizeControl(RectTransform _tran)
    {
        RectTransform rectTrans = m_box.GetComponent<RectTransform>();
        rectTrans.sizeDelta = _tran.sizeDelta;
        Vector3 movePos = _tran.position;
        float yMove = rectTrans.sizeDelta.y * 0.5f + _tran.sizeDelta.y * 0.5f + 55f;
        movePos.y -= yMove;
        rectTrans.position = movePos;
    }
   
    public bool SelectSlot(ShowcaseSlot _caseSlot)
    {
        //슬랏을 선택했을때 선택 가능한지 여부를 반환
        //현재 선택된 횟수를 비교해서 최대 횟수면 불가능 반환
        if (m_selectedSlotList.Count == m_selectMaxCount)
                return false;

        m_selectedSlotList.Add(_caseSlot); //공간있으면 추가
        OnChangedSelect();
        return true;
    }

    public bool CancleSlot(ShowcaseSlot _caseSlot)
    {
        //선택했던 리스트에서 제외하고 트루반환
        m_selectedSlotList.Remove(_caseSlot);
        OnChangedSelect();
        return true;
    }

    private void OnChangedSelect()
    {
        m_selectAction(m_selectedSlotList);
    }
}
