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
    
    }

    public void SelectItem(string _itemName)
    {
        selectedText.text = _itemName;
    }

    public void RestrictValue()
    {
        int inputValue = int.Parse(tmpInput.text);
        inputValue = Mathf.Max(min, inputValue);
        inputValue = Mathf.Min(max, inputValue);
        tmpInput.text = inputValue.ToString();
    }
}
