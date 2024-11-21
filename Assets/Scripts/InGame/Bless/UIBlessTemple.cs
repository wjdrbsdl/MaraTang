using System.Collections;
using UnityEngine;


public class UIBlessTemple : UIBase
{
    //모시는 신에 따라 은총을 갈구할 수 있다.
    //보유한 은총을 장착해제 가능한 곳 
    private GodClassEnum m_curTempleClass = GodClassEnum.무;
    private TokenTile m_curTile;
    private GodBless newBless; //하사받은 은총

    public void SetTempleInfo(TokenTile _tile)
    {
        UISwitch(true);
        m_curTile = _tile;
        m_curTempleClass = _tile.GetGodClass();
    }

    public void PleaseBlessBtn()
    {
        newBless =  MgGodBless.GetInstance().PleaseBless(m_curTempleClass);
        if(newBless == null)
        {
            Debug.Log("내릴 가호 없음");
            return;
        }

    }

    public void AquireBlessBtn()
    {
        if(newBless == null)
        {
            //받을 은총이 없다. 
            return;
        }

        //메인케릭터에 블레스 추가 
        TOrderItem blessItem = new TOrderItem(TokenType.Bless, newBless.PID, 0); //벨류는 머해야되나
        OrderExcutor excutor = new();
        excutor.AdaptItem(blessItem);
    }
}
