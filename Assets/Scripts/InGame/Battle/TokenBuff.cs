using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TokenBuff : TokenBase
{

    #region ���� �����κ�
    public TokenBuff()
    {
        
    }

    //������ ĳ�������� ����
    public TokenBuff(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]); //��Ʈ �����ͻ� 0��°�� pid
        m_itemName = valueCode[1]; //1�� �̸�

        int charTypeIndex = 2;
        //if (System.Enum.TryParse(typeof(CharType), valueCode[charTypeIndex], out object charType))
        //    m_charType = (CharType)charType;

        int tierIndex = charTypeIndex + 1;
        if (int.TryParse(valueCode[tierIndex], out int tier))
            m_tier = tier;



    }
    //���纻 ĳ�� ���� : ĳ���� ���� �� ���
    public TokenBuff(TokenBuff _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = TokenType.Char;
     
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //������ ������ ���� ����� ��ü ���� �迭 �� ����. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //���ݰ� ����
        //��ų �����Ͱ� ����

    }

    #endregion



}
