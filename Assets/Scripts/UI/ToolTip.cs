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
    [SerializeField] private GameObject m_toolTipBox; //툴팁 오브젝트들이 담긴곳. 
    [SerializeField] private ToolTipBase[] m_toolTips;


    #region 초기화
    void Start()
    {
        MakeToolFrame();
    }

    private void MakeToolFrame()
    {
        ToolTipBase[] instantToolArray = m_toolTipBox.GetComponentsInChildren<ToolTipBase>(); //숫자만큼 담고 , 
        m_toolTips = new ToolTipBase[System.Enum.GetValues(typeof(TipType)).Length];
        for (int i = 0; i < instantToolArray.Length; i++)
        {
            ToolTipBase turnTip = instantToolArray[i];
            int seat = (int)turnTip.m_tipType;
            m_toolTips[seat] = turnTip;
        }
    }
    #endregion


    public void ShowTip(SlotBase _slot)
    {
        TipType _type = _slot.GetTipType();
        ToolTipBase tooltip = m_toolTips[(int)_type]; //사용할 팁 프레임 
        //없으면 어쩔수없고
        if (tooltip == null)
            return;

        //설정된 UI는 어딘지 적고 진행
        tooltip.TipSwitch(true);
        tooltip.SetToolTip(_slot.GetItemBase());
    }

    //커서 포인트 아웃으로 진행되는 종료 
    public void CloseTip()
    {
        TipSwitch(false);
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

}
