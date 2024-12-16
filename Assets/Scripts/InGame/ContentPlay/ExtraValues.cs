using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraValues
{
    public float Enforce1Ratio;
    public float Enforce2Ratio;
    public float Enforce3Ratio;
    public float Enforce4Ratio;
    public float CorruptionRatio;
    public static ExtraValues g_instance;

    public ExtraValues(string[] valueCode)
    {
        g_instance = this;
        int enforce1Index = 0;
        int enforce2Index = enforce1Index+ 1;
        int enforce3Index = enforce2Index+ 1;
        int enforce4Index = enforce3Index+ 1;

        Enforce1Ratio = float.Parse(valueCode[enforce1Index]);
        Enforce2Ratio = float.Parse(valueCode[enforce2Index]);
        Enforce3Ratio = float.Parse(valueCode[enforce3Index]);
        Enforce4Ratio = float.Parse(valueCode[enforce4Index]);
       // Debug.Log("4�ܰ� ��ȭ ��ġ " + Enforce4Ratio);
    }

    public float GetEnforceValue()
    {
        float ratio = 1;
        DevilProgress _devilLevel = MGContent.GetInstance().curDevilLevel;
        switch (_devilLevel)
        {
            case DevilProgress.Enforce1:
                ratio = Enforce1Ratio;
                break;
            case DevilProgress.Enforce2:
                ratio = Enforce2Ratio;
                break;
            case DevilProgress.Enforce3:
                ratio = Enforce3Ratio;
                break;
            case DevilProgress.Enforce4:
                ratio = Enforce4Ratio;
                break;
            case DevilProgress.Corruption:
                ratio = CorruptionRatio;
                break;
        }

        return ratio;
    }
}
