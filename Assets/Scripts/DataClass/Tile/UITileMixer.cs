using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UITileMixer : UIBase
{
    private TileType m_madeTile = TileType.Nomal; //만들려는 타일
    private List<TokenTile> m_inTile = new(); //추가된 타일들
    private int m_nationNum = -1;
    private List<TileType> needTile = new(); //현재 부족한 부분
    private List<TileType> recipe = new(); //최초 필요한 부분

    #region 초기화
    public void SetMixInfo(TileType _goalTile, int _nationNum)
    {
        //만들 조합장소와 포함되는 국가 넘버를 세팅
        UISwitch(true);
        m_madeTile = _goalTile;
        m_nationNum = _nationNum;
        //해당 타입을 만들기 위한 재료 정보를 가져와야함 
        SetRecipe(_goalTile);
    }

    public void SetMixInfo(TileType _goalTile, TokenTile _tile)
    {
        //만들 조합장소와 포함되는 국가 넘버를 세팅
        UISwitch(true);
        m_madeTile = _goalTile;
        m_nationNum = _tile.GetNation().GetNationNum();
        //해당 타입을 만들기 위한 재료 정보를 가져와야함 
        SetRecipe(_goalTile);
        AddTile(_tile);
    }
 

    private void SetRecipe(TileType _goalType)
    {
        int[] needTiles = MgMasterData.GetInstance().GetTileData((int)_goalType).NeedTiles;
        needTile = new();
        recipe = new();
        for (int i = 0; i < needTiles.Length; i++)
        {
            needTile.Add((TileType)needTiles[i]);
            recipe.Add((TileType)needTiles[i]);
        }

    }
    #endregion

    public void OnClickTile(TokenTile _tile)
    {
        //타일 누른걸 캐싱해와야하는데 

        //기존 있던 타일이라면 제외
        if(m_inTile.IndexOf(_tile)!= -1)
        {
            RemoveTile(_tile);
            return;
        }
        int tileNation = _tile.GetNation().GetNationNum();
        if (tileNation != m_nationNum)
        {
            //국가영토 아닌 영토인경우 작용안함
            return;
        }
        //재료에 필요한건지 체크
        TileType addType = _tile.GetTileType();
        if (InNeed(addType) == false)
        {
            //필요없는 재료면 리턴
            return;
        }
        AddTile(_tile);
    }

    public void AddTile(TokenTile _tile)
    {
        m_inTile.Add(_tile); //리스트에 넣고
        needTile.Remove(_tile.GetTileType()); //필요한 타입에서 제외하고
        Debug.Log(_tile.GetTileType() + "추가 남은 재료 수" + needTile.Count);
    }

    public void RemoveTile(TokenTile _tile)
    {
        m_inTile.Remove(_tile); //리스트에서 빼고
        needTile.Add(_tile.GetTileType()); //필요한 타입에 추가하고
    }

    private bool InNeed(TileType _type)
    {
        for (int i = 0; i < needTile.Count; i++)
        {
            if (needTile[i] == _type)
                return true;
        }
        return false;
    }
}
