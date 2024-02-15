using System.Collections;
using UnityEngine;


public enum EffectProperty
{
  RestEffectCount, RestActionTurn, RestWoldTurn, ApplyTiming, Power1, Power2
}

public class TokenEffect : TokenBase
{

    TokenEffect()
    {
        m_tokenIValues = new int[GameUtil.EnumLength(EffectProperty.RestEffectCount)];
    }

    public void Effect()
    {
        if (GetStat(EffectProperty.RestEffectCount) == 0)
            Debug.Log("남은 효과 없음");
    }

    public void UseTurn(bool _isActionTurn, int useCount = 1)
    {
        if (_isActionTurn)
        {
            CalStat(EffectProperty.RestActionTurn, - useCount);
        }
        //월드턴이면
        else
        {
            CalStat(EffectProperty.RestWoldTurn, - useCount);
        }
        
        if(IsRestTurn() == false)
        {
            //제거하기
        }

    }

    public bool IsRestTurn()
    {
        if(GetStat(EffectProperty.RestActionTurn) == 0 || GetStat(EffectProperty.RestWoldTurn) == 0)
        {
            return false;
        }

        return true;
    }
}
