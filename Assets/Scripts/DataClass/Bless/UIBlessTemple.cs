using System.Collections;
using UnityEngine;


public class UIBlessTemple : UIBase
{
    //모시는 신에 따라 은총을 갈구할 수 있다.
    //보유한 은총을 장착해제 가능한 곳 


    public void SetTempleInfo(TokenTile _tile)
    {
        UISwitch(true);

    }

    public void PleaseBlessBtn()
    {
       GodBless bless =  MgGodBless.GetInstance().PleaseBless();
        Debug.Log(bless.m_effect.Tokentype + "에 영향 주는 은총 받음");
        PlayerBless.g_instnace.AddBless(bless);
    }
}
