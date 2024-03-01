using System.Collections;
using UnityEngine;


public class MgHud : MgGeneric<MgHud>
{
    //캐릭터 상태 표기 인터페이스
    public CharHud m_charHud;

    public void ShowCharHud(TokenChar _char)
    {
        CharHud hud = Instantiate(m_charHud);
        m_charHud.gameObject.transform.position = _char.GetObject().transform.position + Vector3.down*2f;
        hud.SetHp(_char);
    }

}
