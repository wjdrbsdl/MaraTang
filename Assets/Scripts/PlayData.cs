using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Capital
{
    Stone, Grass
}
public class PlayerCapitalData
{
  
    //������ �÷����ϸ鼭 �����Ǵ� ������ 
    protected int[] m_intValues; 
    protected float[] m_FloatValues;

    public PlayerCapitalData()
    {
        Debug.Log("���ʻ�����");
        m_intValues = new int[System.Enum.GetValues(typeof(Capital)).Length];
    }

    #region ���� �迭 �����ϴ� �κ�
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

        //  Debug.Log("����");
        return enumIntValue;
    }
    #endregion

    public int[] GetCapitalValue()
    {
        return m_intValues;
    }

}
