using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwitchEnum
{
    Off = -1, Stay, On
}

public class UIBase : MonoBehaviour
{
    public enum ESCOffType
    {
        Yes, No
    }
  
    [SerializeField] protected UIType m_uiType = UIType.None; //�⺻ none;
    [SerializeField] public GameObject m_window;
    [SerializeField] private ESCOffType m_allOffType;
    [SerializeField] public bool m_isBaseUI = false; //��� �����ִ� �༮�ΰ�. 

    [SerializeField] private PauseReason m_pauseReason = PauseReason.None;
    public int Switch()
    {
        //���� ������� ������ ȭ���� �Ѱ� ����, ������ ũ�ų� ���ų� ���۵�
        bool on = !m_window.activeSelf;
        m_window.SetActive(on);
        if(on== false)
            OffWindow();
        ApplyPause(on);
        return ReturnSwitchValue(on);

    }

    public int Switch(bool _on)
    {
        //���� ����� ���°� ���� ���¶� �����ϴٸ� 0�� ��ȯ
        if (m_window.activeSelf == _on)
            return (int)SwitchEnum.Stay;

        m_window.SetActive(_on); //�ƴϸ� ���¸� _on���·� �ٲٰ� �ٲ۰��� ��ȯ 
        if (_on == false)
            OffWindow();

        ApplyPause(_on);
        return ReturnSwitchValue(_on);
    }

    public virtual void OffWindow()
    {

    }

    private void ApplyPause(bool _on)
    {
        //������ ���ٸ� ������� ����

        if (m_pauseReason.Equals(PauseReason.None))
            return;

        //������ �ִٸ�, �����ٸ� Pause �����ٸ� Play�� ����
        if (_on)
            SystemPause.g_instance.Pause(m_pauseReason);
        else
            SystemPause.g_instance.Play(m_pauseReason);
    }

    private int ReturnSwitchValue(bool _on)
    {
        if (m_allOffType.Equals(ESCOffType.No))
            return 0; //���� esc�� ������ �����Ǵ� Ÿ���� �ƴ϶�� �׳� 0 ��ȯ 

        if (_on)
        {
            return (int)SwitchEnum.On;
        }
            

        return (int)SwitchEnum.Off;
    }

    #region Get Set
    public ESCOffType GetOffType()
    {
        return m_allOffType;
    }

    public UIType GetUIType()
    {
        return m_uiType;
    }
    #endregion

    #region UI Slot ����
    protected void MakeSamplePool<T>(ref T[] _curArray, GameObject _sampleObj, int _workCount, Transform _box)
    {
        int needSlotCount = MakeCount<T>(_curArray, _workCount);
        if (needSlotCount > 0)
        {
            MakeSlots<T>(ref _curArray, needSlotCount, _sampleObj, _box);
        }

    }

    protected int MakeCount<T>(T[] _curArray, int _goalCount)
    {
        if (_curArray == null || _curArray.Length == 0)
            return _goalCount;

        int makeCount = _goalCount - _curArray.Length; //���� ������� ī����
        if (makeCount < 0)
            makeCount = 0;
        
        return makeCount;
    }

    //������ ���缭 ������ ����� �ش� ���� ������ �����;��Ҷ�. 
    protected void MakeSlots<T>(ref T[] _curArray, int _makeCount, GameObject _slotPrefeb, Transform _parent)
    {
        List<T> newT = new();
        if (_curArray != null)
            newT = new(_curArray); //���� ������� �迭�� ������ ����Ʈ�� �߰��ؾ���. 

        for (int i = 0; i < _makeCount; i++)
        {
          newT.Add(  Instantiate(_slotPrefeb, _parent).GetComponent<T>());

        }

        _curArray = newT.ToArray();
    }

    protected void RegisterSlots<T>(GameObject _parent, ref T[] _slots)
    {
        m_window.SetActive(true); //������â Ȱ��ȭ �ؼ� slot ������Ʈ �����ü� �ֵ���
        _slots = _parent.GetComponentsInChildren<T>();
        m_window.SetActive(false); //������Ʈ ������� ���� ���� ��
    }
    #endregion

    public virtual void InitiUI()
    {

    }
}
