using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSpriteBox : MgGeneric<TempSpriteBox>
{
    public Sprite WoodLand;
    public Sprite Cropland;
    public Sprite TownLand;
    public Sprite MineralLand;
    public Sprite NomalLand;

    public Sprite GetSprite(MainResource mainResource)
    {
        return null;
    }
}
