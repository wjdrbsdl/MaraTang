using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData
{
    private int Pid; //대사 식별번호
    private string ScriptData; //대사
    private int ReceiveAnswer; //플레이어가 선택한 값

    public ConversationData(int _pid, string _script)
    {
        Pid = _pid;
        ScriptData = _script;
    }

    public string GetScript()
    {
        return ScriptData;
    }
}