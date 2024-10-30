using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IWorkOrderPlace
{
    //작업 받는 장소
    public bool RegisterWork(WorkOrder _work);

    public void RemoveWork(WorkOrder _work);
 
}
