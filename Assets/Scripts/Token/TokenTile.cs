using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TileType
{

}

public enum TileViewState
{
    Fog, Sight
}

public class TokenTile : TokenBase
{
    List<TokenChar> m_inTileCharList = new();
    [SerializeField]
    private TileType m_tileType;
    private TileViewState m_viewState = TileViewState.Fog;

    public TokenTile()
    {

    }
    public TokenTile MakeTileToken()
    {
        TokenTile tileToken = new TokenTile();
        tileToken.m_tokenIValues = new int[(int)TileStat.StatSize];
        tileToken.m_tokenType = TokenType.Tile;
        
        return tileToken;
    }
  
    public List<TokenChar> GetCharsInTile()
    {
        return m_inTileCharList;
    }

    public void Migrate(TokenChar _char)
    {
        //�̹� �������� �Ҽӵ� ��ū�� ������ ��ǥ���� ����
        m_inTileCharList.Add(_char);
        _char.SetMapIndex(m_xIndex, m_yIndex);
    }

    public void Immigrate(TokenChar _char)
    {
        //���� �������� ����Ʈ���� ����
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
        if(_toState.Equals(TileViewState.Sight))
        GameUtil.GetHideTileFromMap(GetMapIndex()).FogOff();
    }
}
