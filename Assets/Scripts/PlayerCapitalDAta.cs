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
    //게임을 플레이하면서 누적되는 데이터 
    private Dictionary<Capital, TokenBase> m_dicCapital;
    public static PlayerCapitalData g_instance;

    #region 생성
    public PlayerCapitalData()
    {
        g_instance = this;
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
    #endregion

    public void CalCapital(Capital _capital, int _value)
    {
        string reward = string.Format("{0} 자원 {1} 확보", _capital, _value);
        Announcer.Instance.AnnounceState(reward);
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
        MgUI.GetInstance().ResetCapitalInfo(this);
    }

    public void CalValue(List<(Capital, int)> _tradeList, bool isGain = true)
    {
        int gain = 1; //획득
        if (isGain == false)
            gain = -1; //소비

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
