using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MgSkillFx : MgGeneric<MgSkillFx>
{
    public SkillSfx m_SkillSfx;

    void Start()
    {
        InitiSet();
    }


    public void MakeSkillFx(int[] _pos, string _code)
    {
        //이펙트를 받으면 생성인데

        //1.코드로 맞는 skillSfx를 찾아서 
        SkillSfx sfx = Instantiate(m_SkillSfx);
        //2.생성
        sfx.transform.position = GameUtil.GetTileTokenFromMap(_pos).GetObject().transform.position;
        //3.타이머 설정
        sfx.SetTimer(3f);
    }
}
