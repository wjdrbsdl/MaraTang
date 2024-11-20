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
    private Transform m_grid;
    [SerializeField]
    private ShowcaseSlot[] m_showcaseSlots;

    private Action<List<ShowcaseSlot>> m_selectAction;
    private int m_selectMaxCount;
    private List<ShowcaseSlot> m_selectedSlotList;

    public void OpenWindow(Action<List<ShowcaseSlot>> _selecttAction, int _selectMaxCount)
    {
        UISwitch(true);
        //0. 요청한 스타일 할당
        m_selectAction = _selecttAction;
        m_selectMaxCount = _selectMaxCount;
        m_selectedSlotList = new();

        //1. 캐릭터가 보유한 자원 리스트를 가져온다. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapitalDic();
     
        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_grid);
        
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

    private InputSlot m_targetSlot;
    public void OpenWindow(InputSlot _inputSlot)
    {
        UISwitch(true);
        m_targetSlot = _inputSlot;

        //1. 캐릭터가 보유한 자원 리스트를 가져온다. 
        Dictionary<Capital, TokenBase> haveCapitals = PlayerCapitalData.g_instance.GetHaveCapitalDic();

        MakeSamplePool<ShowcaseSlot>(ref m_showcaseSlots, m_showcaseSample.gameObject, haveCapitals.Count, m_grid);

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
        RectTransform caseRectTrans = m_box.GetComponent<RectTransform>();
        caseRectTrans.pivot = _tran.pivot; //피봇 동일하게 바꾼뒤
        Vector3 movePos = _tran.position; //이동할 위치로 이동시키고 
        //원본 높이 * 피봇 만큼, 난 피봇 반대만큼 - 앞에서 피봇을 일치 시켜놨기 때문에 밑에 내려갈애는 1- 반전으로 진행
        float yMove = caseRectTrans.sizeDelta.y * (1 - caseRectTrans.pivot.y)+ _tran.sizeDelta.y * _tran.pivot.y;
        yMove += 5f; //패딩
        movePos.y -= yMove; //y값을이동 
        caseRectTrans.position = movePos;
    }
   
    public void TempSelectSlot(ShowcaseSlot _caseSlot)
    {
        m_targetSlot.SetShowCase(_caseSlot);
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
