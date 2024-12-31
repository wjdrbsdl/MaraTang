using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TokenBuff : TokenBase
{

    #region 버프 생성부분
    public TokenBuff()
    {
        
    }

    //마스터 캐릭데이터 생성
    public TokenBuff(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]); //시트 데이터상 0번째는 pid
        m_itemName = valueCode[1]; //1은 이름

        int charTypeIndex = 2;
        //if (System.Enum.TryParse(typeof(CharType), valueCode[charTypeIndex], out object charType))
        //    m_charType = (CharType)charType;

        int tierIndex = charTypeIndex + 1;
        if (int.TryParse(valueCode[tierIndex], out int tier))
            m_tier = tier;



    }
    //복사본 캐릭 생성 : 캐릭터 스폰 시 사용
    public TokenBuff(TokenBuff _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = TokenType.Char;
     
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //마스터 데이터 깊은 복사로 객체 고유 배열 값 생성. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //스텟값 복사
        //스킬 마스터값 복사

    }

    #endregion



}
