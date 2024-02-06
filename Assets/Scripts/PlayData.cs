using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Capital
{
    Stone, Grass
}
public class PlayerCapitalData
{
  
    //게임을 플레이하면서 누적되는 데이터 
    protected int[] m_intValues; 
    protected float[] m_FloatValues;

    public PlayerCapitalData()
    {
        m_intValues = new int[System.Enum.GetValues(typeof(Capital)).Length];
    }

    #region 스텟 배열 적용하는 부분
    public int GetValue(System.Enum _enumIndex)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        return m_intValues[index];
    }
    public void SetValue(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_intValues[index] = _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    public void CalValue(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_intValues[index] += _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }

    #endregion

    public int[] GetCapitalValue()
    {
        return m_intValues;
    }

}
