using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EquiptItemData
{
    public int m_pid = 0;
    public TokenType m_tokenType = TokenType.Equipt;
    public string m_itemName = "";
    public EquiptPartEnum m_part = EquiptPartEnum.None;
    public List<TOrderItem> m_effect = new(); //플레이어 스텟에 가하는 요소
    public List<int> m_optionPoolList = new();
    public int ableDropTir = 0;
    public int dropRatio = 0;

    public EquiptItemData()
    {
        
    }

    public EquiptItemData(string[] _dbValueList)
    {
        m_pid = int.Parse(_dbValueList[0]);
        m_tokenType = TokenType.Equipt;
        m_itemName = _dbValueList[1];

        int partIdx = 2;
        if (System.Enum.TryParse(typeof(EquiptPartEnum), _dbValueList[partIdx], out object parsePart))
            m_part = (EquiptPartEnum)parsePart;

        int optionIdx = partIdx + 1;
        string[] options = _dbValueList[optionIdx].Split(FixedValue.PARSING_LIST_DIVIDE);
        for (int i = 0; i < options.Length; i++)
        {
            string pid = options[i];
            if(int.TryParse(pid, out int varidPid))
            {
                m_optionPoolList.Add(varidPid);
            }
        }

        int dropTierIdx = optionIdx + 1;
        ableDropTir = int.Parse(_dbValueList[dropTierIdx]); //드랍하려는 티어 Point가 able값 이상이어야 해당 장비 드랍 가능
        int dropRatioIdx = dropTierIdx + 1;
        dropRatio = int.Parse(_dbValueList[dropRatioIdx]); //다른 Pool의 장비들과 비교해서 드랍될 수치
    }

    public EquiptItem GetItem(int _tier)
    {
        List<TOrderItem> effect = new();
        //1. 유효한 옵션을 고른다
        
        for (int i = 0; i < m_optionPoolList.Count; i++)
        {
            EquiptOptionData optionData = MgMasterData.GetInstance().GetEquiptOptionData(m_optionPoolList[i]);
            if(optionData == null)
            {
                //Debug.Log("잘못된 옵션 pid 할당");
                continue;
            }

            TOrderItem ranOption = optionData.GetOptionValue(_tier);
            if(ranOption.Tokentype.Equals(TokenType.None) == false)
            {
                effect.Add(ranOption);
            }
        }
        //2. 옵션의 수치를 tier에 따라 정의한다
        //effect.Add(new TOrderItem(TokenType.CharStat,(int)CharStat.Strenth, 100)); //확인용 위해 힘스텟 100
        //3. 해당 옵션으로 장비를 새로 만든다. 
        EquiptItem item = new EquiptItem(m_pid, m_itemName, m_part, _tier, effect);

        return item;
    }
}
