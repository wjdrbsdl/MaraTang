using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStudyAction : UIBase
{
    [SerializeField]
    private StudySlot m_studySlotSample;
     [SerializeField]
    private Transform m_grid;
    [SerializeField]
    private StudySlot[] m_studySlots;

    public void SetStudyInfo(TokenTile _tile)
    {
        //해당 학습소에서 배울수있는 액션들을 나열 
        UISwitch(true);
        //나라 종류에 따라 배울 수 있는 액션들이 다름 
        Debug.Log("학습 나열");
        //1. 학습 가능한 액션 정보 가져옴
        int captialTypeCount = System.Enum.GetValues(typeof(Capital)).Length;
        //2. 숫자만큼 슬롯 생성
        MakeSamplePool<StudySlot>(ref m_studySlots, m_studySlotSample.gameObject, captialTypeCount, m_grid);
        //3. 각 슬롯 마다 세팅
    
    }

    private void SetSlots()
    {
        //1. 액션 정보 표기 

        //2. 학습 여부 표기

        //3. 학습 가능 여부 표기
    }

}
