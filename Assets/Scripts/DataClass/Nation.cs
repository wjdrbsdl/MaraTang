using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Nation
{
    private int m_nationNumber;
    private int m_nationLevel = 1;
    private int m_range = 1; //현재 확장된 거리
    private TokenTile m_capitalCity;
    private List<TokenTile> m_territorryList;
    private int[] m_resources;
    private List<int> m_doneTech; // 완료한 테크 Pid
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
        nation.m_doneTech = new List<int>(); //작업 완료 테크
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
        _tileToken.SetNation(m_nationNumber);
    }

    public void RemoveTerritory(TokenTile _tileToken)
    {
        m_territorryList.Remove(_tileToken);
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

    #endregion

    public void ManageNation()
    {
        //국가운영 
        IncomeTerritoryResource(); //영토에서 자원 수급
        ExpandTerritory(); //영토 확장 정책
    }

    private void IncomeTerritoryResource()
    {
        //영토 자원 수집
        for (int i = 0; i < m_territorryList.Count; i++)
        {
            TokenTile tile = m_territorryList[i];
            RuleBook.TMineTileResult mineResult = GamePlayMaster.GetInstance().RuleBook.MineResource(tile);
            CalMineResult(mineResult);
        }
    }

    private void CalMineResult(RuleBook.TMineTileResult _mineResult)
    {
        //채집 결과물을 가지고 자원 획득
        List<(Capital, int)> mineResult = _mineResult.GetResourceAmount();
        for (int i = 0; i < mineResult.Count; i++)
        {
            int resourceIdx = (int)mineResult[i].Item1; //Capital로 획득한 자원 enum값
            int amount = mineResult[i].Item2;
            m_resources[resourceIdx] += amount;
        }
    }

    private void ExpandTerritory()
    {
        int tempExpandCount = 3; //3개씩 확장하는걸로 
        //4칸까지 확장되가는걸로
        int startRange = m_range; //시작할 위치 
        for (int i = startRange; i <= m_nationLevel; i++)
        {
            //1. 수도 도시 주변으로 사거리 내 타일 하나씩 살핌
            List<TokenTile> rangeInTile = GameUtil.GetTileTokenListInRange(i, m_capitalCity.GetXIndex(), m_capitalCity.GetYIndex(), i);
            //2. 무소속이면 대상 토지를 편입. 

            m_range = i; //현재 번창된 사거리 갱신 
            for (int tileIdx = 0; tileIdx < rangeInTile.Count; tileIdx++)
            {
                TokenTile tile = rangeInTile[tileIdx];
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
        ShowTerritory();
    }

    #region 국가 테크 
    public void CompleteTech(int _techPid)
    {
        m_doneTech.Add(_techPid);
    }

    public List<int> GetDoneTech()
    {
        return m_doneTech;
    }

    public bool IsDoneTech(int _techPid)
    {
        if (m_doneTech.IndexOf(_techPid) >= 0)
            return true;

        return false;
    }
    #endregion
}
