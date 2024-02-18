using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PopupDamage : MgGeneric<PopupDamage>
{
    // Start is called before the first frame update
    public DamageNumber damagePopup;
    public float scale = 1f;

    private void Start()
    {
        InitiSet();
    }

    public void DamagePop(GameObject _object, int _deal)
    {
        DamageNumber num = damagePopup.Spawn(_object.transform.position, _deal);
        num.transform.SetParent(gameObject.transform);
        num.transform.localScale = new Vector3(scale, scale, scale);
    }
}
