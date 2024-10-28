using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public enum BlessSynergeCategoryEnum
{
    Main, God, Pid
}

public class BlessSynerge
{
    public int PID;
    public string Name;
    List<TOrderItem> m_needBlessList; //시너지 활성화에 필요한 가호 조건 리스트

    public BlessSynerge(string[] _divdeValues)
    {
        PID = int.Parse( _divdeValues[0]);
        Name = _divdeValues[1];
        m_needBlessList = new();
        GameUtil.ParseOrderItemList(m_needBlessList, _divdeValues[2]);
        //for (int i = 0; i < m_needBlessList.Count; i++)
        //{
        //    TOrderItem blessItem = m_needBlessList[i];
        //    Debug.Log(Name + "시너지 필요한 조건은 " + (BlessSynergeCategoryEnum)blessItem.SubIdx + "중 " + blessItem.Value + "계열");
        //}
        
    }

}
