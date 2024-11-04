using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class TokenTileAction : TokenBase
{
    [SerializeField]
    public bool IsAutoStart = false;

    #region 액션 토큰 : 생성부분 추후 테이블 파싱 값으로 생성하기
 
    public TokenTileAction(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]);
        m_itemName = valueCode[1];
        IsAutoStart = (int.Parse(valueCode[2]) != 0);

        m_tokenIValues = new int[System.Enum.GetValues(typeof(TileActionStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
    }
    #endregion
}
