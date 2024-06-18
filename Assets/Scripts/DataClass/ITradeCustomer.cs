using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ITradeCustomer
{

    public void CheckInventory(List<(TokenType,int,int)> _costList);
    public void PayInventory(List<(TokenType, int, int)> _costList);
}
