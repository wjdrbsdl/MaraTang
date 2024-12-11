using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum ReserchSlotFunction
{
    Select, Show, Lab
}

public class ResearchSlot : SlotBase
{
    public ReserchSlotFunction m_function = ReserchSlotFunction.Select; //해당 슬랏 기능 
    private NationTechData m_techData; //할당된 스킬 pid
    private bool m_isComplete = false; 
    [SerializeField]
    private UIResearch m_reserachUI;
    public TMP_Text reserchText; //나중에 아이콘으로 대체될부분

    private TokenTile m_labTile;

    public void SetResearchSlot(NationTechData _techData, bool _done)
    {
        GetImage().color = Color.white;
        m_techData = _techData;
        m_isComplete = _done;
        reserchText.text = _techData.GetTechName();
        if (_done == false)
            GetImage().color = Color.gray;
    }

    public void SetLabTile(TokenTile _tile)
    {
        GetImage().color = Color.white;
        m_labTile = _tile;
        reserchText.text = _tile.GetMapIndex()[0] + "-" + _tile.GetMapIndex()[1]+"연구소";
        if (_tile.GetWorkOrder() != null)
            GetImage().color = Color.gray;
    }

    public void SetResearchSelect(NationTechData _techData)
    {
        gameObject.SetActive(true);
        m_techData = _techData;
        reserchText.text = _techData.GetTechName();
    }

    public void SetLabSelect(TokenTile _tile)
    {
        m_labTile = _tile;
        reserchText.text = _tile.GetMapIndex()[0] + "-" + _tile.GetMapIndex()[1] + "연구소";
    }

    public void ResetShowSlot()
    {
        m_techData = null;
        m_isComplete = false;
        m_labTile = null;
        reserchText.text = "";
    }

    public override void OnLeftClick()
    {
        //연구 고르는 파트면, 아직 연구안된 연구 고른경우 진행
        if (m_function == ReserchSlotFunction.Select && m_isComplete == false)
        {
            m_reserachUI.SelectReserach(m_techData);
            return;
        }
                
        if(m_function == ReserchSlotFunction.Lab && m_labTile.GetWorkOrder() == null)
        {
            m_reserachUI.SelectLab(m_labTile);
        }


        //안 배운경우에만 배우겠다고 의사전달 


    }
}

