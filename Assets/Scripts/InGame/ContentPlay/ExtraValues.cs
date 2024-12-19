using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraValues
{
    public decimal Enforce1Ratio;
    public decimal Enforce2Ratio;
    public decimal Enforce3Ratio;
    public decimal Enforce4Ratio;
    public decimal CorruptionRatio;
    public static ExtraValues g_instance;
    public int FirstSupplyMeal;

    public ExtraValues(string[] valueCode)
    {
        g_instance = this;
        int enforce1Index = 0;
        int enforce2Index = enforce1Index+ 1;
        int enforce3Index = enforce2Index+ 1;
        int enforce4Index = enforce3Index+ 1;
        int supplyIndex = enforce4Index + 1;

        Enforce1Ratio = decimal.Parse(valueCode[enforce1Index]);
        Enforce2Ratio = decimal.Parse(valueCode[enforce2Index]);
        Enforce3Ratio = decimal.Parse(valueCode[enforce3Index]);
        Enforce4Ratio = decimal.Parse(valueCode[enforce4Index]);
        FirstSupplyMeal = int.Parse(valueCode[supplyIndex]);
       // Debug.Log("4단계 강화 수치 " + Enforce4Ratio);
    }

    public decimal GetEnforceValue()
    {
        decimal ratio = 1;
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
