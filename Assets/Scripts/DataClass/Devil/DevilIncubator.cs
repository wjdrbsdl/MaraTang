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
    private List<TokenChar> m_devilList = new List<TokenChar>();
    private List<bool> m_contentMeet = new();
    private List<DevilState> m_devilStateList;
    private int m_turnTerm = 15; //발생 주기
    private bool m_turnEnough = false;

    public List<TokenChar> DiceDevilList(int _diceCount)
    {
        //등장할 악마를 골라서 생성 

        //1. 캐릭 마스터 데이터에서 악마 데이터만 추출하여 복사
        Dictionary<int,TokenChar> charDic = MgMasterData.GetInstance().GetCharDic();
        List<TokenChar> copyDevilList = new();
        foreach(KeyValuePair<int, TokenChar> tokenChar in charDic)
        {
            if (tokenChar.Value.GetCharType().Equals(CharType.Devil))
            {
                
                TokenChar devilCopy = new TokenChar(tokenChar.Value);
                copyDevilList.Add(devilCopy);
               // Debug.Log(devilCopy.GetItemName() + " 악마 복사본 리스트에 추가" + devilCopy.GetCharType());
            }
        }

        //2. 등장시킬 악마 랜덤으로 숫자만큼 뽑기
        List<int> randomDevilIdx = GameUtil.GetRandomNum(copyDevilList.Count, _diceCount);

        //3. 랜덤 주사위 인덱스에 있는 악마를 devilList에 추가하여 등장시킬 악마 확정
        for (int i = 0; i < randomDevilIdx.Count; i++)
        {
            m_devilList.Add(copyDevilList[randomDevilIdx[i]]);
            m_contentMeet.Add(false);
            Debug.Log(m_devilList[i].GetItemName() + "악마 확정");
        }
        
        return m_devilList;
    }

    public void ChangeWorldContent(TOrderItem _worldContent)
    {
        //토큰타입 - 컨텐츠, subIdx = 컨텐츠 pid, value = 세부 값
        //해당 조건에 따라서 보유중인 악마들의 봉인 조건을 따져서 충족 진행 
    }

    public void ChangeWorldTurn(int _currentTurn)
    {
        if(_currentTurn%m_turnTerm == 0)
        {
            m_turnEnough = true;
            Debug.Log("때 조건 충족");
        }
    }

}

