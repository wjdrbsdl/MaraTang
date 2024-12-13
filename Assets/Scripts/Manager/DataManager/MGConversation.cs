using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MGConversation : Mg<MGConversation>
{
    List<(int, ConversationData)> m_scriptList = new(); //표기되어야할 리스트 
    int m_lastSerialNum = FixedValue.No_VALUE;
    public MGConversation()
    {
        g_instance = this;
    }

    private ConversationData GetConversationData(ConversationThemeEnum _theme, int _pid)
    {
      return  MgMasterData.GetInstance().GetConversationData(_theme, _pid);
    }
    
    public void ShowScriptItem(TOrderItem _scriptItem, int _serialNum)
    {
        ConversationData scriptData = GetConversationData((ConversationThemeEnum)_scriptItem.SubIdx, _scriptItem.Value);
        ShowScript(scriptData, _serialNum);
    }

    private void ShowScript(ConversationData _script, int _serialNum)
    {
        if (MgUI.GetInstance().ShowScript(_script, _serialNum) == false)
        {
            //대화를 열어달라고 요청 받았는데, 대화를 열지 못했을 경우 
            ReservateScript(_serialNum, _script);//예약하고 종료
            return;
        }
        //대화창에 잘 출력했으면
        m_lastSerialNum = _serialNum; //마지막 출력했던 고유번호 저장
    }

    public void RequestNextScrtip()
    {
      //  Debug.Log("예약 진행 전 남은 대사 수 " + m_scriptList.Count);
        if(m_scriptList.Count == 0)
        {
            return;
        }

       // Debug.Log("0번째 대사 출력 ");
        ShowScript(m_scriptList[0].Item2, m_scriptList[0].Item1);
        m_scriptList.RemoveAt(0);
      //  Debug.Log("출력 후 남은 대사 수 " + m_scriptList.Count);
    }

    private void ReservateScript(int _serialNum, ConversationData _scriptData)
    {
        //마지막 진행했던 대화와 같은 맥락의 대화면 0번째로 추가
     //   Debug.LogFormat("{0}번 고유번호로 식별번호:{1}, 대사 :{2} 예약", _serialNum, _scriptData.GetPid(), _scriptData.GetScript());
        if (m_lastSerialNum == _serialNum)
        {
            m_scriptList.Insert(0, (_serialNum, _scriptData));
            return;
        }
        //이외에는 맨 뒷 순서로 
        m_scriptList.Add((_serialNum, _scriptData)); //해당 대화를 씨리얼넘버와 함께 추가
     //   Debug.Log("예약된 카운트 " + m_scriptList.Count);
    }
}


