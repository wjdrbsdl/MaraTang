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
    //게임을 플레이하면서 누적되는 데이터 
    protected int[] m_intValues;
    private Dictionary<Capital, TokenBase> m_dicCapital;
    public static PlayerCapitalData g_instance;
    public PlayerCapitalData()
    {
        g_instance = this;
        m_intValues = new int[System.Enum.GetValues(typeof(Capital)).Length];
        m_dicCapital = new();
    }

    //불러온 데이터로 로드시 
    public PlayerCapitalData(TokenBase[] _loadCapital)
    {
        //불러온 데이터로 로드시 
        g_instance = this;
        m_dicCapital = new();
        for (int i = 0; i < _loadCapital.Length; i++)
        {
            m_dicCapital.Add((Capital)_loadCapital[i].GetPid(), _loadCapital[i]);
        }
    }

    #region 스텟 배열 적용하는 부분
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
        //Debug.Log("추가된 루틴으로 들어옴" + _capital +"."+_value);
        if (m_dicCapital.ContainsKey(_capital))
        {
            //해당 토큰 수량에 변화
            m_dicCapital[_capital].CalStat(CapitalStat.Amount, _value);
        }
        else
        {
            //없으면 추가
            TokenBase tokenCapital = new TokenBase(_capital, _value);
            m_dicCapital.Add(_capital, tokenCapital);
        }
        //재료가 0이하로 내려가는경우는 없어야만, 여기 넘어오기전에 소비를 시키는 단계에서 수량체크를 해야함. 
        if (m_dicCapital[_capital].GetStat(CapitalStat.Amount)== 0)
        {
            //근데 만약 수량이 0 보다 이하라면
            m_dicCapital.Remove(_capital); //다시 삭제 
        }
        else if(m_dicCapital[_capital].GetStat(CapitalStat.Amount) < 0)
        {
            Debug.LogError("수량 체크 실패");
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
