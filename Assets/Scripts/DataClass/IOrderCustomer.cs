using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IOrderCustomer
{

    public void OrderCallBack(OrderReceipt _orderReceipt);
}

public struct OrderReceipt
{
    //오더 완료 주문서
    public TokenBase madeToken;
}
