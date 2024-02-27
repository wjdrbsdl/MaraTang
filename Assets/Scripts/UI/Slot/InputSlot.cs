using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class InputSlot : SlotBase
{
    public TMP_InputField tmpInput; //수량 입력부분
    public TMP_Text selectedText; //나중에 아이콘으로 대체될부분
    public int min, max = 1;
    public TokenType showcaseType = TokenType.Capital; //표시될 타입 - 해당 타입으로 showCase에 리스트를 요청
    public ShowcaseSlot ShowCase = null;
    public Action<InputSlot> OnInputEvent;
    public Action<InputSlot> OnClickEvent; //해당 슬롯 눌렀을 때 콜백을 보내던가 해야하는데
    private void Start()
    {
        tmpInput.onValueChanged.AddListener(delegate { RestrictValue(); });
    }

    public override void OnLeftClick()
    {
        if (GetTokenBase() != null)
            Debug.Log(GetTokenBase().GetPid() + "할당되어 있음 "+GetTokenBase().GetStat(CapitalStat.Amount));
        if (OnClickEvent != null)
            OnClickEvent(this);
    }

    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        max = _token.GetStat(CapitalStat.Amount); //최댓값 수정 
        tmpInput.text = 1.ToString(); //초기값 1 로 수정
        selectedText.text = _token.GetItemName();
    }

    public void SetShowCase(ShowcaseSlot _caseSlot)
    {
        ShowCase = _caseSlot;
        if (OnInputEvent != null)
            OnInputEvent(this); //자기를 담은 이벤트 수행
    }

    public void SetEventOnInput(Action<InputSlot> _onClick, Action<InputSlot> _onInput)
    {
        OnClickEvent = _onClick;
        OnInputEvent = _onInput;
    }


    public int GetAmount()
    {
        return int.Parse(tmpInput.text);
    }

    private void RestrictValue()
    {
        int inputValue = int.Parse(tmpInput.text);
        inputValue = Mathf.Max(min, inputValue);
        inputValue = Mathf.Min(max, inputValue);
        tmpInput.text = inputValue.ToString();
    }
}
