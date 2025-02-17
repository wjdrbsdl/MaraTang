﻿using System.Collections;
using UnityEngine;
using TMPro;


public class BtnCharStatIntense : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_statText;
    [SerializeField]
    private TMP_Text m_valueText;
    private TokenChar m_char;
    private CharStat m_stat;
    public void SetButton(TokenChar _char, CharStat _stat)
    {
        m_char = _char;
        m_statText.text = _stat.ToString();
        m_stat = _stat;
        m_valueText.text = _char.GetStat(_stat).ToString();
    }
    public void OnButtonClick()
    {
        //해당 액션을 선택하면 
        if (m_char.IsPlayerChar())
        {
            Debug.Log("스텟 강화하기");
            //임시로 바로 스텟강화
            GamePlayMaster.GetInstance().RuleBook.IntenseStat(m_char, m_stat);
        }
        
        else
        {
            Debug.Log("내캐릭불가");
        }
    }
}
