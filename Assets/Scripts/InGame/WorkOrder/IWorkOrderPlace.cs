using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IWorkOrderPlace
{
    //작업 받는 장소에서 지녀야할 함수 -실시간으로 작업현황을 알수 없는 클래스에서 타일 공사 표기등 실시간으로 작업현황을 표시해야하는경우 필요.

    //어떤식으로든 자기 내부에 작업서를 지녀야함
    public bool RegisterWork(WorkOrder _work);

    //작업이 완성, 취소된 경우 해당 작업서를 내부에서 제거해야함. 
    public void RemoveWork();
 
}
