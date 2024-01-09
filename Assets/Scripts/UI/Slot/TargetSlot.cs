using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSlot : SlotBase
{
    //�׼� ��ū ���� Ÿ�� ���� 
    public Sprite m_tileSprite;
    public Sprite m_charSprite;

    public override void SetSlot(TokenBase _token)
    {
        base.SetSlot(_token);

        if (_token.GetTokenType().Equals(TokenType.Tile))
            m_icon.sprite = m_tileSprite;
        else if (_token.GetTokenType().Equals(TokenType.Char))
            m_icon.sprite = m_charSprite;
    }
}
