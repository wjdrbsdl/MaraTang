using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TileMixSlot : SlotBase
{
    /*
     * 타일 이미지를 표기할 녀석
     * 투입된건 컬러, 안된건 흑백
     * 각 재료에 맞는 타일의 Pos 혹은 Tile이 저장되어있음
     * 누르면 해당 Tile에 카메라를 초점맞출수 있음. 
     * 버리면 해당 재료에서 제외됨
     * 목표타일은 재료 타일들중에 갖다 박으면 됨 
     */
    public TokenTile m_tile; //할당된 아이템의 tokenType
    public TileType m_needType;
    public int m_slotIndex = 0;
    [SerializeField] private TMP_Text m_putState;

    private void Start()
    {
        m_isDragSlot = true;
    }

    public void SetRecipe(TileType _needTile)
    {
        m_needType = _needTile;
        m_tile = null;
    }

    public bool PutTile(TokenTile _tile)
    {
        //이미 할당되어있으면 거짓 반환
        if (m_tile != null)
            return false;

        //투입하려는 대상이 다른 타입이면 거짓 반환
        if (_tile.GetTileType().Equals(m_needType) == false)
            return false;

        //아니면 할당하고 참 반환
        m_tile = _tile;
        m_putState.text = m_needType.ToString();
        return true;
    }

    public void ResetSlot()
    {
        m_tile = null;
        m_putState.text = "필요 "+m_needType.ToString();
    }
}
