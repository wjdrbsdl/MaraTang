using System.Collections;
using UnityEngine;
using DamageNumbersPro;

public class MgCharDisplay : MgGeneric<MgCharDisplay>
{
    public DamageNumberMesh m_texture;

    private void Awake()
    {
        ManageInitiSet();
    }

    public void PlayScript(TokenBase _talker, string _text)
    {
        Vector3 pos = _talker.GetObject().gameObject.transform.position;
        DamageNumber num = m_texture.Spawn(pos + Vector3.up * 3, _text);
    }
}