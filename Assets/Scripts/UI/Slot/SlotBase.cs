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
    [SerializeField] protected SlotType m_slotType;//�ν����Ϳ��� ����, 
    [SerializeField] public UIType m_uiType;//�ν����Ϳ��� ���� : �ڽ��� ���� UI �׷�
    [SerializeField] public TipType m_tipType;//�ν����Ϳ��� ���� : �ڽ��� Ȱ���� Tip Type

    private void Awake()
    {
        if(m_icon == null)
        m_icon = GetComponent<Image>();
    }

    private void Start()
    {
        //���õ� �������� ������ Ŭ��� ����. 
        if (m_token == null)
        {
            if(m_tierImage!=null)
                m_tierImage.enabled = false;
            if (m_icon!= null)
                m_icon.sprite = null;
        }
            
    }

  
    #region slot ���� ����
    public virtual void SetSlot(TokenBase _token)
    {
        ClearSlot(); //���� ������ ����?

        m_token = _token;
        m_icon.sprite = _token.GetIcon();
        //SetTier(_token.GetTier());
        //DisplayCool(0, 1); //icon�� ��Ÿ�� ǥ�⸦ �ϴ� �������� ����. 
        //m_token.e_coolTimer += DisplayCool; //�ش� �����ۿ��� ������ �޵��� �� �Լ��� ����
        
    }

    //�ʱ�ȭ 
    public virtual void ClearSlot()
    {
        //�ݺ����� Ŭ��� ���� ���� 
        if (m_token == null)
            return;

        m_token.e_coolTimer -= DisplayCool; //�ش� �����ۿ� �־��� �� �Լ� ����. 
        m_icon.sprite = null;
        m_token = null;
        if(m_tierImage != null)
        m_tierImage.enabled = false;

    }

    protected void SetFromSlot(TokenBase _changedItem)
    {
        // Debug.Log("����� ���� ��ȭ ����");
        if (_changedItem != null)
        {
            DragSlot.instance.dragslot.SetSlot(_changedItem);
        }
        else
        {
            DragSlot.instance.dragslot.ClearSlot();
        }

    }

    //Ȱ����, ��Ÿ�� ���ο� ���� ���İ�
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

    #region �巡��
    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        //�ϴ� ��� ���Կ��� ��������س��µ�, �� ���Կ��� ������ �ǰ� ���� ������ �ش� �޼��带 �������̵��ϰ� �������� �θ� ��. 
        if (m_token != null)
        {
            Debug.Log("���� �巡��");
            StartDrag();
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        //�巡�� ������ �Ҵ�Ȱ�쿡�� - �� �巡�׽����� Ȱ��ȭ�� ��� �� �̹����� Ŀ���� �ѾƴٴҰ�. 
        if (DragSlot.instance.dragslot != null)
        {
            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("���� ���� ����Ǿ���.");
        //�巡���� ������ ������� ������, ����� ���ۿ��ΰ� �ƴҶ� ����
        if (DragSlot.instance.dragslot != null)
        {
            IndividualDrop();
        }

     
    }

    public virtual void IndividualDrop()
    {
        Debug.Log("SlotBase ������ ���, �ƹ��ϵ� �Ͼ�� ����");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetDragSlot();
    }

    public void ResetDragSlot()
    {
        //Debug.Log("End ����Ǿ���.");
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

    #region Ŀ�� ��, �ƿ�
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
