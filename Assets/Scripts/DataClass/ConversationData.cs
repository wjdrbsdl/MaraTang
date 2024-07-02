using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData
{
    private int Pid; //��� �ĺ���ȣ
    private string ScriptData; //���
    private int ReceiveAnswer; //�÷��̾ ������ ��

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