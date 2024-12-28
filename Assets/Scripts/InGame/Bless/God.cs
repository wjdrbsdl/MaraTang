using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class God
{
    public int PID;
    public int Tier;
    public string Test;
    public GodClassEnum m_mainCategory = GodClassEnum.무; //해당 신의 분류
    public List<GodBless> m_blessList; //해당 신이 보유한 블레스

    public God(string[] _parseStr)
    {
        PID = int.Parse(_parseStr[0]);
        int partIdx = 2;
        if (System.Enum.TryParse(typeof(GodClassEnum), _parseStr[partIdx], out object parsePart))
            m_mainCategory = (GodClassEnum)parsePart;

        Tier = int.Parse(_parseStr[3]);

        string[] haveBless = _parseStr[4].Split(FixedValue.PARSING_LIST_DIVIDE);
        m_blessList = new();
        for(int i = 0; i < haveBless.Length; i++)
        {
            int blessPid = int.Parse(haveBless[i]);
            GodBless bless = MgMasterData.GetInstance().GetGodBless(blessPid);
            if(bless != null)
            {
                bless.m_classCategory = m_mainCategory; //은총의 주요 카테고리는 해당 신이 속한 카테고리
                bless.m_godPid = PID;
                m_blessList.Add(bless);
            }
               
          //  Debug.Log(_parseStr[1] + "이 내릴수 있는 가호 pid " + blessPid);
        }

        // Debug.Log(_parseStr[1] + "피아이디" + PID + " 카테고리" + m_mainCategory + " 티어" + Tier);
    }

}
