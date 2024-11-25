using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpriteBox : MgGeneric<TempSpriteBox>
{
    public Sprite WoodLand;
    public Sprite FarmLand;
    public Sprite TownLand;
    public Sprite MineralLand;
    public Sprite NomalLand;
    public Sprite MountainLand;
    public Sprite CapitalLand;
    public Sprite ChildLand;
    public Sprite CaveLand;

    public Sprite ExpandLandPin;
    public Sprite ManageLandPin;
    public Sprite DefaultPin;

    public Sprite[] m_baseTile;
    public Sprite[] m_elements;
    public Sprite[] Chares;
    public Sprite[] HideTiles;

    public Sprite ActionSlotFrame;
    public Sprite ActionSlotBackGround;
        
    public Sprite GetTileSprite(TileType _type)
    {
        switch (_type)
        {
            case TileType.Capital:
                return CapitalLand;
            case TileType.Nomal:
                return NomalLand;
            case TileType.Town:
                return TownLand;
            case TileType.Farm:
                return FarmLand;
            case TileType.Mine:
                return MineralLand;
            case TileType.Mountain:
                return MountainLand;
            case TileType.WoodLand:
                return WoodLand;
            case TileType.Child:
                return ChildLand;
            case TileType.Cage:
                return ChildLand;
        }
        return NomalLand;
    }

    public Sprite GetBaseTile(MainResource _main)
    {
        return m_baseTile[(int)_main];
    }

    public Sprite GetTileElement(int _index)
    {
        if (m_elements.Length < _index)
            return m_elements[0];

        return m_elements[_index];
    }
    public Sprite[] GetHideSprites()
    {
        return HideTiles;
    }

    public Sprite GetCharSprite(int _pid)
    {
        if (_pid < 0 || Chares.Length <= _pid)
            return Chares[1];
        return Chares[_pid];
    }

    public Sprite GetPolicySprite(MainPolicyEnum _policy)
    {
        switch (_policy)
        {
            case MainPolicyEnum.ExpandLand:
                return ExpandLandPin;
            case MainPolicyEnum.ManageLand:
                return ManageLandPin;
            default:
                return DefaultPin;

        }
    }
}
