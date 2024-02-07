using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UITokenSnapInfo : UIBase
{
    [SerializeField]
    private TMP_Text m_Info;

    public void SetTokenSnapInfo(TokenBase _token)
    {
        TokenType type = _token.GetTokenType();
        if (type.Equals(TokenType.Char))
        {
            TokenChar charToken = (TokenChar)_token;
            SetCharTokenInfo(charToken);
            return;
        }
        if (type.Equals(TokenType.Tile))
        {
            TokenTile tileToken = (TokenTile)_token;
            SetTileTokenInfo(tileToken);
            return;
        }
    }

    private void SetCharTokenInfo(TokenChar _tokenChar)
    {
        int maxActCount = _tokenChar.GetStat(CharStat.MaxActionCount);
        int curActCount = _tokenChar.GetStat(CharStat.CurActionCount);
        m_Info.text =("char" + curActCount +" / "+maxActCount);
    }

    private void SetTileTokenInfo(TokenTile _tokenTile)
    {
        m_Info.text = ("tile");
    }
}
