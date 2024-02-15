using System.Collections;
using UnityEngine;


public enum EffectProperty
{
   RestEffectCount, RestActionTurn, RestWoldTurn, ApplyTiming 
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

}
