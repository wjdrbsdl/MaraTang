using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIResearch : UIBase
{
    //해당 플레이어가 지닌 자원을 종류 수량을 선택하는 부분. 
    [SerializeField]
    private ResearchSlot m_reserachSlotSample;
    [SerializeField]
    private Transform m_techGrid;
    [SerializeField]
    private ResearchSlot[] m_researchSlots;

    [SerializeField]
    private ResearchSlot m_labSlotSample;
    [SerializeField]
    private Transform m_labGrid;
    [SerializeField]
    private ResearchSlot[] m_labSlots;

    [SerializeField]
    private ResearchSlot m_selectSlot;
    [SerializeField]
    private ResearchSlot m_selectLabSlot;

    private NationTechData m_selectTech;
    private TokenTile m_selectTile;

    public void SetNationResearch(TokenTile _tile)
    {
        UISwitch(true);
        //국가 소유가 아닌 연구소는 애초 해당 UI가 열리지 않는다. 
        ResetSelect();
        Nation nation = _tile.GetNation();
        List<NationTechData> techList = MgMasterData.GetInstance().GetTechDic().Select(dic => dic.Value).ToList();
        List<TokenTile> labTileList = nation.GetPlace(TileType.Lab);
        //1. 아이템 수만큼 showSlot을 생성
        MakeSamplePool<ResearchSlot>(ref m_researchSlots, m_reserachSlotSample.gameObject, techList.Count, m_techGrid);
        //2. 리스트대로 슬랏에 표기
        SetTechSlots(techList, nation.TechPart);
        //3. 국가가 보유한 랩실 표기 
        MakeSamplePool<ResearchSlot>(ref m_labSlots, m_labSlotSample.gameObject, labTileList.Count, m_labGrid);
        //4. 
        SetLabSlots(labTileList);
    }

    public void ResetSelect()
    {
        m_selectSlot.ResetShowSlot();
        m_selectLabSlot.ResetShowSlot();
        m_selectTech = null;
        m_selectTile = null;
    }

    private void SetTechSlots(List<NationTechData> _techList, NationTechPart _nationTech)
    {
        int itemCount = _techList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            m_researchSlots[i].gameObject.SetActive(true);
            m_researchSlots[i].SetResearchSlot(_techList[i], _nationTech.IsDoneTech(_techList[i].GetPid()));
        }
        for (int i = itemCount; i < m_researchSlots.Length; i++)
        {
            m_researchSlots[i].gameObject.SetActive(false);
        }
    }

    private void SetLabSlots(List<TokenTile> _labList)
    {
        int itemCount = _labList.Count;

        for (int i = 0; i < itemCount; i++)
        {
            m_labSlots[i].gameObject.SetActive(true);
            m_labSlots[i].SetLabTile(_labList[i]);
        }
        for (int i = itemCount; i < m_labSlots.Length; i++)
        {
            m_labSlots[i].gameObject.SetActive(false);
        }
    }
    public void SelectReserach(NationTechData _techData)
    {
       // Debug.Log((TechEnum)_techData.GetPid() + "배우겠다고 클릭");
        m_selectSlot.SetResearchSelect(_techData);
        m_selectTech = _techData;
    }
    public void SelectLab(TokenTile _labTile)
    {
       // Debug.Log(_labTile.GetMapIndex()[0] + "-" + _labTile.GetMapIndex()[1] + "연구소 선택");
        m_selectLabSlot.SetLabSelect(_labTile);
        m_selectTile = _labTile;
    }

    public void OnClickConfirm()
    {
        if (m_selectTech == null)
            return;

        if (m_selectTile == null)
            return;

        
        NationTechData techData = m_selectTech;
        TItemListData changeCost = techData.ResearchCostData;
        TokenTile workTile = m_selectTile;
        Debug.Log(workTile.GetNation().GetNationNum()+"번 국가 연구소에서"+ (TechEnum)techData.GetPid() + "연구 시작");
        new WorkOrder(changeCost.GetItemList(), techData.NeedTurn, techData.NeedLabor, workTile, techData.GetPid(), WorkType.Research);
        UISwitch(false);
    }
}

