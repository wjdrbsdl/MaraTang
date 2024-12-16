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

    public ExtraValues(string[] valueCode)
    {
        int enforce1Index = 0;
        int enforce2Index = enforce1Index+ 1;
        int enforce3Index = enforce2Index+ 1;
        int enforce4Index = enforce3Index+ 1;

        Enforce1Ratio = float.Parse(valueCode[enforce1Index]);
        Enforce2Ratio = float.Parse(valueCode[enforce2Index]);
        Enforce3Ratio = float.Parse(valueCode[enforce3Index]);
        Enforce4Ratio = float.Parse(valueCode[enforce4Index]);

    }
}
