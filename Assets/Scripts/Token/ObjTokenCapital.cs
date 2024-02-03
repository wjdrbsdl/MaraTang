using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTokenCapital : ObjectTokenBase
{

    public override void SetToken(TokenBase _token, TokenType _tokenType)
    {
        base.SetToken(_token, _tokenType);
        gameObject.SetActive(true);
    }
}
