using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TipType
{
   None, Skill, Quick, SearchLand, Seed, Plant, Authored, MyLand
}

public class ToolTip : MonoBehaviour
{
    [SerializeField] private GameObject m_toolTipBox; //���� ������Ʈ���� ����. 
    [SerializeField] private ToolTipBase[] m_toolTips;
    private UIType m_currentUI = UIType.None; //���� ����������ִ°�. 
    //���� ������ ��Ÿ�Կ� ���� �� ������ �ٸ����� �ʿ��� ��Ÿ���� ���ٸ�, �ش� ��Ÿ������ ������ �ִ� �������� �����ϴ� ���. 
    //���� �� GameObj�� ��ġ�� ǥ�õǴ� UI�� ���ο� ��ġ�ϱ� ���� MGUI�� ���� ȣ��. 
    //�غ����� �������� �־���ϴµ� �װ� ToolTipBase�� �������̵��ѳ༮��. �� ���̽���� ����. 

    #region �ʱ�ȭ
    void Start()
    {
        MakeToolFrame();
    }

    private void MakeToolFrame()
    {
        ToolTipBase[] instantToolArray = m_toolTipBox.GetComponentsInChildren<ToolTipBase>(); //���ڸ�ŭ ��� , 
        m_toolTips = new ToolTipBase[System.Enum.GetValues(typeof(TipType)).Length];
        for (int i = 0; i < instantToolArray.Length; i++)
        {
            ToolTipBase turnTip = instantToolArray[i];
            int seat = (int)turnTip.m_tipType;
            m_toolTips[seat] = turnTip;
        }
    }
    #endregion

    //�ش� Ÿ���� �����̰� �����ٰ� �˷���
    public void CallOfUI(UIType _ui)
    {
        if (m_currentUI == _ui)
            CloseTip();
    }

    public void ShowTip(SlotBase _slot)
    {
        TipType _type = _slot.GetTipType();
        ToolTipBase tooltip = m_toolTips[(int)_type]; //����� �� ������ 
        //������ ��¿������
        if (tooltip == null)
            return;

        //������ UI�� ����� ���� ����
        SetTargetUI(_slot.m_uiType);
        tooltip.TipSwitch(true);
        tooltip.SetToolTip(_slot.GetItemBase());
    }

    //Ŀ�� ����Ʈ �ƿ����� ����Ǵ� ���� 
    public void CloseTip()
    {
        TipSwitch(false);
        ResetTargetUI();
    }

    private void TipSwitch(bool _on)
    {
        for (int i = 0; i < m_toolTips.Length; i++)
        {
            if (m_toolTips[i] == null)
                continue;

            m_toolTips[i].TipSwitch(_on);
        }
    }

    private void SetTargetUI(UIType _uiType)
    {
        m_currentUI = _uiType;
    }
    private void ResetTargetUI()
    {
        m_currentUI = UIType.None;
    }
}
