using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgUI : MgGeneric<MgUI>
{
    [Header("액션")]
    [SerializeField] private UIActionTokenBox m_actionTokenBox;
    [SerializeField] private UIFillContent m_fillContentUI;
    [SerializeField] private UIEventContent m_eventContentUI;
    [SerializeField] private UITileWorkShop m_tileWorkShopUI;
    [SerializeField] private UIChefUI m_chefUI;

    [Header("데이터 표기")]
    [SerializeField] private UICapital m_capitalUI;
    [SerializeField] private UITokenSnapInfo m_snapInfoUI;
    [SerializeField] private UIPlayData m_playDataUI;

    [Header("컷씬")]
    [SerializeField] private UICutScene m_cutScene;

    [SerializeField] private UIShowcase m_shocaseUI;

    #region 플레이어 액션
    public void ShowActionToken()
    {
        //플레이어 캐릭터 눌렀을 때
        OffPlayUI();
        TokenChar _char = PlayerManager.GetInstance().GetSelectedChar();
        m_actionTokenBox.SetActionSlot(_char);
    }

    public void ShowTileWorkShopUI()
    {
        m_uiStack.Push(m_tileWorkShopUI);
        m_tileWorkShopUI.SetTileWorkShopInfo();
    }

    public void ShowSubUI(int subCode, TokenTile _tile, TokenAction _action)
    {
        if (subCode.Equals(1))
        {
            m_chefUI.SetChefUI(subCode, _tile, _action);
        }
        m_uiStack.Push(m_chefUI);
    }
    #endregion

    #region 이벤트 관련
    public void ShowEventList(List<TokenEvent> _eventTokens)
    {
        OffPlayUI();
        m_eventContentUI.ShowEventList(_eventTokens);
    }
    #endregion

    #region 현황 데이터 표기
    public void ResetCapitalInfo(PlayerCapitalData _capitalData)
    {
        m_capitalUI.ResetCapitalInfo(_capitalData);
    }

    public void ResetSnapInfo(TokenBase _token)
    {
        m_snapInfoUI.SetTokenSnapInfo(_token);
    }

    public void ResetPlayData()
    {
        m_playDataUI.ShowPlayData();
    }

    #endregion

    public void ShowCaseOpen(RectTransform _uiTransform, Action<ShowcaseSlot> _selectAction)
    {
        //얘를 열고, 그 위치도 조정
        m_shocaseUI.OpenWindow(_selectAction) ;
        m_shocaseUI.SizeControl(_uiTransform);

    }


    #region 활성화 UI 관리
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

    public void PushUIStack(UIBase _ui)
    {
        m_uiStack.Push(_ui);
    }

    public void OffPlayUI()
    {
        int stackCount = m_uiStack.Count;
        for (int i = 0; i < stackCount; i++)
        {
            CancleLastUI();
        }
    }
    #endregion
}
