using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIConversation : UIBase
{
    //해당 타일의 정보 표기 및 타일에서 할 수 있는 작업을 제공하는 UI
    [SerializeField]
    private Image m_talkerImage;
    [SerializeField]
    private TMP_Text m_talkerText;

    public void SetConversation(ConversationData _conversation)
    {
        UISwitch(true);

        m_talkerText.text = _conversation.GetScript();
    }
}
