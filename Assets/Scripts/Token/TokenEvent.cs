using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenEvent : TokenBase
{
    public TokenEvent()
    {

    }

    //�����͵����� ����
    public TokenEvent(int pid, int value2)
    {
        m_tokenPid = pid;
    }
    
    //���纻 ����

    public TokenEvent(TokenEvent _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
    }
}
