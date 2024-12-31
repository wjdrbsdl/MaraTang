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
        bool prayReady = m_curTile.IsReadyInherece;
        if(prayReady == false)
        {
            Debug.Log("신전에 기도드릴 준비가 안되었음");
            m_curTile.ReadyInherenceWork(); //신전 기도드릴 준비 템플에서 하기 
            return;
        }


        newBless =  MgGodBless.GetInstance().PleaseBless(m_curTempleClass);
        if(newBless == null)
        {
            Debug.Log("내릴 가호 없음");
            return;
        }

        m_curTile.DoneInhereceFunction();//신전에서 가호 무언가를 내림받았을때 작업 수행한걸로
        Debug.Log(newBless.m_godPid + "받았다.");

    }

    public void AquireBlessBtn()
    {
        if(newBless == null)
        {
            //받을 은총이 없다. 
            return;
        }

        //메인케릭터에 블레스 추가 
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        //새 축복 클래스로 복사해서 케릭터에게 획득 진행 
        GodBless bless = new GodBless(newBless);
        mainChar.AquireBless(bless);
    }

    public void OpenPlayerBless()
    {
        MgUI.GetInstance().ShowPlayerBless();
    }
}
