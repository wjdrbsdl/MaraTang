﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgUI : MgGeneric<MgUI>
{
    [Header("액션")]
    [SerializeField] private UIActionTokenBox m_actionTokenBox;
    [SerializeField] private UIFillContent m_fillContentUI;
    [SerializeField] private UIRewardChoose m_rewardChooseUI;
    [SerializeField] private UITileWorkShop m_tileWorkShopUI;
    [SerializeField] private UICharStats m_charStatUI;
    [SerializeField] private UIMixer m_chefUI;

    [Header("데이터 표기")]
    [SerializeField] private UICapital m_capitalUI;
    [SerializeField] private UITokenSnapInfo m_snapInfoUI;
    [SerializeField] private UIPlayData m_playDataUI;
    [SerializeField] private UIMiniMap m_miniMapUI;
    [SerializeField] private UIQuest m_questUI;
    [SerializeField] private UIQuestList m_questListUI;

    [Header("컷씬")]
    [SerializeField] private UICutScene m_cutScene;

    [SerializeField] private UIShowcase m_shocaseUI;

    #region 플레이어 액션
    public void ShowActionToken()
    {
        //플레이어 캐릭터 눌렀을 때
        TokenChar _char = PlayerManager.GetInstance().GetSelectedChar();
        m_actionTokenBox.SetActionSlot(_char);
    }

    public void ShowTokenObjectUI(TokenBase _token)
    {
        TokenType tokenType = _token.GetTokenType();

        if (tokenType.Equals(TokenType.Tile))
        {
            m_uiStack.Push(m_tileWorkShopUI);
            m_tileWorkShopUI.SetTileWorkShopInfo((TokenTile)_token);
        }
        else if (tokenType.Equals(TokenType.Char))
        {
            m_uiStack.Push(m_charStatUI);
            m_charStatUI.SetCharStat((TokenChar)_token);
        }
    }

    public void ShowCapitalChefUI(CapitalAction subCode, TokenTile _tile, TokenAction _action)
    {
      
        m_chefUI.SetChefUI(subCode, _tile, _action);
      
        m_uiStack.Push(m_chefUI);
    }
    #endregion

    #region 이벤트 관련
    public void ShowEventList(List<TokenEvent> _eventTokens)
    {
        OffPlayUI();
        m_rewardChooseUI.ShowEventList(_eventTokens);
    }

    public void ShowRewardList(RewardData _reward)
    {
        m_rewardChooseUI.ShowRewardList(_reward);
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

    public void ShowQuest(Quest _quest)
    {
        m_questUI.SetQuestInfo(_quest);
    }

    public void ShowQuestList()
    {
        m_questListUI.SetQuestList();
    }

    #endregion

    public void ShowCaseOpen(RectTransform _uiTransform, Action<List<ShowcaseSlot>> _selectAction, int maxCount)
    {
        //얘를 열고, 그 위치도 조정
        m_shocaseUI.OpenWindow(_selectAction, maxCount) ;
        m_shocaseUI.SizeControl(_uiTransform);

    }

    public void ShowCaseOpen(RectTransform _uiTransform, InputSlot _inputSlot)
    {
        //얘를 열고, 그 위치도 조정
        m_shocaseUI.OpenWindow(_inputSlot);
        m_shocaseUI.SizeControl(_uiTransform);

    }

    #region 활성화 UI 관리
    Stack<UIBase> m_uiStack = new();
    public bool CheckOpenUI()
    {
        //행동 변화를 막을만한 UI가 펼쳐져 있는가
        if(m_uiStack.Count >= 1)
        {
             return true;
        }

        return false;
    }

    public void CancleLastUI()
    {
        //Debug.Log("종료 요청");
        if (m_uiStack.Count >= 1)
        {
            //마지막 UI를 꺼내서 취소 -> 취소 버튼으로 취소 가능한지 여부는 알아서 판단
            UIBase ui = m_uiStack.Pop();
            ui.Switch(false);
        }
    }

    public void PushUIStack(UIBase _ui)
    {
        if(m_uiStack.Contains(_ui) == false)
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
