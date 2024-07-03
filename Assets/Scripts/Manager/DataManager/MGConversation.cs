using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MGConversation : Mg<MGConversation>
{
    public MGConversation()
    {
        g_instance = this;
    }

    public List<TOrderItem> GetSentenceItemList(ConversationTheme _theme, int _start, int _end)
    {
        List<TOrderItem> sentenceList = new List<TOrderItem>();
        for (int pid = _start; pid <= _end; pid++)
        {
            ConversationData conversation = GetConversationData(_theme, pid);
            if (conversation == null)
                continue;
           // Debug.Log("뽑아온 대화 pid" + conversation.GetPid()+"요구한 pid "+pid);
            TOrderItem sentenceItem = new TOrderItem(_theme, conversation);
            sentenceList.Add(sentenceItem);
        }

        return sentenceList;
    }

    private ConversationData GetConversationData(ConversationTheme _theme, int _pid)
    {
      return  MgMasterData.GetInstance().GetConversationData(_theme, _pid);
    }
}


