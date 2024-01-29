using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCapitalData
{
    public enum Resource
    {
        Stone, Grass
    }
    //게임을 플레이하면서 누적되는 데이터 
    protected int[] m_intValues; 
    protected float[] m_FloatValues;

    public PlayerCapitalData()
    {
        Debug.Log("최초생ㅅㅇ");
        m_intValues = new int[System.Enum.GetValues(typeof(Resource)).Length];
    }

    #region 스텟 배열 적용하는 부분
    public int GetData(System.Enum _enumIndex)
    {
        int index = ParseEnumValue(_enumIndex);
        return m_intValues[index];
    }
    public void SetData(System.Enum _enumIndex, int _value)
    {
        int index = ParseEnumValue(_enumIndex);
        m_intValues[index] = _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    public void CalData(System.Enum _enumIndex, int _value)
    {
        int index = ParseEnumValue(_enumIndex);
        m_intValues[index] += _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }

    protected int ParseEnumValue(System.Enum _enumValue)
    {
        int enumIntValue = (int)System.Enum.Parse(_enumValue.GetType(), _enumValue.ToString());

        //  Debug.Log("들어옴");
        return enumIntValue;
    }
    #endregion


}
