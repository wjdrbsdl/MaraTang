using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum DevilState
{
    봉인, 유아, 성인, 토벌
}

public class DevilIncubator
{
    private List<TokenChar> m_devilList;
    private List<DevilState> m_devilStateList;

    public void DiceDevilList()
    {
        //등장할 악마를 골라서 생성 
        Dictionary<int,TokenChar> charDic = MgMasterData.GetInstance().GetCharDic();
        List<TokenChar> devilList = new();
        foreach(KeyValuePair<int, TokenChar> tokenChar in charDic)
        {
            if (tokenChar.Value.GetCharType().Equals(CharType.Devil))
            {
                Debug.Log(tokenChar.Value.GetItemName() + " 악마 리스트에 추가");
                devilList.Add(tokenChar.Value);
            }
        }
    }

}

