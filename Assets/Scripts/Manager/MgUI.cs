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
    [SerializeField] private UICaptailMix m_capitalMixUI;
    [SerializeField] private UICapitalChange m_capitalChangeUI;

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

  
    public void ShowCharActionList()
    {
        //플레이어 캐릭터 눌렀을 때
        TokenChar _char = PlayerManager.GetInstance().GetSelectedChar();
        m_actionTokenBox.SetActionSlot(_char);
    }

    public void ShowTokenObjectInfo(TokenBase _token)
    {
        //토큰 오브젝트 눌렀을때 정보창 띄우기 
        TokenType tokenType = _token.GetTokenType();

        switch (tokenType)
        {
            case TokenType.Tile:
                TokenTile tileToken = (TokenTile)_token;
                ShowTileTokenInfo(tileToken);
                break;
            case TokenType.Char:
                PushUIStack(m_charStatUI);
                m_charStatUI.SetCharStat((TokenChar)_token);
                break;
            case TokenType.Event:
                Debug.Log("이벤트 정보 보여주기");
                break;
        }

    }

    public void ShowCapitalWorkShop(CapitalAction subCode, TokenTile _tile, TokenAction _action)
    {
        UIBase openUI = null;
        //재료 관련 작업, 서브 코드에 따라 적당한 UI 호출 
        if (subCode.Equals(CapitalAction.ChageCapital))
        {
            m_capitalChangeUI.SetChangeUI(_tile, _action);
            openUI = m_capitalChangeUI;
            return;
        }
        if (subCode.Equals(CapitalAction.MixCapital))
        {
            m_capitalMixUI.SetChefUI(_tile, _action);
            openUI = m_capitalMixUI;
            return;
        }

        PushUIStack(openUI);
    }

    public void ShowItemList(TTokenOrder _orderToken)
    {
        m_rewardChooseUI.ShowItemList(_orderToken);
    }

    private void ShowTileTokenInfo(TokenTile _tile)
    {
        TileType tileType = _tile.GetTileType();
        if (tileType.Equals(TileType.Capital))
        {
            Debug.Log("국가 트리 정보 보여주기");
            Nation tileNation = _tile.GetNation();
            foreach(KeyValuePair<int, NationTechTree> tech in MgMasterData.GetInstance().GetTechDic())
            {
                Debug.LogFormat("분류:{0} 이름:{1}, 필요 턴:{2}, 필요 광물:{3}, 필요나무 :{4}", 
                    tech.Value.GetTechValue(TechTreeStat.Class), 
                    tech.Value.GetTechName(),
                    tech.Value.GetTechValue(TechTreeStat.NeedTurn), 
                    tech.Value.GetTechValue(TechTreeStat.NeedMineral),
                    tech.Value.GetTechValue(TechTreeStat.NeedWood));

                if (tileNation != null)
                    tileNation.CompleteTech(tech.Key);
            }
            if (tileNation != null)
            {
                List<int> doneTech = tileNation.GetDoneTech();
                for (int i = 0; i < doneTech.Count; i++)
                {
                    Debug.LogFormat("{0}번 기술 완료 했으며 해당 기술 이름은 {1}", doneTech[i], MgMasterData.GetInstance().GetTechData(doneTech[i]).GetTechName());
                }
            }
                
        }


        PushUIStack(m_tileWorkShopUI);
        m_tileWorkShopUI.SetTileWorkShopInfo(_tile);

    }

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

    public void RefreshQuestList()
    {
        m_questListUI.RefreshList();
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
        if (_ui == null)
            return;

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
