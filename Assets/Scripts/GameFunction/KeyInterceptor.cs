using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface KeyInterceptor
{
    public void ClickTokenBase(TokenBase _tokenBase);

    public void DoubleClickTokenBase(TokenBase _tokenBase);

    public void PushNumKey(int _keyNum);
}
