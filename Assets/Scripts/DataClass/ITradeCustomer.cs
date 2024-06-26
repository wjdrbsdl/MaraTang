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
    public void PayCostData(OrderCostData _costData, bool _isPay = true);
}

public struct OrderCostData
{
    List<TOrderItem> costList;

    public OrderCostData(List<(Capital, int)> _costList)
    {
        costList = new();
        for (int i = 0; i < _costList.Count; i++)
        {
            TOrderItem costData = new TOrderItem((int)TokenType.Capital, (int)_costList[i].Item1, _costList[i].Item2);
            costList.Add(costData);
        }
    }

    public OrderCostData((Capital, int) _cost)
    {
        costList = new();
        TOrderItem costData = new TOrderItem((int)TokenType.Capital, (int)_cost.Item1, _cost.Item2);
        costList.Add(costData);
    }

    public OrderCostData(List<TOrderItem> _itemList)
    {
        costList = _itemList;
    }

    public void Add(TOrderItem _orderItem)
    {
        if (costList == null)
            costList = new();
        
        costList.Add(_orderItem);
    }

    public List<TOrderItem> GetCostList()
    {
        if (costList == null)
            costList = new();

        return costList;
    }
}