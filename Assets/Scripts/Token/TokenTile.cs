using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#region 타일 enum
public enum TileType
{
    Nomal, Town, Farm, Rock, Lake, Capital
}

public enum TileViewState
{
    Fog, Sight
}

public enum TileAction
{
    Grass, Mineral, RemoveGrass, RemoveMineral
}

public enum TileStat
{
   Nation, Height, TileEnergy
}
#endregion

public class TokenTile : TokenBase
{
    List<TokenChar> m_inTileCharList = new();
    [SerializeField]
    public TileType tileType;
    public int ChunkNum;
    private TileViewState m_viewState = TileViewState.Fog;
    private TokenEvent m_enteranceEvent; //입장시 발동하는 이벤트가 있는가

    /*타일 상호 순서
     * 1. 타일에 1 캐릭 존재 - 타캐릭 점유시 입장불가
     * 2. 이벤트는 해당 타일 입장으로만 발동
     * 3. 타일 사용은 점유로 가능(타일위에 혹은 해당 영역 보유)하나 이벤트 존재시 사용 불가
     */

    #region 타일생성
    public TokenTile()
    {

    }
    public TokenTile MakeTileToken()
    {
        TokenTile tileToken = new TokenTile();
        tileToken.m_tokenIValues = new int[GameUtil.EnumLength(TileStat.Height)];
        tileToken.m_tokenType = TokenType.Tile;
        int ran = Random.Range(0, 2);
        if (ran.Equals(1))
            tileToken.tileType = TileType.Farm;
        
        return tileToken;
    }

    public void SetEcoValue(float _ecoValue)
    {
        int ecoValue = (int)(_ecoValue * 100f);
        SetStatValue(TileStat.Height, ecoValue);
        SetStatValue(TileStat.TileEnergy, 30);
        SetStatValue(TileStat.Nation, FixedValue.NO_NATION_NUMBER); //-1이 미소속 영토. 
    }

    public void SetEcoSprite()
    {
        int ecoValue = GetStat(TileStat.Height);

        int selectEcoMap = 0;
        if (ecoValue < 33)
            selectEcoMap = 0;
        else if (ecoValue < 66)
            selectEcoMap = 1;
        else
            selectEcoMap = 1;

        GetObject().SetSprite( MgToken.GetInstance().m_tilesSprite[selectEcoMap]);
    }

    #endregion

    public List<TokenChar> GetCharsInTile()
    {
        return m_inTileCharList;
    }

    #region 캐릭 이동 관련
    public void Migrate(TokenChar _char)
    {
        //이민 시켰으면 소속된 토큰의 지도상 좌표값도 수정
        m_inTileCharList.Add(_char);
        _char.SetMapIndex(m_xIndex, m_yIndex);
    }

    public void RemoveCharToken(TokenChar _char)
    {
        //이주 보냈으면 리스트에서 제거
        m_inTileCharList.Remove(_char);
    }

    public void ShowRouteNumber(int _number)
    {
        ((ObjectTile)m_object).ShowRouteNumber(_number);
    }

    public void OffRouteNumber()
    {
        ((ObjectTile)m_object).OffRouteNumber();
    }
    #endregion

    public void ChangeViewState(TileViewState _toState)
    {
        if (_toState.Equals(m_viewState))
            return;

        if(_toState.Equals(TileViewState.Sight))
        GameUtil.GetHideTileFromMap(GetMapIndex()).FogOff();
    }

    public void ChangeTileType(TileType _tileType)
    {
        switch (_tileType)
        {
            case TileType.Capital:
                tileType = _tileType;
                SetSprite(MgToken.GetInstance().m_tilesSprite[2]);
                break;
        }
    }

    public void Dye(Color _color)
    {
        GetObject().GetComponent<SpriteRenderer>().color = _color;
    }

    public TileType GetTileType()
    {
        return tileType;
    }

    public void SetNation(int _nationNumber)
    {
        SetStatValue(TileStat.Nation, _nationNumber);
    }

    #region 입장 이벤트
    public TokenEvent GetEneteranceEvent()
    {
        return m_enteranceEvent;
    }

    public void SetEnteraceEvent(TokenEvent _enterEvent)
    {
        m_enteranceEvent = _enterEvent;
    }

    public void DeleteEnterEvent()
    {
        m_enteranceEvent = null;
    }
    #endregion
}
