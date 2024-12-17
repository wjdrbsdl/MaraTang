using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgSkillFx : MgGeneric<MgSkillFx>
{
    public SkillSfx m_SkillSfx;

    void Start()
    {
        ManageInitiSet();
    }


    public void MakeSkillFx(int[] _pos, int _actionPid)
    {
        //이펙트를 받으면 생성인데

        //1.코드로 맞는 skillSfx를 찾아서 
        SkillSfx sfx = Instantiate(m_SkillSfx);
        //2.생성
        sfx.transform.position = GameUtil.GetTileTokenFromMap(_pos).GetObject().transform.position;
        //3.타이머 설정
        sfx.SetTimer(3f);
    }
    public void MakeSkillFx(TokenTile _tile, int _actionPid)
    {
        //이펙트를 받으면 생성인데

        //1.액션에 맞는 효과 불러오기 
        SkillSfx sfx = Instantiate(m_SkillSfx); //임시로 정의된 sfx를 할당

        //2.이펙트 생성
        sfx.transform.position = _tile.GetObject().transform.position;

        //3.타이머 설정
        sfx.SetTimer(1.3f);
    }
}
