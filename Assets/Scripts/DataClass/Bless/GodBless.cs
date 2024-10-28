using System.Collections;
using UnityEngine;


public enum BlessMainCategory
{
    무, 마법, 전사, 궁사
}

public class GodBless
{
    public int PID;
    public BlessMainCategory m_mainCategory = BlessMainCategory.무; //가호 시너지 체크를 위해 분류 
    public TOrderItem m_effect; //플레이어 스텟에 가하는 요소

    public GodBless(string[] _dbValueList)
    {
        PID = int.Parse(_dbValueList[0]);
        m_effect = new TOrderItem(TokenType.CharStat, (int)CharStat.MaxHp, 30);
    }
}
