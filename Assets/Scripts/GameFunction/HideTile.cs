using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTile : MonoBehaviour
{
    [SerializeField]
    private SpriteMask m_mask;
    public Sprite[] m_sprites;
    public SpriteRenderer m_render;
    private void Start()
    {
        int ran = Random.Range(0, m_sprites.Length);
        m_render.sprite = m_sprites[ran];
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
