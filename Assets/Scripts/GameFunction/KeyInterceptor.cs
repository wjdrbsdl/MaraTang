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

    //이전 키 사용자 기록 - 키 반환 할때 이전 사용자로 돌림- 이전사용자는 다시 이전사용자로 돌리기 
    public void SetPreIntecoptor(KeyInterceptor _ceptor);
}
