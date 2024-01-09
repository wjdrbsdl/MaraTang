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
    private UIType m_currentUI = UIType.None; //현재 툴팁대상이있는가. 
    //툴을 슬롯의 팁타입에 따라 즉 슬롯이 다르더라도 필요한 팁타입이 같다면, 해당 팁타입으로 슬랏에 있는 아이템을 설명하는 방식. 
    //이후 팁 GameObj의 위치도 표시되는 UI의 내부에 위치하기 위해 MGUI를 통해 호출. 
    //준비방식은 프레임이 있어야하는데 그게 ToolTipBase를 오버라이딩한녀석들. 그 베이스대로 진행. 

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

    //해당 타입의 유아이가 꺼졌다고 알려줌
    public void CallOfUI(UIType _ui)
    {
        if (m_currentUI == _ui)
            CloseTip();
    }

    public void ShowTip(SlotBase _slot)
    {
        TipType _type = _slot.GetTipType();
        ToolTipBase tooltip = m_toolTips[(int)_type]; //사용할 팁 프레임 
        //없으면 어쩔수없고
        if (tooltip == null)
            return;

        //설정된 UI는 어딘지 적고 진행
        SetTargetUI(_slot.m_uiType);
        tooltip.TipSwitch(true);
        tooltip.SetToolTip(_slot.GetItemBase());
    }

    //커서 포인트 아웃으로 진행되는 종료 
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
