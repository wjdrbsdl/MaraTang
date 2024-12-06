using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MapBlock : MonoBehaviour
{
    public Image m_blockImage;
    public RectTransform rectTransform;

    public void SetSize(float _width, float _height)
    {
        
    }

    public void SetMapPos(Transform _box, TokenTile _tile)
    {
        int[] pos = _tile.GetMapIndex();
        rectTransform.position = new Vector3(5, 5);
    }

}
