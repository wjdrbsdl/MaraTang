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

    //인스턴스 해서 타 클래스에서 DragSlot.instance 로 바로 간편하게 사용할 수 있게 해주는 부분. 
    //클래스에 변수 그리고 컴포넌트 받는 과정을 줄이는 코드.
    private void Awake()
    {
        instance = this;
        SetOff(false);        
    }
    //실제 사용하는 함수들은 2개
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
