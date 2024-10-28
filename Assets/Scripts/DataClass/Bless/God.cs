using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class God
{
    public int PID;
    public int Tier;
    public BlessMainCategory m_mainCategory = BlessMainCategory.무; //해당 신의 분류
    public List<GodBless> m_blessList; //해당 신이 보유한 블레스

    public God(string[] _parseStr)
    {
        PID = int.Parse(_parseStr[0]);
        string className = _parseStr[2];
        if(className == "전사")
        {
            m_mainCategory = BlessMainCategory.전사;
        }
        else if (className == "법사")
        {
            m_mainCategory = BlessMainCategory.마법;
        }
        else if (className == "궁사")
        {
            m_mainCategory = BlessMainCategory.궁사;
        }
        Tier = int.Parse(_parseStr[3]);

        Debug.Log(_parseStr[1] + "피아이디" + PID + " 카테고리" + m_mainCategory + " 티어" + Tier);
    }
    
}
