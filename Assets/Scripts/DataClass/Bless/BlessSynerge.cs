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
        //서브 인덱스가 클수록 더 좁은 범위 먼저 확인해야하므로 앞으로 배치 
        m_needBlessList.Sort((TOrderItem A, TOrderItem B) => B.SubIdx.CompareTo(A.SubIdx));
    }

    public bool CheckSynerge(TokenChar _char)
    {
        List<GodBless> haveBless = _char.GetBlessList();
        List<int> checkIdx = new(); //체크된건 여기 추가해서 중복확인
        int needCount = m_needBlessList.Count;
        for (int i = 0; i < needCount; i++)
        {
            BlessSynergeCategoryEnum category = (BlessSynergeCategoryEnum)m_needBlessList[i].SubIdx;
            bool oneCondition = false; //해당 조건 충족인지 확인
            for (int x = 0; x < haveBless.Count; x++)
            {
                GodBless bless = haveBless[i]; //확인할 가호
                if(CheckDetail(category, m_needBlessList[i].Value, bless) == true)//조건 충족 가호가 있고
                {
                    //사용 안된 가호라면
                    if(checkIdx.IndexOf(x) == -1)
                    {
                        checkIdx.Add(x); //해당 인덱스 추가하고 
                        oneCondition = true; //해당 조건 충족으로 진행
                        break; //다음 거 확인
                    }
                } 
            }
            if(oneCondition == false)
            {
                //만약 하나라도 조건 안맞으면 실패
                return false;
            }
        }
        //무사히 for문 나왔다는건 모든 조건이 충족했다는 말
        return true;
    }

    private bool CheckDetail(BlessSynergeCategoryEnum _category, int _value, GodBless _bless)
    {
        switch (_category)
        {
            case BlessSynergeCategoryEnum.Pid:
                if (_bless.PID == _value)
                {
                    return true;
                }
                return false;
            case BlessSynergeCategoryEnum.God:
                if(_value == (int)_bless.m_subCategory)
                {
                    return true;
                }
                return false;
            case BlessSynergeCategoryEnum.Main:
                if(_value == (int)_bless.m_mainCategory)
                {
                    return true;
                }
                return false;
        }
        return false;
    }

    public void Log()
    {
        for (int i = 0; i < m_needBlessList.Count; i++)
        {
            TOrderItem blessItem = m_needBlessList[i];
            Debug.Log(Name + "시너지 필요한 조건은 " + (BlessSynergeCategoryEnum)blessItem.SubIdx + "중 " + blessItem.Value + "계열");
        }
    }

}
