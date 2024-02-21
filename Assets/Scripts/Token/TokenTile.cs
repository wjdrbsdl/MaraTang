using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#region 타일 enum
public enum TileType
{
    Nomal, Farm, Rock, Bake
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
    Height
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
    #endregion

    public List<TokenChar> GetCharsInTile()
    {
        return m_inTileCharList;
    }

    public void Migrate(TokenChar _char)
    {
        //이민 시켰으면 소속된 토큰의 지도상 좌표값도 수정
        m_inTileCharList.Add(_char);
        _char.SetMapIndex(m_xIndex, m_yIndex);
    }

    public void RemoveToken(TokenChar _char)
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

    public void ChangeViewState(TileViewState _toState)
    {
        if (_toState.Equals(m_viewState))
            return;

        if(_toState.Equals(TileViewState.Sight))
        GameUtil.GetHideTileFromMap(GetMapIndex()).FogOff();
    }

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
}
