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
        List<int> actionList = MgMasterData.GetInstance().GetCharActionList();
        int actionCount = actionList.Count;
        //2. ���ڸ�ŭ ���� ����
        MakeSamplePool<StudySlot>(ref m_studySlots, m_studySlotSample.gameObject, actionCount, m_grid);
        //3. �� ���� ���� ����
    
    }

    private void SetSlots(List<int> _actionList)
    {
        for (int i = 0; i < _actionList.Count; i++)
        {
            int actionPid = _actionList[i];
            m_studySlots[i].SetAction(actionPid);
        }
        
    }

}
