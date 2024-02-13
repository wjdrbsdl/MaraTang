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
        Switch(true);

        //�⺻ �ڿ���
        int captialTypeCount = System.Enum.GetValues(typeof(Capital)).Length;

        //����ϴ� ��ŭ ���� ����
        MakeSamplePool<InfoCaptialStat>(ref m_capitalStats, m_sample.gameObject, captialTypeCount, m_box);
        //��ư ����
        SetButtons(_capitalData);
    }

    private void SetButtons(PlayerCapitalData _capitalData)
    {
        //0�� �ƴ� �ڿ��� �� Ȱ��ȭ
        int[] intVlaues = _capitalData.GetCapitalValue();
        Dictionary<Capital, TokenBase> curCapitals = _capitalData.GetCurrentCapital();
        int setCount = 0; //���� ������ ��

        foreach(KeyValuePair<Capital, TokenBase> item in curCapitals)
        {
            m_capitalStats[setCount].gameObject.SetActive(true);
            m_capitalStats[setCount].SetCaptialInfo(item.Value);
            setCount += 1; //������ ���� �ø���
        }
        
        //���� �� ���ں��� �� �ڱ��� ��Ȱ��
        for (int i = setCount; i < intVlaues.Length; i++)
        {
            m_capitalStats[i].gameObject.SetActive(false);
        }
    }
}
