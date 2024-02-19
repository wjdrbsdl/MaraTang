using System.Collections;
using UnityEngine;
using TMPro;


public class BtnCharStatIntense : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_statText;
    private TokenChar m_char;
    public void SetButton(TokenChar _char, CharStat _stat)
    {
        m_char = _char;
        m_statText.text = _char.GetStat(_stat).ToString();
    }
    public void OnButtonClick()
    {
        //해당 액션을 선택하면 
        if(m_char.IsPlayerChar())
        Debug.Log("스텟 강화하기");
        else
        {
            Debug.Log("내캐릭불가");
        }
    }
}
