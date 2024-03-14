using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICharStats : UIBase
{
    //해당 타일의 정보 표기 및 타일에서 할 수 있는 작업을 제공하는 UI
    [SerializeField]
    private BtnQuestList m_questInfo; //해당 캐릭터가 퀘스트와 연관이있으면 표기 
    [SerializeField]
    private Transform m_charStatBox; //액션 리스트 버튼 담을 장소
    [SerializeField]
    private BtnCharStatIntense m_statButtonSample;
    [SerializeField]
    private BtnCharStatIntense[] m_statkButtones;
    private int setCount = 0;
    private CharStat[] m_showStat = {CharStat.Strenth, CharStat.Dexility, CharStat.Inteligent };

    public void SetCharStat(TokenChar _char)
    {
        Switch(true);
  
        setCount = m_showStat.Length;
        //사용하는 만큼 버튼 활성화 
        MakeSamplePool<BtnCharStatIntense>(ref m_statkButtones, m_statButtonSample.gameObject, setCount, m_charStatBox);
        //버튼 세팅
        SetButtons(_char);
        SetQuestInfo(_char);
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

    private void SetQuestInfo(TokenChar _char)
    {
        if (_char.QuestCard == null)
        {
            m_questInfo.gameObject.SetActive(false);
            return;
        }
        m_questInfo.gameObject.SetActive(true);
        m_questInfo.SetButton(_char.QuestCard);
    }
}
