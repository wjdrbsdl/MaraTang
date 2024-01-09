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
    private UIBase[] m_UIes; //��� UI�� �޾Ƴ��� ��
    private UIBase[] m_offTypeUIes; //All Off ĳ���ý� ���� UI�� ��Ƴ��� ��
    [SerializeField] private UIBase[] m_mainUIes; //�ٸ� ��򰡷κ��� OnOff�� �䱸 ���� ���� UI�� ��Ƴ��� ��. 
    [SerializeField] private ToolTip m_toolTip;
    public GameObject m_InGameUI;
    public int m_opendCount = 0; //�������ִ� ���� ���� .

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
        //�����̵� �ʱ� ���� ����
        for (int i = 0; i < m_mainUIes.Length; i++)
        {
            m_mainUIes[i].InitiUI();
        }

    }
    #region ȭ�� ����
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

    #region UI ����ġ 
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
        
            //�����ִ� ������ �������� 
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

            //�����ִ� ������ �������� 
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

            //���� �����̵� ��, �⺻ �����̶�� �Ѽ� ����. 
            m_mainUIes[i].Switch(m_mainUIes[i].m_isBaseUI);
        }
    }
    #endregion

    #region Tip ����ġ
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

    #region UI �з� - ����Ű ���� UI, Escape ��� UI

    //����Ű�� ���� OnOff��ų ���� �͵�. ��� UI�� ����� �ɼ��������� �ϴ� MainUiEnum �� ����, �ش� idx��� �����ϱ� ���� �ʿ�. 
    private void ClassfyUIType()
    {
        int mainUINum = System.Enum.GetValues(typeof(UIType)).Length;
        m_mainUIes = new UIBase[mainUINum]; //�ֿ� ���� ������ ����ŭ �迭 ����

        //Ÿ�Կ� ���� idx��� mainUIes�� ����. - ���� onOff �޼��� ���� ���� �غ� 
        for (int i = 0; i < m_UIes.Length; i++)
        {
            UIType type = m_UIes[i].GetUIType();
            if (type == UIType.None) //����Ű ������ ���� UI��� ��� 
                continue;

            m_mainUIes[(int)type] = m_UIes[i]; //�ش� enum�� idx���, �迭�� ���� �����̸� �Ҵ� -> ���� idx��° ������ onoff�� �Ҷ� ��������.
        }
    }

    //escape�� �� UI�� �з�, �ش� UI�� �������� ���� ��ų ��뵵 ����. 
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
        //�ش� â�� UI �� ���� �Ǿ����� ����.
        _window.transform.SetAsLastSibling();
    }

    public bool IsUIAllOff()
    {
        //UI������ �۾������� üũ�ؼ� 
        if (m_opendCount.Equals(0))
            return true;

        return false;
    }
}
