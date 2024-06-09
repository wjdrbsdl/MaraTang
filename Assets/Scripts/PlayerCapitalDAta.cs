using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Capital
{
    Food, Mineral, Wood, Blue
}

public enum CapitalStat
{
    Amount
}

public class PlayerCapitalData
{
    //������ �÷����ϸ鼭 �����Ǵ� ������ 
    private Dictionary<Capital, TokenBase> m_dicCapital;
    public static PlayerCapitalData g_instance;

    #region ����
    public PlayerCapitalData()
    {
        g_instance = this;
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
    #endregion

    public void CalCapital(Capital _capital, int _value)
    {
        string reward = string.Format("{0} �ڿ� {1} Ȯ��", _capital, _value);
        Announcer.Instance.AnnounceState(reward);
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
        MgUI.GetInstance().ResetCapitalInfo(this);
    }

    public void CalValue(List<(Capital, int)> _tradeList, bool isGain = true)
    {
        int gain = 1; //ȹ��
        if (isGain == false)
            gain = -1; //�Һ�

        for (int i = 0; i < _tradeList.Count; i++)
        {
            CalCapital(_tradeList[i].Item1, gain*_tradeList[i].Item2);
        }
    }

    public Dictionary<Capital, TokenBase> GetHaveCapitalDic()
    {
        return m_dicCapital;
    }

    public bool IsEnough(Capital _capital, int _need)
    {
        if (m_dicCapital.ContainsKey(_capital) == false)
            return false;

        if (m_dicCapital[_capital].GetStat(CapitalStat.Amount) < _need)
            return false;

        return true;
    }

    public bool IsEnough(List<(Capital, int)> _needList)
    {
        for (int i = 0; i < _needList.Count; i++)
        {
            if (IsEnough(_needList[i].Item1, _needList[i].Item2) == false)
                return false;
        }

        return true;
    }
}
