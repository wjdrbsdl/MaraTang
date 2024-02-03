using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum SlotType
{
    Skill, Quick, Land, Seed, Craft, Authored, MyLand
}

public class SlotBase : MonoBehaviour, IDragHandler, IBeginDragHandler, IDropHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    protected TokenBase m_token;
    [SerializeField] protected Image m_icon;
    [SerializeField] protected Image m_tierImage;
    [SerializeField] protected SlotType m_slotType;//인스펙터에서 설정, 
    [SerializeField] public UIType m_uiType;//인스펙터에서 설정 : 자신이 속한 UI 그룹
    [SerializeField] public TipType m_tipType;//인스펙터에서 설정 : 자신이 활용할 Tip Type

    private void Awake()
    {
        if(m_icon == null)
        m_icon = GetComponent<Image>();
    }

    private void Start()
    {
        //세팅된 아이템이 없으면 클리어를 진행. 
        if (m_token == null)
        {
            if(m_tierImage!=null)
                m_tierImage.enabled = false;
            if (m_icon!= null)
                m_icon.sprite = null;
        }
            
    }

  
    #region slot 정보 세팅
    public virtual void SetSlot(TokenBase _token)
    {
        ClearSlot(); //먼저 비우고나서 진행?

        m_token = _token;
        m_icon.sprite = _token.GetIcon();
        //SetTier(_token.GetTier());
        //DisplayCool(0, 1); //icon의 쿨타임 표기를 일단 가득으로 진행. 
        //m_token.e_coolTimer += DisplayCool; //해당 아이템에만 영향을 받도록 쿨 함수를 넣음
        
    }

    //초기화 
    public virtual void ClearSlot()
    {
        //반복적인 클리어를 막기 위해 
        if (m_token == null)
            return;

        m_token.e_coolTimer -= DisplayCool; //해당 아이템에 넣었던 쿨 함수 제거. 
        m_icon.sprite = null;
        m_token = null;
        if(m_tierImage != null)
        m_tierImage.enabled = false;

    }

    protected void SetFromSlot(TokenBase _changedItem)
    {
        // Debug.Log("출발지 슬롯 정화 시작");
        if (_changedItem != null)
        {
            DragSlot.instance.dragslot.SetSlot(_changedItem);
        }
        else
        {
            DragSlot.instance.dragslot.ClearSlot();
        }

    }

    //활성도, 쿨타임 여부에 따라 알파값
    protected void SetAlpha(float _value)
    {
        Color alpha = m_icon.color;
        alpha.a = _value;
        m_icon.color = alpha;
    }

    protected void DisplayCool(float _remain, float _cool)
    {
        m_icon.fillAmount = (_cool - _remain) / _cool;
    }

    private void SetTier(eTokenTier _tier)
    {
        m_tierImage.enabled = true;
        switch (_tier)
        {
            case eTokenTier.Nomal:
                SetColor(Color.white);
                break;
            case eTokenTier.Magic:
                SetColor(Color.green);
                break;
            case eTokenTier.Rare:
                SetColor(Color.blue);
                break;
            case eTokenTier.Unique:
                SetColor(Color.magenta);
                break;
            case eTokenTier.Legend:
                SetColor(Color.yellow);
                break;
        }
    }

    private void SetColor(Color _color)
    {
        Color color = m_tierImage.color;
        color = _color;
        m_tierImage.color = color;
    }

    #endregion

    #region 드래그
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        //일단 모든 슬롯에서 가능토록해놨는데, 각 슬롯에서 시작이 되고 싶지 않으면 해당 메서드를 오버라이딩하고 공백으로 두면 됨. 
        if (m_token != null)
        {
            Debug.Log("공용 드래그");
            StartDrag();
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //드래그 슬랏에 할당된경우에는 - 즉 드래그슬랏이 활성화된 경우 그 이미지는 커서를 쫓아다닐것. 
        if (DragSlot.instance.dragslot != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("슬롯 위에 드랍되었다.");
        //드래그한 슬롯이 비어잇지 않으며, 대상이 제작여부가 아닐때 동작
        if (DragSlot.instance.dragslot != null)
        {
            IndividualDrop();
        }

     
    }

    public virtual void IndividualDrop()
    {
        Debug.Log("SlotBase 레벨의 드롭, 아무일도 일어나지 않음");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetDragSlot();
    }

    public void ResetDragSlot()
    {
        //Debug.Log("End 드랍되었다.");
        DragSlot.instance.SetOff(false);
    }

    public void StartDrag()
    {
        DragSlot.instance.dragslot = this;
        DragSlot.instance.DragSetImage(m_icon);
        DragSlot.instance.transform.position = transform.position;
        DragSlot.instance.SetOff(true);
    }
    #endregion

    #region 커서 인, 아웃
    public void OnPointerEnter(PointerEventData eventData)
    {
  
    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    #endregion

    public virtual void OnLeftClick()
    {

    }

    #region Get Set
    public TipType GetTipType()
    {
        return m_tipType;
    }

    public SlotType GetSlotType()
    {
        return m_slotType;
    }

    public TokenBase GetItemBase()
    {
        return m_token;
    }

    public Image GetImage()
    {
        return m_icon;
    }

    #endregion
}
