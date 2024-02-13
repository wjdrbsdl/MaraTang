using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Capital
{
    Yellow, Green, Red, Blue
}

public enum CapitalStat
{
    Amount
}

public class PlayerCapitalData
{
    //������ �÷����ϸ鼭 �����Ǵ� ������ 
    protected int[] m_intValues;
    private Dictionary<Capital, TokenBase> m_dicCapital;
    public static PlayerCapitalData g_instance;
    public PlayerCapitalData()
    {
        g_instance = this;
        m_intValues = new int[System.Enum.GetValues(typeof(Capital)).Length];
        m_dicCapital = new();
    }

    //�ҷ��� �����ͷ� �ε�� 
    public PlayerCapitalData(TokenBase[] _loadCapital)
    {
        //�ҷ��� �����ͷ� �ε�� 
        g_instance = this;
        m_dicCapital = new();
        for (int i = 0; i < _loadCapital.Length; i++)
        {
            m_dicCapital.Add((Capital)_loadCapital[i].GetPid(), _loadCapital[i]);
        }
    }

    #region ���� �迭 �����ϴ� �κ�
    public int GetValue(System.Enum _enumIndex)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        return m_intValues[index];
    }
    public void SetValue(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_intValues[index] = _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    public void CalValue(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_intValues[index] += _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }

    public void CalValue(Capital _capital, int _value)
    {
        //Debug.Log("�߰��� ��ƾ���� ����" + _capital +"."+_value);
        if (m_dicCapital.ContainsKey(_capital))
        {
            //�ش� ��ū ������ ��ȭ
            m_dicCapital[_capital].CalStat(CapitalStat.Amount, _value);
        }
        else
        {
            //������ �߰�
            TokenBase tokenCapital = new TokenBase(_capital, _value);
            m_dicCapital.Add(_capital, tokenCapital);
        }
        //��ᰡ 0���Ϸ� �������°��� ����߸�, ���� �Ѿ�������� �Һ� ��Ű�� �ܰ迡�� ����üũ�� �ؾ���. 
        if (m_dicCapital[_capital].GetStat(CapitalStat.Amount)== 0)
        {
            //�ٵ� ���� ������ 0 ���� ���϶��
            m_dicCapital.Remove(_capital); //�ٽ� ���� 
        }
        else if(m_dicCapital[_capital].GetStat(CapitalStat.Amount) < 0)
        {
            Debug.LogError("���� üũ ����");
        }
    }

    #endregion

    public int[] GetCapitalValue()
    {
        return m_intValues;
    }

    public Dictionary<Capital, TokenBase> GetCurrentCapital()
    {
        return m_dicCapital;
    }

}
