using System.Collections;
using UnityEngine;

public enum PenaltyType
{
    ConditionFail, PolluteChunk, DestroyTile, AccelBoss
}
public class PenaltyData : IOrderCustomer
{
    public PenaltyType m_penaltyType;

    public PenaltyData()
    {

    }

    public void OnOrderCallBack(OrderReceipt _orderReceipt) //페널티 고객
    {
        Debug.Log("페널티 고객");
    }
}
