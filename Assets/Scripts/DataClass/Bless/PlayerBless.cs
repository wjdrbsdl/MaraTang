using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class PlayerBless 
{
    public List<GodBless> m_haveList;
    public static PlayerBless g_instnace;

    public PlayerBless()
    {
        g_instnace = this;
        m_haveList = new();
    }

    public void AddBless(GodBless _bless)
    {
        Debug.Log("보유 은총 추가");
        m_haveList.Add(_bless);
    }

    public void EquiptBless(GodBless _bless)
    {
        //블레스 장착한경우, 바로 적용가능한 옵션이면 옵션적용
        for (int i = 0; i < _bless.m_effect.Count; i++)
        {
            TokenType blessType = _bless.m_effect[i].Tokentype;
            switch (blessType)
            {
                case TokenType.CharStat:
                    Debug.Log("스텟 관련 가호면 바로 적용");
                    break;
                default:
                    break;
            }
        }
        
    }
}
