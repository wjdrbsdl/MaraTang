using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConversationTheme
{
    Tutorial, YesOrNot
}

public enum ConversationStat
{
    Pid, Sentenece, Choose
}

public class ConversationData
{
    private int Pid; //대사 식별번호
    private string ScriptData; //대사
    private int ReceiveAnswer; //플레이어가 선택한 값

    public ConversationData(string[] _parsingData)
    {
        Pid = int.Parse(_parsingData[1]);
        ScriptData = _parsingData[2];
    }

    public string GetScript()
    {
        return ScriptData;
    }

    public int GetPid()
    {
        return Pid;
    }
}

public class ConversationGroup
{
    private Dictionary<int, ConversationData> ConversationDataDic;
    private ConversationTheme Theme;

    public ConversationGroup(ConversationTheme _theme)
    {
        Theme = _theme;
        ConversationDataDic = new Dictionary<int, ConversationData>();
    }

    public ConversationData GetConversationData(int _pid)
    {
        return GetDicData<ConversationData>(ConversationDataDic, _pid);
    }

    private T1 GetDicData<T1>(Dictionary<int, T1> _dic, int _pid)
    {
        if (_dic.ContainsKey(_pid))
            return _dic[_pid];

        return default(T1);
    }

    public void AddConversationData(ConversationData _data)
    {
        ConversationDataDic.Add(_data.GetPid(), _data);
    }
}
