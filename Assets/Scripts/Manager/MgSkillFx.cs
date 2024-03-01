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
        //����Ʈ�� ������ �����ε�

        //1.�ڵ�� �´� skillSfx�� ã�Ƽ� 
        SkillSfx sfx = Instantiate(m_SkillSfx);
        //2.����
        sfx.transform.position = GameUtil.GetTileTokenFromMap(_pos).GetObject().transform.position;
        //3.Ÿ�̸� ����
        sfx.SetTimer(3f);
    }
}
