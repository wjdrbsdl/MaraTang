using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nation
{
    private TokenTile m_capitalCity;
    private List<TokenTile> m_territorryList;
    private int[] m_resources;

    #region 국가 생성자
    public Nation()
    {
        m_territorryList = new();
    }

    public Nation MakeNewNation(TokenTile _capitalCity)
    {
        Nation nation = new();
        nation.SetCapitalCity(_capitalCity);
        nation.AddTerritory(_capitalCity);
        //주변 1칸은 기본적으로 해당 국가 영토로 편입
        List<TokenTile> boundaryTile = GameUtil.GetTileTokenListInRange(1, _capitalCity.GetXIndex(), _capitalCity.GetYIndex(),1);
        for (int i = 0; i < boundaryTile.Count; i++)
        {
            nation.AddTerritory(boundaryTile[i]);
        }
        nation.m_resources = new int[GameUtil.EnumLength(Capital.Food)];
        return nation;
    }

    public void Destroy()
    {
        //국가 소멸시 할 작업들. 
    }
    #endregion

    #region 국가 영토
    public void AddTerritory(TokenTile _tileToken)
    {
        //이미 있는 영토
        if(m_territorryList.IndexOf(_tileToken)>= 0)
                return;

        m_territorryList.Add(_tileToken);
    }

    public void RemoveTerritory(TokenTile _tileToken)
    {
        m_territorryList.Remove(_tileToken);
    }
    #endregion

    public void ManageNation()
    {
        //국가운영 
        int[] idx = m_capitalCity.GetMapIndex();
     //   Debug.Log(idx[0] + " " + idx[1] + "번 국가 운영 시작");

    }

    public void SetCapitalCity(TokenTile _tileToken)
    {
        m_capitalCity = _tileToken;
    }

    public TokenTile GetCapital()
    {
        return m_capitalCity;
    }

    public void ShowTerritory()
    {
        for (int i = 1; i < m_territorryList.Count; i++)
        {
            TokenTile tile = m_territorryList[i];
            tile.Dye(Color.red);
         //   Debug.Log("해당 타일의 타입은 " + tile.GetTileType());
        }
    }
}
