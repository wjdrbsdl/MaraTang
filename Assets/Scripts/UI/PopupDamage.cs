using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DamageNumbersPro;

public class PopupDamage : MgGeneric<PopupDamage>
{
    // Start is called before the first frame update
    public DamageNumber damagePopup;
    public DamageNumberMesh damageMesh;
    public float scale = 1f;
    public float upPos = 2f;
    private void Start()
    {
        ManageInitiSet();
    }

    public void DamagePop(GameObject _object, int _deal, int a)
    {
        DamageNumber num = damagePopup.Spawn(_object.transform.position, _deal);
        num.transform.SetParent(gameObject.transform);
        num.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DamagePop(GameObject _object, int _deal)
    {
        DamageNumber num = damageMesh.Spawn(_object.transform.position+ Vector3.up*upPos, _deal);
       
        num.transform.localScale = new Vector3(scale, scale, scale);
    }
}
