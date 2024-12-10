using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITradeCustomer
{
    //국가, 메인캐릭터 등 거래 주체가 되는 클래스에서 상속을 받아 지불을 위한 함수를 구현
    //TOrderItem의 TokenType에 따라 Capital이라면 메인캐릭터는 PlayerData에서 가져와서 체크하고 Nation은 m_capital 데이터로 진행하는 방식
    public bool CheckInventory(TItemListData _costData);
    public void PayCostData(TItemListData _costData, bool _isPay = true);
}

public struct TItemListData
{
    List<TOrderItem> costList;

    public TItemListData(TOrderItem _item)
    {
        costList = new();
        costList.Add(_item);
    }

    public TItemListData(List<(Capital, int)> _costList)
    {
        costList = new();
        for (int i = 0; i < _costList.Count; i++)
        {
            TOrderItem costData = new TOrderItem(TokenType.Capital, (int)_costList[i].Item1, _costList[i].Item2);
            costList.Add(costData);
        }
    }

    public TItemListData((Capital, int) _cost)
    {
        costList = new();
        TOrderItem costData = new TOrderItem(TokenType.Capital, (int)_cost.Item1, _cost.Item2);
        costList.Add(costData);
    }

    public TItemListData(List<TOrderItem> _itemList)
    {
        costList = _itemList;
    }

    //각 tokenType의 enum에 따라 생성된 배열 밸류를 다시 ToderItemList로 돌리는 함수
    public TItemListData(int[] _values, TokenType _tokenType)
    {
        costList = new();
        for (int i = 0; i < _values.Length; i++)
        {
            TOrderItem item = new TOrderItem(_tokenType, i, _values[i]);
            costList.Add(item);
        }
    }

    public void Add(TOrderItem _orderItem)
    {
        if (costList == null)
            costList = new();
        
        costList.Add(_orderItem);
    }

    public List<TOrderItem> GetItemList()
    {
        List<TOrderItem> copyList = new();
        if (costList == null)
            return copyList;

        //필요재료를 여러 군데서 쓸 수 있으므로 원본 훼손이 되지 않도록 카피본을 만들어서 반환 .
        for (int i = 0; i < costList.Count; i++)
        {
            TOrderItem copyItem = costList[i];
            copyList.Add(copyItem);
        }

        return copyList;
    }
}