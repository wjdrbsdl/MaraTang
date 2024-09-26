using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum GuildCardStatEnum
{
    Grade, ClearCount, GuildPoint
}

public enum GuildGradeEnum
{
    First, Second, Third
}
public class GuildCard
{
    private int[] m_guildCardValues = new int[GameUtil.EnumLength(GuildCardStatEnum.Grade)];

    #region 스텟적용
    public int GetStat(GuildCardStatEnum _enumIndex)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        return m_guildCardValues[index];
    }
    public void SetStatValue(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_guildCardValues[index] = _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    public virtual void CalStat(System.Enum _enumIndex, int _value)
    {
        int index = GameUtil.ParseEnumValue(_enumIndex);
        m_guildCardValues[index] += _value;
        //Debug.Log(m_tokenType + ": " + _enumIndex + ":" + m_tokenIValues[index]);
    }
    #endregion

    public GuildGradeEnum GetGuildGrade()
    {
        return (GuildGradeEnum) GetStat(GuildCardStatEnum.Grade);
    }
}

