using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapBlock : MonoBehaviour
{
    public Image m_blockImage;
    public RectTransform rectTransform;
    public int xPos;
    public int yPos;

    public void SetInfo(TokenTile _tile)
    {
        xPos = _tile.GetMapIndex()[0];
        yPos = _tile.GetMapIndex()[1];
        SetColor(_tile.GetMainResource());
        SetTileType(_tile.tileType);
    }

    private void SetColor(MainResource _resource)
    {
        Color color = Color.white;
        switch (_resource)
        {
            case MainResource.Tree:
                color = Color.green;
                break;
            case MainResource.Food:
                color = Color.yellow;
                break;
            case MainResource.Mineral:
                color = Color.blue;
                break;
        }
        m_blockImage.color = color;
    }

    private void SetTileType(TileType _type)
    {
        if (_type == TileType.Mountain)
            m_blockImage.color = Color.black;
    }

}
