using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTokenObject : ObjectTokenBase
{

    public override void SetToken(TokenBase _token, TokenType _tokenType)
    {
        base.SetToken(_token, _tokenType);
        gameObject.SetActive(true);
    }

    public override void OnClickObject()
    {
        PlayerManager.GetInstance().SelectActionToken(m_token);
    }
}
