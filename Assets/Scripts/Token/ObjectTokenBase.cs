using System.Collections;
using UnityEngine;
using AllIn1SpriteShader;
public enum CharState
{
    Idle, Move, Attack, Trace, Sleep
}

public class ObjectTokenBase : MonoBehaviour
{
    
    public TokenType m_tokenType;
    public string m_name;
    [SerializeField]
    private TokenBase m_token; //들어있는 토큰
    public CharHud m_hud;
    public SpriteRenderer m_charIcon;
    public SpriteRenderer m_actionIcon;
    CharState m_state = CharState.Idle;
    public Animator m_animator;
    public int m_ClickPriority = 0;
    public AllIn1Shader m_spriteMenu;

    public virtual void OnClickObject()
    {
        //클릭 매니저를 통해 오브젝트가 클릭되면
        //1. 해당 오브젝트 토큰에 클릭을 호출
        //2. 플레이어 매니저로 토큰 오브젝트가 클릭되었음을 알림 토큰 전달. 
        PlayerManager.g_instance.ClickTokenObject(m_token);
        
    }

    public void SyncObjectPosition(int _x, int _y)
    {
        //Token 의 위치 값 대로 오브젝트 위치를 동기화 
       // Debug.Log("<color=green> 오브젝트 위치도 이동"+_x+":"+_y+" </color>");
        if(MgToken.g_instance.GetMaps()[_x, _y].GetObject() != null)
        {
            transform.position = MgToken.g_instance.GetMaps()[_x, _y].GetObject().transform.position;
        }
        else
        {
            //오류
            Debug.LogError("타일 토큰에 오브젝트 할당이 안되있다.");
            transform.position = new Vector3(_x, _y, -1);
        }
        
    }

    public void SyncObjectPosition()
    {
        SyncObjectPosition(m_token.GetXIndex(), m_token.GetYIndex());
    }

    #region GetSet
    public virtual void SetObjectToken(TokenBase _token, TokenType _tokenType)
    {
        //토큰 타입에 따라 
        //1. 클릭 우선도
        //2. 렌더링 우선도 
        //3. token 클래스
        m_tokenType = _tokenType; // 해당 오브젝트의 토큰타입은 보유한 토큰타입으로 설정
        if(_tokenType == TokenType.Char)
        {
            m_token = (TokenChar)_token;
            m_ClickPriority = 3;
        }
        else if (_tokenType == TokenType.Tile)
        {
            m_token = (TokenTile)_token;
            m_ClickPriority = 1;
        }
        else if (_tokenType == TokenType.Event)
        {
            m_token = (TokenEvent)_token;
            m_ClickPriority = 2;
        }
        m_token.SetObject(this);
        m_name = _token.GetItemName();
    }

    public TokenBase GetToken()
    {
       return m_token;
    }

    public void SetHud(CharHud _hud)
    {
        m_hud = _hud;
    }

    public void SetSprite(Sprite _sprite)
    {
        m_charIcon.sprite = _sprite;
    }

    public CharHud GetHud()
    {
        return m_hud;
    }

    public int GetClickPriority()
    {
        //오브젝트의 경우 레이를 쏘기 때문에 여러 오브젝트가 감지 될 수 있음
        //이 중 tokenType별로 할당된 클릭우선도에 따라 대상을 선택
        return m_ClickPriority;
    }
    #endregion

    public void ShowWorkOrder(WorkOrder _workOrder)
    {

    }

    public void OffWorkOrder()
    {

    }


    public void ShowActionIcon(ActionType _action)
    {
        m_actionIcon.sprite = TempSpriteBox.GetInstance().CapitalLand;
        m_actionIcon.gameObject.SetActive(true);
    }
    public void OffActionIcon()
    {
        m_actionIcon.gameObject.SetActive(false);
    }
   
    public void PlayAnimation(CharState _state)
    {
        m_state = _state;
        m_animator.SetInteger("State", (int)m_state);
    }

    public void DestroyObject()
    {
        if (m_hud != null)
            m_hud.DestroyHud();
        gameObject.SetActive(false);
    }

    private void EffectCharObjEffect()
    {
     //   m_spriteMenu.
    }
}
