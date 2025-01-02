using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public enum BlessSynergeCategoryEnum
{
    Main, God, Pid
}

public class BlessSynergeData
{
    public int PID;
    public string Name;
    List<TOrderItem> m_needBlessList; //시너지 활성화에 필요한 가호 조건 리스트
    List<List<TOrderItem>> m_effectList;
    List<int> m_needCount;
    public BlessSynergeData(string[] _divdeValues)
    {
        PID = int.Parse( _divdeValues[0]);
        int nameIdx = 1;
        Name = _divdeValues[nameIdx];

        int needBlessIdx = nameIdx + 1;
        m_needBlessList = new();
        GameUtil.ParseOrderItemList(m_needBlessList, _divdeValues[needBlessIdx]);
        m_needBlessList.Sort((TOrderItem A, TOrderItem B) => B.SubIdx.CompareTo(A.SubIdx));

        int needCountIdx = needBlessIdx +1;
        m_needCount = new();
        int effectIdx = 5;
        m_effectList = new();
        if(_divdeValues.Length>= 6)
        {
            string[] effectStrs = _divdeValues[effectIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
            string[] needCountStrs = _divdeValues[needCountIdx].Split(FixedValue.PARSING_LINE_DIVIDE);
            for (int i = 0; i < effectStrs.Length; i++)
            {
                m_needCount.Add(int.Parse(needCountStrs[i]));
                m_effectList.Add(new List<TOrderItem>());
                GameUtil.ParseOrderItemList(m_effectList[i], effectStrs[i]);
            }
        }
        
        //서브 인덱스가 클수록 더 좁은 범위 먼저 확인해야하므로 앞으로 배치 
        
    }

    public int CheckSynergeStep(TokenChar _char)
    {
        List<GodBless> haveBless = _char.GetBlessList();
        List<int> checkIdx = new(); //체크된건 여기 추가해서 중복확인
        int needCount = m_needBlessList.Count;
        int checkCount = 0;
        int step = 0;
        for (int i = 0; i < needCount; i++)
        {
            BlessSynergeCategoryEnum category = (BlessSynergeCategoryEnum)m_needBlessList[i].SubIdx;
            for (int x = 0; x < haveBless.Count; x++)
            {
                GodBless bless = haveBless[x]; //확인할 가호
                if(CheckDetail(category, m_needBlessList[i].Value, bless) == true)//조건 충족 가호가 있고
                {
                    //사용 안된 가호라면
                    if(checkIdx.IndexOf(x) == -1)
                    {
                        checkIdx.Add(x); //해당 인덱스 추가하고 
                        checkCount +=1; //충족수 +1
                        break; //다음 거 확인
                    }
                } 
            }
        }
        //충족 수에 따라 현재 적용할 시너지가 몇단계인지 도출
        for (int i = m_needCount.Count-1; i >= 0; i--)
        {
            if (m_needCount[i]<= checkCount)
            {
                step = i + 1;
                break;
            }
        }
        return step;
    }

    private bool CheckDetail(BlessSynergeCategoryEnum _category, int _value, GodBless _bless)
    {
        switch (_category)
        {
            case BlessSynergeCategoryEnum.Pid:
                if (_bless.GetPid() == _value)
                {
                    return true;
                }
                return false;
            case BlessSynergeCategoryEnum.God:
                if(_value == _bless.m_godPid)
                {
                    return true;
                }
                return false;
            case BlessSynergeCategoryEnum.Main:
                if(_value == (int)_bless.m_classCategory)
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

    public List<TOrderItem> GetEffectList(int _step)
    {
        if (_step == 0)
            return new List<TOrderItem>(); //아무것도 없는거 반환

        //이펙트는 1단계부터 정의되어 있어서 인덱스는 -1 해줘야함. 
        return m_effectList[_step-1];
    }
}
