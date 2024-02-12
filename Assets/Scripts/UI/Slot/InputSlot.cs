using System.Collections;
using UnityEngine;
using TMPro;
public class InputSlot : SlotBase
{
    public TMP_InputField tmpInput;
    public TMP_Text selectedText;
    private int m_clicked = 0;
    public int min, max = 1;

    private void Start()
    {
        tmpInput.onValueChanged.AddListener(delegate { RestrictValue(); });
    }

    public override void OnLeftClick()
    {
        //1. 보유한 재료 전시 -> 선택하면 해당 inputSlot의 item으로 세팅
        MgUI.GetInstance().ShowCaseOpen(this);
        //2. min,max 값 조정 
   
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
