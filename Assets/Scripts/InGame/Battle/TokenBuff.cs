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

    #region ���� �����κ�
    public TokenBuff()
    {
        
    }

    //������ ĳ�������� ����
    public TokenBuff(string[] valueCode)
    {
        if (System.Enum.TryParse(typeof(BuffEnum), valueCode[0], out object parsebuff))
        {
            m_tokenPid = (int)parsebuff;
            m_buff = (BuffEnum)parsebuff;
        }

        m_itemName = valueCode[1]; //1�� �̸�
        m_tokenType = TokenType.Buff;
    }
    //���纻 ĳ�� ���� : ĳ���� ���� �� ���
    public TokenBuff(TokenBuff _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_buff = _masterToken.m_buff;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = _masterToken.m_tokenType;

        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //������ ������ ���� ����� ��ü ���� �迭 �� ����. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //���ݰ� ����
        //��ų �����Ͱ� ����

    }

    #endregion



}
