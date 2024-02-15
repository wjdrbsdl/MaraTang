using System.Collections;
using UnityEngine;


public enum EffectStat
{
   RestEffectCount, RestActionTurn, RestWoldTurn, ApplyTiming 
}
public class TokenEffect : TokenBase
{

    TokenEffect()
    {
        m_tokenIValues = new int[GameUtil.EnumLength(EffectStat.RestEffectCount)];
    }

    public void Effect()
    {
        if (GetStat(EffectStat.RestEffectCount) == 0)
            Debug.Log("남은 효과 없음");
    }

}
