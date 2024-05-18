using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nation
{
    private TokenTile m_capital;
    private List<TokenTile> m_territorryList;

    public void AddTerritory(TokenTile _tileToken)
    {
        //이미 있는 영토
        if(m_territorryList.IndexOf(_tileToken)>= 0)
                return;

        m_territorryList.Add(_tileToken);
    }


}
