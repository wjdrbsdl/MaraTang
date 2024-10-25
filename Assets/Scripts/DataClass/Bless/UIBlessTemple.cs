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
        MgGodBless.GetInstance().PleaseBless();
    }
}
