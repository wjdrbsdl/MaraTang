using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTile : MonoBehaviour
{
    [SerializeField]
    private SpriteMask m_mask;

    public void MaskOn()
    {
        m_mask.backSortingOrder = 3;
    }

    public void FogOff()
    {
        m_mask.backSortingOrder = 2;
    }
}
