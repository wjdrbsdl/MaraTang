using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] public GameObject m_window;
    public bool IsOnType = false; //m_window를 끄는 타입인가. 
    public bool IsStackType = true; //switch로 온오프시 uiStack에 관계되는 타입인가 
    private void Start()
    {
        InitiUI();
    }

    public virtual void InitiUI()
    {
        m_window.SetActive(IsOnType); //유아이를 동작하면 window를 Switch()로 활성화 시키는데, 하이어키에서 켜놓은상태로 시작하면 동작이 안됨.
    }

    public void Switch()
    {
        //전달 받은대로 유아이 화면을 켜고 끄기, 무조건 크거나 끄거나 동작됨
        bool on = !m_window.activeSelf;
        m_window.SetActive(on);
        if(on== false)
            OffWindow();
   }

    public void UISwitch(bool _on)
    {
        //만약 명명한 상태가 현재 상태랑 동일하다면 끝
        if (m_window.activeSelf == _on)
            return;

        m_window.SetActive(_on); //아니면 상태를 선택한 상태로 바꿈
        if (_on == false)
            OffWindow();
        else
        {
            OnWindow();
        }
    }

    public virtual void OffWindow()
    {
        MgUI.GetInstance().PopUIStack();
    }

    public virtual void OnWindow()
    {
        //켜질때면
        if(IsStackType)
        MgUI.GetInstance().PushUIStack(this); //해당 유아이를 넣어보자. 
    }

    public virtual void RequestOpen()
    {
        UISwitch(true);
    }

    public virtual void ReqeustOff()
    {
        //다른사정없으면 기본적으로 스위치 종료. 
        UISwitch(false);
    }

    #region UI Slot 셋팅
    protected void MakeSamplePool<T>(ref T[] _curArray, GameObject _sampleObj, int _workCount, Transform _box)
    {
        int needSlotCount = MakeCount<T>(_curArray, _workCount);
        if (needSlotCount > 0)
        {
            MakeSlots<T>(ref _curArray, needSlotCount, _sampleObj, _box);
        }

    }
    private int MakeCount<T>(T[] _curArray, int _goalCount)
    {
        if (_curArray == null || _curArray.Length == 0)
            return _goalCount;

        int makeCount = _goalCount - _curArray.Length; //현재 만들어진 카운터
        if (makeCount < 0)
            makeCount = 0;
        
        return makeCount;
    }

    //수량에 맞춰서 슬랏을 만들고 해당 슬랏 정보를 가져와야할때. 
    private void MakeSlots<T>(ref T[] _curArray, int _makeCount, GameObject _slotPrefeb, Transform _parent)
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

    protected void SetTrashZone()
    {
        MgUI.GetInstance().GetTrashZone().SetTrashZone(this.gameObject);
    }
}
