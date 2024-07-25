using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    //대화 선택창을 요구하고 싶으면 mgConversation으로 요청
    public void AskTextSelect(ConversationTheme _theme, int _start, int _end, Action _confirmAction = null, ITradeCustomer _giver = null, ITradeCustomer _taker = null)
    {
        List<TOrderItem> senetenceItems = MGConversation.GetInstance().GetSentenceItemList(_theme, _start, _end);
        SelectItemInfo selectInfo = new SelectItemInfo(senetenceItems, true);
        selectInfo.SetAction(_confirmAction);
        selectInfo.SetGiver(_giver);
        selectInfo.SetTaker(_taker);
        selectInfo.ShowScript();
    }
    
    public void ShowConverSation(TOrderItem _scriptItem)
    {
        ConversationData scriptData = GetConversationData((ConversationTheme)_scriptItem.SubIdx, _scriptItem.Value);
        MgUI.GetInstance().ShowScript(scriptData);
    }
}


