using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTile : MonoBehaviour
{
    [SerializeField]
    public SpriteRenderer m_render;
    
    public void SetTileSprite()
    {
        Sprite[] sprites = MgToken.GetInstance().m_hideSprite;
        int ran = Random.Range(0, sprites.Length);
        m_render.sprite = sprites[ran];
    }

    public void MaskOn()
    {
    //    m_mask.backSortingOrder = 3;
    }

    public void FogOff()
    {
    //    m_mask.backSortingOrder = 2;

        gameObject.SetActive(false);
    }
}
