using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PopupDamage : MgGeneric<PopupDamage>
{
    // Start is called before the first frame update
    public DamageNumber damagePopup;

    private void Start()
    {
        InitiSet();
    }

    public void DamagePop(GameObject _object, int _deal)
    {
        damagePopup.Spawn(_object.transform.position, _deal);
    }
}
