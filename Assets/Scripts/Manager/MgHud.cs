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
        //1. 캐릭터 허드 가져오기
        CharHud hud = _char.GetObject().GetHud();
        //2. 없으면 만들어서 주기
        if(hud == null)
        {
            hud = Instantiate(m_charHud);
            _char.GetObject().SetHud(hud);
        }
        
        hud.transform.SetParent(m_hudBox);
        hud.SetHp(_char);
    }

}
