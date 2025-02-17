using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UICapital : UIBase
{
    [SerializeField]
    private Transform m_box;
    [SerializeField]
    private InfoCaptialStat m_sample;
    [SerializeField]
    private InfoCaptialStat[] m_capitalStats;

    public void ResetCapitalInfo(PlayerCapitalData _capitalData)
    {
        UISwitch(true);

        //기본 자원수
        int captialTypeCount = System.Enum.GetValues(typeof(Capital)).Length;

        //사용하는 만큼 스텟 생성
        MakeSamplePool<InfoCaptialStat>(ref m_capitalStats, m_sample.gameObject, captialTypeCount, m_box);
        //버튼 세팅
        SetButtons(_capitalData);
    }

    private void SetButtons(PlayerCapitalData _capitalData)
    {
        //0이 아닌 자원의 수 활성화
        int capitalTypes = GameUtil.EnumLength(Capital.Food);//총 자원수
        Dictionary<Capital, TokenBase> curCapitals = _capitalData.GetHaveCapitalDic();
        int setCount = 0; //정보 설정한 수

        foreach(KeyValuePair<Capital, TokenBase> item in curCapitals)
        {
            //0인 자원은 표시안함
            if (item.Value.GetStat(CapitalStat.Amount) == 0)
                continue;
            m_capitalStats[setCount].gameObject.SetActive(true);
            m_capitalStats[setCount].SetCaptialInfo(item.Value);
            setCount += 1; //세팅한 숫자 올리고
        }
        
        //세팅 된 숫자부터 그 뒤까진 비활성
        for (int i = setCount; i < capitalTypes; i++)
        {
            m_capitalStats[i].gameObject.SetActive(false);
        }
    }
}
