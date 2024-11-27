using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NationStatPart
{
    private Nation m_nation;
    private int[] nationStatValues;

    public NationStatPart(Nation _nation)
    {
        m_nation = _nation;
        nationStatValues = _nation.GetStatValues();
    }

    public void RelateStat()
    {
        //환경도에 따라서 정서수치의 +-를 산출
        //기존 정서수치에 따라서 산출된 +-에 보정 -> 극점일수록 더 낮게 변화 - 그런거없이 그냥 때려박을수도
        //이벤트성은 그냥 적용
        
    }

    public void AdaptEventStat(NationStatEnum _statEnum, int _value)
    {
        //보정없이 어차피 때려박을거면 굳이 여기타고 와서 할필요가 잇으려나. 
    }

    private void CalStat(NationStatEnum _nationStat, int _value)
    {
        int index = (int)_nationStat;
        nationStatValues[index] += _value;
        //Debug.Log(_nationStat + " 가" + _value + "적용");
        if (_nationStat.Equals(NationStatEnum.Happy))
        {
            if (nationStatValues[index] <= 0)
            {
                //   Debug.Log("행복도 마이너스 타락 진행");
            }
        }
    }

}
