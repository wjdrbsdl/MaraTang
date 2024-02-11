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
  
    [SerializeField] protected UIType m_uiType = UIType.None; //기본 none;
    [SerializeField] public GameObject m_window;
    [SerializeField] private ESCOffType m_allOffType;
    [SerializeField] public bool m_isBaseUI = false; //상시 켜져있는 녀석인가. 

    [SerializeField] private PauseReason m_pauseReason = PauseReason.None;
    public int Switch()
    {
        //전달 받은대로 유아이 화면을 켜고 끄기, 무조건 크거나 끄거나 동작됨
        bool on = !m_window.activeSelf;
        m_window.SetActive(on);
        if(on== false)
            OffWindow();
        ApplyPause(on);
        return ReturnSwitchValue(on);

    }

    public int Switch(bool _on)
    {
        //만약 명명한 상태가 현재 상태랑 동일하다면 0을 반환
        if (m_window.activeSelf == _on)
            return (int)SwitchEnum.Stay;

        m_window.SetActive(_on); //아니면 상태를 _on상태로 바꾸고 바꾼값을 반환 
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
        //사유가 없다면 관계없이 종료

        if (m_pauseReason.Equals(PauseReason.None))
            return;

        //사유가 있다면, 켜진다면 Pause 꺼진다면 Play로 진행
        if (_on)
            SystemPause.g_instance.Pause(m_pauseReason);
        else
            SystemPause.g_instance.Play(m_pauseReason);
    }

    private int ReturnSwitchValue(bool _on)
    {
        if (m_allOffType.Equals(ESCOffType.No))
            return 0; //만약 esc로 꺼지고 조정되는 타입이 아니라면 그냥 0 반환 

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

    #region UI Slot 셋팅
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

        int makeCount = _goalCount - _curArray.Length; //현재 만들어진 카운터
        if (makeCount < 0)
            makeCount = 0;
        
        return makeCount;
    }

    //수량에 맞춰서 슬랏을 만들고 해당 슬랏 정보를 가져와야할때. 
    protected void MakeSlots<T>(ref T[] _curArray, int _makeCount, GameObject _slotPrefeb, Transform _parent)
    {
        List<T> newT = new();
        if (_curArray != null)
            newT = new(_curArray); //기존 만들어진 배열이 있으면 리스트에 추가해야함. 

        for (int i = 0; i < _makeCount; i++)
        {
          newT.Add(  Instantiate(_slotPrefeb, _parent).GetComponent<T>());

        }

        _curArray = newT.ToArray();
    }

    protected void RegisterSlots<T>(GameObject _parent, ref T[] _slots)
    {
        m_window.SetActive(true); //윈도우창 활성화 해서 slot 컴포넌트 가져올수 있도록
        _slots = _parent.GetComponentsInChildren<T>();
        m_window.SetActive(false); //컴포넌트 담았으면 이제 꺼도 됨
    }
    #endregion

    public virtual void InitiUI()
    {

    }
}
