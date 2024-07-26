using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MgUI : MgGeneric<MgUI>
{
    [Header("액션")]
    [SerializeField] private UIActionTokenBox m_actionTokenBox;
    [SerializeField] private UIRewardChoose m_rewardChooseUI;
    [SerializeField] private UITileInfo m_tileWorkShopUI;
    [SerializeField] private UICharStats m_charStatUI;
    [SerializeField] private UICaptailMix m_capitalMixUI;
    [SerializeField] private UICapitalChange m_capitalChangeUI;
    [SerializeField] private UISelectItem m_selectItemUI;
    [SerializeField] private UISelectItem m_selectTextUI;
    [SerializeField] private UIScript m_scriptUI;

    [Header("데이터 표기")]
    [SerializeField] private UINationPolicy m_policyUI;
    [SerializeField] private UICapital m_capitalUI;
    [SerializeField] private UITokenSnapInfo m_snapInfoUI;
    [SerializeField] private UIPlayData m_playDataUI;
    [SerializeField] private UIMiniMap m_miniMapUI;
    [SerializeField] private UIQuest m_questUI;
    [SerializeField] private UIQuestList m_questListUI;

    [Header("컷씬")]
    [SerializeField] private UICutScene m_cutScene;

    [SerializeField] private UIShowcase m_shocaseUI;

    #region 인터페이스 오픈
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

    public void ShowIconSelectList(SelectItemInfo _itemInfo)
    {
        m_selectItemUI.SetSelectedInfo(_itemInfo);
    }

    public void ShowTextSelectList(SelectItemInfo _itemInfo)
    {
        m_selectTextUI.SetSelectedInfo(_itemInfo);
    }

    public void ShowScript(ConversationData _scriptData)
    {
        m_scriptUI.SetScript(_scriptData);
    }

    public void SetScriptCustomer(SelectItemInfo _selectInfo)
    {
        m_scriptUI.SetSelectInfo(_selectInfo);
    }

    public void ShowNationPolicy(Nation _nation)
    {
        m_policyUI.SetNationPolicy(_nation);
    }

    private void ShowTileTokenInfo(TokenTile _tile)
    {
        TileType tileType = _tile.GetTileType();
        if (tileType.Equals(TileType.Capital))
        {
        //    Debug.Log("국가 트리 정보 보여주기");
            Nation tileNation = _tile.GetNation();
            foreach(KeyValuePair<int, NationTechData> tech in MgMasterData.GetInstance().GetTechDic())
            {
                NationTechData techTree = tech.Value;
                TItemListData researchCostData = techTree.ResearchCostData;
                bool isAble = tileNation.CheckInventory(researchCostData);
                //    Debug.Log(techTree.GetTechName()+" 학습 가능 여부 "+ isAble);

                tileNation.TechPart.CompleteTech(techTree.GetPid());
            }
            tileNation.TechPart.CalTechEffect();
           // ShowNationPolicy(tileNation);
        }

        PushUIStack(m_tileWorkShopUI);
        m_tileWorkShopUI.SetTileInfo(_tile, _tile.GetTileType());

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
            UIBase ui = m_uiStack.Peek();
            ui.ReqeustOff();
        }
    }

    public void PushUIStack(UIBase _ui)
    {
        if (_ui == null)
            return;

        if(m_uiStack.Contains(_ui) == false)
            m_uiStack.Push(_ui);
    }

    public void PopUIStack()
    {
        if(m_uiStack.Count >=1)
            m_uiStack.Pop();
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
