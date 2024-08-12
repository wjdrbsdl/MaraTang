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
        //�ش� �н��ҿ��� �����ִ� �׼ǵ��� ���� 
        UISwitch(true);
        //���� ������ ���� ��� �� �ִ� �׼ǵ��� �ٸ� 
        Debug.Log("�н� ����");
        //1. �н� ������ �׼� ���� ������
        int captialTypeCount = System.Enum.GetValues(typeof(Capital)).Length;
        //2. ���ڸ�ŭ ���� ����
        MakeSamplePool<StudySlot>(ref m_studySlots, m_studySlotSample.gameObject, captialTypeCount, m_grid);
        //3. �� ���� ���� ����
    
    }

    private void SetSlots()
    {
        //1. �׼� ���� ǥ�� 

        //2. �н� ���� ǥ��

        //3. �н� ���� ���� ǥ��
    }

}
