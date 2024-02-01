using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragSlot : MonoBehaviour
{
    public static DragSlot instance;

    public SlotBase dragslot;

    [SerializeField]
    private Image m_icon;

    //�ν��Ͻ� �ؼ� Ÿ Ŭ�������� DragSlot.instance �� �ٷ� �����ϰ� ����� �� �ְ� ���ִ� �κ�. 
    //Ŭ������ ���� �׸��� ������Ʈ �޴� ������ ���̴� �ڵ�.
    private void Awake()
    {
        instance = this;
        SetOff(false);        
    }
    //���� ����ϴ� �Լ����� 2��
    public void DragSetImage(Image _itemimage)
    {
        m_icon.sprite = _itemimage.sprite;
       
    }
    public void SetColor(float _num)
    {
        Color color = m_icon.color;
        color.a = _num;
        m_icon.color = color;

    }
    public void SetOff(bool onOff)
    {
        gameObject.SetActive(onOff);
    }
}
