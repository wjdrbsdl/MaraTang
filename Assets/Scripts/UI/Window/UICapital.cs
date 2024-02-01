using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
        Switch(true);

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
        int[] intVlaues = _capitalData.GetCapitalValue();
        int setCount = 0; //정보 설정한 수
        for (int i = 0; i < intVlaues.Length; i++)
        {
            if (intVlaues[i] == 0)
                continue;

            //0이 아닌 경우에만 정보 표기하고
            m_capitalStats[setCount].gameObject.SetActive(true);
            m_capitalStats[setCount].SetCaptialInfo((Capital)i, intVlaues[i]);
            setCount += 1; //세팅한 숫자 올리고
        }
        //세팅 된 숫자부터 그 뒤까진 비활성
        for (int i = setCount; i < intVlaues.Length; i++)
        {
            m_capitalStats[i].gameObject.SetActive(false);
        }
    }
}
