using System.Collections;
using UnityEngine;
public enum CharState
{
    Idle, Move, Swing
}

public class ObjectTokenBase : MonoBehaviour
{
    
    public TokenType m_tokenType;
    [SerializeField]
    public TokenBase m_token; //들어있는 토큰
    public SpriteRenderer m_charIcon;
    public SpriteRenderer m_actionIcon;
    CharState m_state = CharState.Idle;
    public Animator m_animator;
    public int m_ClickPriority = 0;
    public bool m_testPlayerToken = false;

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
        Debug.Log("<color=green> 오브젝트 위치도 이동"+_x+":"+_y+" </color>");
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

    public virtual void SetToken(TokenBase _token, TokenType _tokenType)
    {
        m_tokenType = _tokenType; // 해당 오브젝트의 토큰타입은 보유한 토큰타입으로 설정
        if(_tokenType == TokenType.Char)
        {
            m_token = (TokenChar)_token;
            m_ClickPriority = 2;
        }
        else if (_tokenType == TokenType.Tile)
        {
            m_token = (TokenTile)_token;
            m_ClickPriority = 1;
        }
        else if (_tokenType == TokenType.Action)
        {
            m_token = (TokenAction)_token;
        }
        m_token.SetObject(this);
    }

    public TokenBase GetToken()
    {
        switch (m_tokenType)
        {
            case TokenType.Player:
                return (TokenChar)m_token;
            case TokenType.Tile:
                return (TokenTile)m_token;
            case TokenType.Action:
                return (TokenAction)m_token;
            case TokenType.Char:
                return (TokenChar)m_token;
        }
        return null;
    }

    public int GetClickPrior()
    {
        return m_ClickPriority;
    }

    public void ShowActionIcon(ActionType _action)
    {
        m_actionIcon.sprite = GamePlayMaster.g_instance.m_testActionIcon[(int)_action];
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
}
