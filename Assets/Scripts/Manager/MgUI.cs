using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum UIType
{
    SkillTree, SeedInven, PlantTree, LandFinder, Quick, None, CropInfo, PlayerDisplay, PlayerStat, InterAction, Expander
}
public class MgUI : MgGeneric<MgUI>
{
    [SerializeField] private GameObject m_listBox;
    private UIBase[] m_UIes; //모든 UI를 받아놓은 곳
    private UIBase[] m_offTypeUIes; //All Off 캐스팅시 꺼질 UI만 모아놓은 곳
    [SerializeField] private UIBase[] m_mainUIes; //다른 어딘가로부터 OnOff를 요구 받을 만한 UI만 모아놓은 곳. 
    [SerializeField] private ToolTip m_toolTip;
    public GameObject m_InGameUI;
    public int m_opendCount = 0; //열려져있는 숫자 세기 .

    private void Awake()
    {
        g_instance = this;
        //m_UIes = m_listBox.GetComponentsInChildren<UIBase>();
        //ClassfyESCOffType();
        //ClassfyUIType();
    }

    public override void InitiSet()
    {
        base.InitiSet();
        InitiUi();
    }

    private void InitiUi()
    {
        //유아이들 초기 상태 세팅
        for (int i = 0; i < m_mainUIes.Length; i++)
        {
            m_mainUIes[i].InitiUI();
        }

    }
    #region 화면 조정
    public void IntroScene()
    {
        //m_InGameUI.SetActive(false);
        AllOff();
    }

    public void PlayScene()
    {
      //  m_InGameUI.SetActive(true);
        SetBaseState();
    }
    #endregion 

    #region UI 스위치 
    public void SwitchUI(UIType _uiType)
    {
         int state = m_mainUIes[(int)_uiType].Switch();
       m_opendCount += state;
        PlaceToFoward(m_mainUIes[(int)_uiType].m_window);
        if ((SwitchEnum)state == SwitchEnum.Off)
            m_toolTip.CallOfUI(_uiType);
   
    }

    public void SwitchUI(UIType _uiType, bool _state)
    { 
        int state = m_mainUIes[(int)_uiType].Switch(_state);
        m_opendCount += state;

        if ((SwitchEnum)state == SwitchEnum.Off)
            m_toolTip.CallOfUI(_uiType);
    }


    public void ESCOff()
    {
        for (int i = 0; i < m_offTypeUIes.Length; i++)
        {
            int state = m_offTypeUIes[i].Switch(false);
            m_opendCount += state;
        
            //켜져있던 툴팁도 꺼지도록 
            if ((SwitchEnum)state == SwitchEnum.Off)
                m_toolTip.CallOfUI(m_offTypeUIes[i].GetUIType());
                
        }
    }

    public void AllOff()
    {
        for (int i = 0; i < m_mainUIes.Length; i++)
        {
            if (m_mainUIes[i] == null)
                continue;

            int state = m_mainUIes[i].Switch(false);
            m_opendCount += state;

            //켜져있던 툴팁도 꺼지도록 
            if ((SwitchEnum)state == SwitchEnum.Off)
                m_toolTip.CallOfUI((UIType)i);
        }
    }

    public void SetBaseState()
    {
        for (int i = 0; i < m_mainUIes.Length; i++)
        {
            if (m_mainUIes[i] == null)
                continue;

            //메인 유아이들 중, 기본 유아이라면 켜서 진행. 
            m_mainUIes[i].Switch(m_mainUIes[i].m_isBaseUI);
        }
    }
    #endregion

    #region Tip 스위치
    public void ShowTip(SlotBase _slot)
    {
        m_toolTip.ShowTip(_slot);
    }

    public void CloseTip()
    {
        if(m_toolTip != null)
        m_toolTip.CloseTip();
    }
    #endregion

    #region UI 분류 - 단축키 지정 UI, Escape 대상 UI

    //단축키를 통해 OnOff시킬 만한 것들. 모든 UI가 대상이 될수도있으나 일단 MainUiEnum 을 통해, 해당 idx대로 정렬하기 위해 필요. 
    private void ClassfyUIType()
    {
        int mainUINum = System.Enum.GetValues(typeof(UIType)).Length;
        m_mainUIes = new UIBase[mainUINum]; //주요 메인 유아이 수만큼 배열 선언

        //타입에 따라 idx대로 mainUIes에 배정. - 이후 onOff 메서드 쓰기 위한 준비 
        for (int i = 0; i < m_UIes.Length; i++)
        {
            UIType type = m_UIes[i].GetUIType();
            if (type == UIType.None) //단축키 지정이 없는 UI라면 통과 
                continue;

            m_mainUIes[(int)type] = m_UIes[i]; //해당 enum의 idx대로, 배열에 현재 유아이를 할당 -> 이후 idx번째 유아이 onoff를 할때 쓸수있음.
        }
    }

    //escape로 끌 UI를 분류, 해당 UI가 열려있을 때는 스킬 사용도 멈춤. 
    private void ClassfyESCOffType()
    {
        List<UIBase>  insList = new List<UIBase>();
        for (int i = 0; i < m_UIes.Length; i++)
        {
            if (m_UIes[i].GetOffType().Equals(UIBase.ESCOffType.Yes))
                insList.Add(m_UIes[i]);

        }
        m_offTypeUIes = insList.ToArray();
    }


    #endregion

    public void PlaceToFoward(GameObject _window)
    {
        //해당 창을 UI 중 가장 맨앞으로 당기기.
        _window.transform.SetAsLastSibling();
    }

    public bool IsUIAllOff()
    {
        //UI적으로 작업중인지 체크해서 
        if (m_opendCount.Equals(0))
            return true;

        return false;
    }
}
