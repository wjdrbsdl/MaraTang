using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Capital
{
    None, Wood, Food, Person, Mineral,
    RedMushRoom, Paparu, Subak
}

public enum CapitalStat
{
    Amount
}

public class PlayerCapitalData : ITradeCustomer
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

    public bool CheckInventory(TItemListData _costData)
    {
        List<TOrderItem> CostList = _costData.GetItemList();
        for (int i = 0; i < CostList.Count; i++)
        {
          //  Debug.LogFormat("{0}그룹의 {1} 인덱스의 필요수량 {2}", CostList[i].Tokentype, CostList[i].SubIdx, CostList[i].Value);
            TokenType costType = CostList[i].Tokentype;
            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                    if (IsEnough((Capital)CostList[i].SubIdx, CostList[i].Value) == false)
                    {
                        Debug.Log("부족 겜마스터 인벤체크 여부 변수값에 따라 체크");
                        if(GamePlayMaster.GetInstance().m_testCheckPlayerInventory == true)
                        {
                            return false;
                        }
                        Debug.LogWarning("치트로 자원 없어도 지불");
                    }
                    break;
                default:
                    Debug.LogError("없는 case 토큰타입");
                    break;
            }
        }

        return true;
    }

    public void PayCostData(TItemListData _costData, bool _isPay = true)
    {
        List<TOrderItem> BuildCostList = _costData.GetItemList();
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            TokenType costType = BuildCostList[i].Tokentype;
            int subIdx = BuildCostList[i].SubIdx;
            int value = -BuildCostList[i].Value;
            if (_isPay == false)
                value *= -1; //지불이 아니라 받는거면 +로 전환

            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                    CalCapital((Capital)subIdx, value);
                    break;
                default:
                    Debug.Log("고려 파트 아닌 부분");
                    break;
            }
            
        }

    }

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
        //if (m_dicCapital[_capital].GetStat(CapitalStat.Amount)== 0)
        //{
        //    //근데 만약 수량이 0 보다 이하라면
        //    m_dicCapital.Remove(_capital); //다시 삭제 
        //}
        //else
        if(m_dicCapital[_capital].GetStat(CapitalStat.Amount) < 0)
        {
            Debug.LogError("수량 체크 실패");
        }
        //변화된 자원을 액션코드로 전달
        MGContent.GetInstance().SendActionCode(new TOrderItem(TokenType.Capital, (int)_capital, m_dicCapital[_capital].GetStat(CapitalStat.Amount))); //플레이어 자원변화 코드 전달
        MgUI.GetInstance().ResetCapitalInfo(this);
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

    public List<TOrderItem> GetItemList()
    {
        List<TOrderItem> list = new();
        foreach (var item in m_dicCapital)
        {
            TOrderItem capitalItem = new TOrderItem(TokenType.Capital, (int)item.Key, item.Value.GetStat(CapitalStat.Amount));
            list.Add(capitalItem);
        }
        return list;
    }
}
