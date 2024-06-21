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

    public Sprite[] Chares;
    public Sprite GetSprite(TileType _type)
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
        }
        return NomalLand;
    }

    public Sprite GetCharSprite(int _pid)
    {
        if (_pid < 0 || Chares.Length <= _pid)
            return Chares[0];
        return Chares[_pid];
    }
}
