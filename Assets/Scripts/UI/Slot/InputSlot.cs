using System.Collections;
using UnityEngine;
using TMPro;
public class InputSlot : SlotBase
{
    public TMP_InputField tmpInput; //수량 입력부분
    public TMP_Text selectedText; //나중에 아이콘으로 대체될부분
    public int min, max = 1;

    private void Start()
    {
        tmpInput.onValueChanged.AddListener(delegate { RestrictValue(); });
    }

    public override void OnLeftClick()
    {
        if (GetTokenBase() != null)
            Debug.Log(GetTokenBase().GetPid() + "할당되어 있음 "+GetTokenBase().GetStat(CapitalStat.Amount));
    }

    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);
        max = _token.GetStat(CapitalStat.Amount); //최댓값 수정 
        tmpInput.text = 1.ToString(); //초기값 1 로 수정
        selectedText.text = _token.GetItemName();
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
