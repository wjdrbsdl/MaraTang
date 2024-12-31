using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum BuffEnum
{
    Fracture, ArmorBreak
}

public enum NestingType
{
    Cant, Reset, Stack
}

public class TokenBuff : TokenBase
{
    public BuffEnum m_buff = BuffEnum.Fracture;
    public NestingType m_nestType = NestingType.Cant;
    public List<TOrderItem> m_effect = new();
    public int m_restTurn;
    #region 버프 생성부분
    public TokenBuff()
    {
        
    }

    //마스터 캐릭데이터 생성
    public TokenBuff(string[] _valueCode)
    {
        m_tokenType = TokenType.Buff;
        if (System.Enum.TryParse(typeof(BuffEnum), _valueCode[0], out object parsebuff))
        {
            m_tokenPid = (int)parsebuff;
            m_buff = (BuffEnum)parsebuff;
        }

        m_itemName = _valueCode[1]; //1은 이름
        int nestingIdx = 2;
        int restTurnIdx = nestingIdx + 1;
        m_restTurn = int.Parse(_valueCode[restTurnIdx]);
    }
    //복사본 캐릭 생성 : 캐릭터 스폰 시 사용
    public TokenBuff(TokenBuff _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_buff = _masterToken.m_buff;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = _masterToken.m_tokenType;
        m_restTurn = _masterToken.m_restTurn;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //마스터 데이터 깊은 복사로 객체 고유 배열 값 생성. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //스텟값 복사
        //스킬 마스터값 복사

    }

    #endregion

    public void Count(int _count)
    {
        m_restTurn -= 1;
        if (m_restTurn <= 0)
            m_restTurn = 0;
    }

    public bool DoneTime()
    {
        return m_restTurn <= 0;
    }

}
