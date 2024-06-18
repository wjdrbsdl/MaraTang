using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITradeCustomer
{
    //국가, 메인캐릭터 등 거래 주체가 되는 클래스에서 상속을 받아 지불을 위한 함수를 구현
    //TOrderItem의 TokenType에 따라 Capital이라면 메인캐릭터는 PlayerData에서 가져와서 체크하고 Nation은 m_capital 데이터로 진행하는 방식
    public bool CheckInventory(OrderCostData _costData);
    public void PayCostData(OrderCostData _costData);
}

public struct OrderCostData
{
    List<TOrderItem> costList;

    public OrderCostData(List<(TokenType, int, int)> _costList)
    {
        costList = new();
        for (int i = 0; i < _costList.Count; i++)
        {
            TOrderItem costData = new TOrderItem(_costList[i].Item1, _costList[i].Item2, _costList[i].Item3);
            costList.Add(costData);
        }
    }

    public void Add((TokenType, int, int) _costData)
    {
        if (costList == null)
            costList = new();

        TOrderItem orderItemCost = new TOrderItem(_costData.Item1, _costData.Item2, _costData.Item3);
        costList.Add(orderItemCost);
    }

    public List<TOrderItem> GetCostList()
    {
        if (costList == null)
            costList = new();

        return costList;
    }
}