using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenEvent : TokenBase
{
    public TokenEvent()
    {

    }

    //마스터데이터 생성
    public TokenEvent(int pid, int value2)
    {
        m_tokenPid = pid;
    }
    
    //복사본 생성

    public TokenEvent(TokenEvent _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
    }
}
