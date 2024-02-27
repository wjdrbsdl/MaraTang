using System.Collections;
using UnityEngine;

public enum PenaltyType
{
    ConditionFail, PolluteChunk, DestroyTile, AccelBoss
}
public class PenaltyData
{
    public PenaltyType m_penaltyType;

    public PenaltyData()
    {

    }
}
