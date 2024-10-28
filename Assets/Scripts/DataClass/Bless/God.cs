using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class God
{
    public int PID;
    public int Tier;
    public string Test;
    public BlessMainCategory m_mainCategory = BlessMainCategory.무; //해당 신의 분류
    public List<int> m_blessList; //해당 신이 보유한 블레스

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

        string[] haveBless = _parseStr[4].Split(FixedValue.PARSING_LIST_DIVIDE);
        m_blessList = new();
        for(int i = 0; i < haveBless.Length; i++)
        {
            int blessPid = int.Parse(haveBless[i]);
            m_blessList.Add(blessPid);
          //  Debug.Log(_parseStr[1] + "이 내릴수 있는 가호 pid " + blessPid);
        }

        // Debug.Log(_parseStr[1] + "피아이디" + PID + " 카테고리" + m_mainCategory + " 티어" + Tier);
    }

}
