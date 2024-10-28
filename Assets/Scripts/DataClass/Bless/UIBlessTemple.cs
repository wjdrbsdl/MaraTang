using System.Collections;
using UnityEngine;


public class UIBlessTemple : UIBase
{
    //모시는 신에 따라 은총을 갈구할 수 있다.
    //보유한 은총을 장착해제 가능한 곳 
    private BlessMainCategory m_curTempleClass = BlessMainCategory.무;

    public void SetTempleInfo(TokenTile _tile)
    {
        UISwitch(true);

    }

    public void PleaseBlessBtn()
    {
       GodBless bless =  MgGodBless.GetInstance().PleaseBless(BlessMainCategory.무);
        if(bless == null)
        {
            Debug.Log("내릴 가호 없음");
            return;
        }
     
        PlayerBless.g_instnace.AddBless(bless);
    }
}
