using System.Collections;
using UnityEngine;


public class UIBlessTemple : UIBase
{
    //모시는 신에 따라 은총을 갈구할 수 있다.
    //보유한 은총을 장착해제 가능한 곳 
    private GodClassEnum m_curTempleClass = GodClassEnum.무;

    public void SetTempleInfo(TokenTile _tile)
    {
        UISwitch(true);

    }

    public void PleaseBlessBtn()
    {
       GodBless bless =  MgGodBless.GetInstance().PleaseBless(GodClassEnum.무);
        if(bless == null)
        {
            Debug.Log("내릴 가호 없음");
            return;
        }

        //메인케릭터에 블레스 추가 
        PlayerManager.GetInstance().GetMainChar().AddBless(bless);
    }
}
