using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UICharStats : UIBase
{
    //해당 타일의 정보 표기 및 타일에서 할 수 있는 작업을 제공하는 UI
    [SerializeField]
    private Transform m_charStatBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private BtnCharStatIntense m_statButtonSample;
    [SerializeField]
    private BtnCharStatIntense[] m_statkButtones;
    [SerializeField]
    private TMP_Text m_buffText;
    private int setCount = 0;
    private CharStat[] m_showStat = {CharStat.Strenth, CharStat.Dex, CharStat.Inteligent };

    public void SetCharStat(TokenChar _char)
    {
        UISwitch(true);
  
        setCount = m_showStat.Length;
        //사용하는 만큼 버튼 활성화 
        MakeSamplePool<BtnCharStatIntense>(ref m_statkButtones, m_statButtonSample.gameObject, setCount, m_charStatBox);
        //버튼 세팅
        SetButtons(_char);
        SetBuffInfo(_char);
    }
 
    private void SetButtons(TokenChar _char)
    {

        for (int i = 0; i < m_showStat.Length; i++)
        {
            m_statkButtones[i].gameObject.SetActive(true);
            m_statkButtones[i].SetButton(_char, m_showStat[i]);
        }
        for (int i = setCount; i < m_statkButtones.Length; i++)
        {
            m_statkButtones[i].gameObject.SetActive(false);
        }
    }

    private void SetBuffInfo(TokenChar _char)
    {
        List<TokenBuff> buffList = _char.GetBuffList();
        string info = "";
        for (int i = 0; i < buffList.Count; i++)
        {
            TokenBuff buff = buffList[i];
            info += buff.GetItemName() + "의 효과는 ";
            for (int x = 0; x < buff.m_effect.Count; x++)
            {
                TOrderItem effect = buff.m_effect[x];
                string effectStr = GameUtil.GetTokenEnumName(effect);
                info += effectStr + " 파워는 "+buff.m_power.ToString();
            }
           info +=" \n";
        }
        m_buffText.text = info;
    }
}
