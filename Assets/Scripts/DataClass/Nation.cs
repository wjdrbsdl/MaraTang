using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nation
{
    private int m_nationNumber;
    private TokenTile m_capitalCity;
    private List<TokenTile> m_territorryList;
    private int[] m_resources;
    static Color[] nationColor = { Color.red, Color.yellow, Color.black };

    #region 국가 생성자
    public Nation()
    {
        m_territorryList = new();
    }

    public Nation MakeNewNation(TokenTile _capitalCity, int _nationNuber)
    {
        Nation nation = new();
        nation.m_nationNumber = _nationNuber;
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
        System.Text.StringBuilder valueReport = new System.Text.StringBuilder();
        valueReport.Append("총영토 : "+m_territorryList.Count+"수금전 자원\n");
        for (int i = 0; i < m_resources.Length; i++)
        {
           
            valueReport.Append((Capital)i+": " + m_resources[i]+"\n");
        }
        IncomeTerritory(); //영토에서 자원 수급
        valueReport.Append("수금후 자원\n");
        for (int i = 0; i < m_resources.Length; i++)
        {
         
            valueReport.Append((Capital)i + ": " + m_resources[i] + "\n");
        }
        Debug.Log(valueReport);

        ExpandTerritory();
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
            tile.Dye(nationColor[m_nationNumber]);
         //   Debug.Log("해당 타일의 타입은 " + tile.GetTileType());
        }
    }

    private void IncomeTerritory()
    {
        //영토 자원 수집
        for (int i = 0; i < m_territorryList.Count; i++)
        {
            TokenTile tile = m_territorryList[i];
            TileType territory = tile.GetTileType();
            int incomeValue = tile.GetStat(TileStat.TileEnergy);
            Capital capital = Capital.None;
            //테이블로 각 타입에서 얻을 수 있는 자원을 정의해놓기?
            switch (territory)
            {
                case TileType.Nomal:
                    break;
                case TileType.Town:
                    break;
                case TileType.Farm:
                    capital = Capital.Food;
                    break;
                case TileType.Rock:
                    break;
                case TileType.Lake:
                    break;
                case TileType.Capital:
                    break;
                default:
                    break;
            }
            m_resources[(int)capital] += incomeValue;
        }
    }

    private void ExpandTerritory()
    {
        int tempExpandCount = 3; //3개씩 확장하는걸로 
        //4칸까지 확장되가는걸로
        System.Text.StringBuilder valueReport = new System.Text.StringBuilder();
        valueReport.Append("총영토 : " + m_territorryList.Count + "\n");
        for (int i = 1; i <= 4; i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_capitalCity.GetXIndex(), m_capitalCity.GetYIndex(), i);
            //2. 무소속이면 대상 토지를 편입. 


            valueReport.Append(i+"거리 \n");
            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
                valueReport.Append(tile.GetXIndex()+", "+tile.GetYIndex()+"좌표 토지 소속은 " + tile.GetStat(TileStat.Nation)+"\n");
                if (tile.GetStat(TileStat.Nation).Equals(FixedValue.NO_NATION_NUMBER))
                {
                    AddTerritory(tile); //무소속이면 해당 타일 편입
                    tempExpandCount -= 1;
                    if (tempExpandCount.Equals(0))
                    {
                        break;
                    }
                }
            }

            if (tempExpandCount.Equals(0))
            {
                break;
            }
        }
        Debug.Log(valueReport);
    }
}
