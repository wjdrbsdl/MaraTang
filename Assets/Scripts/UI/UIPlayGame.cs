using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIPlayGame : MonoBehaviour
{
    [SerializeField] private UIActionTokenBox m_actionTokenBox;
    [SerializeField] private UIFillContent m_fillContentUI;
    [SerializeField] private UIEventContent m_eventContentUI;
    [SerializeField] private UICapital m_capitalUI;
    [SerializeField] private UITokenSnapInfo m_snapInfoUI;
    [SerializeField] private UITileWorkShop m_tileWorkShopUI;
    [SerializeField] private UIBase[] m_offUIes; //껐다 켰따 할 리스트들

    private void Start()
    {
        m_offUIes = new UIBase[] { m_actionTokenBox, m_fillContentUI, m_eventContentUI, m_tileWorkShopUI };
    }

    public void ShowActionToken(TokenChar _char)
    {
        //플레이어 캐릭터 눌렀을 때 - 플레이어 매니저상 어떤 상태인지에 따라서 세팅하기 
        //1. 일단 내턴에서 액션 수행을 위한 선택으로 간주하고, action선택 ui에서 액션슬랏 세팅하도록 진행
        Debug.Log("내캐릭터 눌러짐");
        OffPlayUI();
        m_actionTokenBox.SetActionSlot(_char);
    }

    #region 내용 채우기 
    public void ShowFillContentUI(TokenChar _char, TokenAction _action)
    {
        OffPlayUI();
        m_fillContentUI.SetContentForm(_char, _action);
    }

    public void AddContent(TokenBase _contentTarget)
    {
        //해당 액션에 타겟이 추가 된 경우 //타겟이 옳은지 틀린지는 룰북에서 확인하고, 통과된 경우만 이곳으로 호출 UI는 오로지 입출력만 담당.
        m_fillContentUI.AddContent(_contentTarget);
    }
    #endregion

    #region 이벤트 관련
    public void ShowEventList(List<TokenEvent> _eventTokens)
    {
        OffPlayUI();
        m_eventContentUI.ShowEventList(_eventTokens);
    }
    #endregion

    public void ResetCapitalInfo(PlayerCapitalData _capitalData)
    {
        m_capitalUI.ResetCapitalInfo(_capitalData);
    }

    public void ResetSnapInfo(TokenBase _token)
    {
        m_snapInfoUI.SetTokenSnapInfo(_token);
    }

    public void ResetTileWorkShopUI(TokenTile _tile)
    {
        m_uiStack.Push(m_tileWorkShopUI);
        m_tileWorkShopUI.SetTileWorkShopInfo(_tile);
    }

    public void OffPlayUI()
    {
        for (int i = 0; i < m_offUIes.Length; i++)
        {
            m_offUIes[i].Switch(false);
        }
    }

    Stack<UIBase> m_uiStack = new();
    public bool CheckLastUI()
    {
        //행동 변화를 막을만한 UI가 펼쳐져 있는가
        
        //해당 스택에는 다른 상태로 넘어가면 안되는 UI가 있을 것
        if(m_uiStack.Count >= 1)
        {
            //마지막 UI
            //UIBase ui = m_uiStack.Pop();
            //ui.Switch(false);
            return true;
        }

        return false;
    }

    public void CancleLastUI()
    {
        if (m_uiStack.Count >= 1)
        {
            //마지막 UI를 꺼내서 취소 -> 취소 버튼으로 취소 가능한지 여부는 알아서 판단
            UIBase ui = m_uiStack.Pop();
            ui.Switch(false);
        }
    }
}
