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
        //����Ʈ�� ������ �����ε�

        //1.�ڵ�� �´� skillSfx�� ã�Ƽ� 
        SkillSfx sfx = Instantiate(m_SkillSfx);
        //2.����
        sfx.transform.position = GameUtil.GetTileTokenFromMap(_pos).GetObject().transform.position;
        //3.Ÿ�̸� ����
        sfx.SetTimer(3f);
    }
    public void MakeSkillFx(TokenTile _tile, int _actionPid)
    {
        //����Ʈ�� ������ �����ε�

        //1.�׼ǿ� �´� ȿ�� �ҷ����� 
        SkillSfx sfx = Instantiate(m_SkillSfx); //�ӽ÷� ���ǵ� sfx�� �Ҵ�

        //2.����Ʈ ����
        sfx.transform.position = _tile.GetObject().transform.position;

        //3.Ÿ�̸� ����
        sfx.SetTimer(1.3f);
    }
}
