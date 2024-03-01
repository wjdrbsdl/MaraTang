using System.Collections;
using UnityEngine;


public class MgHud : MgGeneric<MgHud>
{
    //캐릭터 상태 표기 인터페이스
    public CharHud m_charHud;
    [SerializeField]
    private Transform m_hudBox;

    private void Start()
    {
        InitiSet();
    }

    public void ShowCharHud(TokenChar _char)
    {
        CharHud hud = Instantiate(m_charHud);
        hud.transform.SetParent(m_hudBox);
        hud.SetHp(_char);
    }

}
