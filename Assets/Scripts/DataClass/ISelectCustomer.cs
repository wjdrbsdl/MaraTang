using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISelectCustomer
{
    public void OnSelectCallBack(int _slotIndex);

    public void OnChangeValueCallBack(int _slotIndex, int _value);
    public void OnConfirm();
}
