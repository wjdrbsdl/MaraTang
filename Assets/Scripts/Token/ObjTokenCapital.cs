using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTokenCapital : ObjectTokenBase
{

    public override void SetObjectToken(TokenBase _token, TokenType _tokenType)
    {
        base.SetObjectToken(_token, _tokenType);
        gameObject.SetActive(true);
    }
}
