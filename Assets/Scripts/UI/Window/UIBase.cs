using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] public GameObject m_window;
    public bool IsOnType = false; //m_window�� ���� Ÿ���ΰ�. 
    public bool IsStackType = true; //switch�� �¿����� uiStack�� ����Ǵ� Ÿ���ΰ� 
    private void Start()
    {
        InitiUI();
    }

    public virtual void InitiUI()
    {
        m_window.SetActive(IsOnType); //�����̸� �����ϸ� window�� Switch()�� Ȱ��ȭ ��Ű�µ�, ���̾�Ű���� �ѳ������·� �����ϸ� ������ �ȵ�.
    }

    public void Switch()
    {
        //���� ������� ������ ȭ���� �Ѱ� ����, ������ ũ�ų� ���ų� ���۵�
        bool on = !m_window.activeSelf;
        m_window.SetActive(on);
        if(on== false)
            OffWindow();
   }

    public void UISwitch(bool _on)
    {
        //���� ����� ���°� ���� ���¶� �����ϴٸ� ��
        if (m_window.activeSelf == _on)
            return;

        m_window.SetActive(_on); //�ƴϸ� ���¸� ������ ���·� �ٲ�
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
        //��������
        if(IsStackType)
        MgUI.GetInstance().PushUIStack(this); //�ش� �����̸� �־��. 
    }

    public virtual void RequestOpen()
    {
        UISwitch(true);
    }

    public virtual void ReqeustOff()
    {
        //�ٸ����������� �⺻������ ����ġ ����. 
        UISwitch(false);
    }

    #region UI Slot ����
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

        int makeCount = _goalCount - _curArray.Length; //���� ������� ī����
        if (makeCount < 0)
            makeCount = 0;
        
        return makeCount;
    }

    //������ ���缭 ������ ����� �ش� ���� ������ �����;��Ҷ�. 
    private void MakeSlots<T>(ref T[] _curArray, int _makeCount, GameObject _slotPrefeb, Transform _parent)
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

    protected void SetTrashZone()
    {
        MgUI.GetInstance().GetTrashZone().SetTrashZone(this.gameObject);
    }
}
