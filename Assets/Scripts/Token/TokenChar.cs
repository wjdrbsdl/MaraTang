using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public enum CharStat
{
    MaxActionEnergy, CurActionEnergy, MaxActionCount, CurActionCount, Sight, Strenth, Dexility, Inteligent,
    MaxHp, CurHp
}

public class TokenChar : TokenBase, ITradeCustomer
{
    public bool isMainChar = false;
    public bool m_isPlayerChar = false;

    [JsonProperty] private CharState m_state = CharState.Idle;
    [JsonProperty] private List<TokenAction> m_haveActionList = new(); //이 캐릭터가 지니고 있는 액션 토큰들
    private TokenAction m_nextAction = null;
    private TokenBase m_nextTarget;
    

    #region 캐릭 토큰 생성부분
    public TokenChar()
    {

    }

    //마스터 캐릭데이터 생성
    public TokenChar(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]); //시트 데이터상 0번째는 pid
        m_itemName = valueCode[1]; //1은 이름
        SetAction(ref m_haveActionList, valueCode[2]); //2는 보유 액션 pid
        m_tokenType = TokenType.Char;
        m_tokenIValues = new int[System.Enum.GetValues(typeof(CharStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)CharStat.CurActionCount] = m_tokenIValues[(int)CharStat.MaxActionCount];
        m_tokenIValues[(int)CharStat.CurActionEnergy] = m_tokenIValues[(int)CharStat.MaxActionEnergy];
        m_tokenIValues[(int)CharStat.CurHp] = m_tokenIValues[(int)CharStat.MaxHp];
        
        
    }

    private void SetAction(ref List<TokenAction> _haveAction, string actionCode)
    {
        string[] actions = actionCode.Split(MgMasterData.DIVIDECHAR);
        for (int i = 0; i < actions.Length; i++)
        {
            int actionPid = int.Parse(actions[i]);
            TokenAction masterAction = MgMasterData.GetInstance().GetMasterCharAction(actionPid);
            //정의되지 않은 액션이라면 넘김. 
            if (masterAction == null)
                continue;
            TokenAction charAction = new TokenAction(masterAction);
            _haveAction.Add(charAction);
        }
    }

    //복사본 캐릭 생성 : 캐릭터 스폰 시 사용
    public TokenChar(TokenChar _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = TokenType.Char;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //마스터 데이터 깊은 복사로 객체 고유 배열 값 생성. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize); //스텟값 복사
        //스킬 마스터값 복사
        for (int i = 0; i < _masterToken.GetActionList().Count; i++)
        {
            TokenAction copyAction = new TokenAction(_masterToken.GetActionList()[i]);
            m_haveActionList.Add(copyAction);
        }
    }

    #endregion

    public void ShowAction(bool isShow)
    {
        if (m_object == null)
            return;

        if (isShow && m_nextAction != null)
            m_object.ShowActionIcon(m_nextAction.GetActionType());
        else if (isShow == false)
            m_object.OffActionIcon();
    }

    public void SetState(CharState _state)
    {
        m_state = _state;
        if (m_object == null)
            return;

        m_object.PlayAnimation(_state);
    }

    public void SetSprite()
    {
        //pid에 맞게 objectToken의 스프라이트를 변경
        int spriteIdx = m_tokenPid - 1;
        GetObject().SetSprite(TempSpriteBox.GetInstance().GetCharSprite(spriteIdx));
    }

    public CharState GetState()
    {
        return m_state;
    }

    #region Get
    public TokenBase GetTarget()
    {
        return m_nextTarget;
    }

    public int GetActionCount()
    {
        return m_tokenIValues[(int)CharStat.CurActionCount];
    }

    public List<TokenAction> GetActionList()
    {
        return m_haveActionList;
    }

    public TokenAction GetNextAction()
    {
        return m_nextAction;
    }

    public void ClearNextAction()
    {
        m_nextAction = null;
    }

    public TokenAction GetNextActionToken()
    {
        return m_nextAction;
    }

    public bool IsPlayerChar()
    {
        //나중엔 토큰매니저나 규칙으로 인해 m_isPlayerChar를 바꾸고 지금은 오브젝트에서 당기는걸로
        
        return m_isPlayerChar;
    }
    #endregion

    #region Set
    public void SetNextAction(TokenAction _action)
    {
        m_nextAction = (_action);
    }

    public override void SetMapIndex(int _x, int _y)
    {
        base.SetMapIndex(_x, _y); //좌표상 옮기고
       // Debug.Log("<color=yellow>"+num + "번" + _x + " ," + _y + "으로 옮김</color>");
    }

    public void SetObjectPostion(int _x, int _y)
    {
        m_object.SyncObjectPosition(_x, _y);
    }

    public void SetTarget(TokenBase _tokenBase)
    {
        m_nextTarget = _tokenBase;
    }

    public void AddActionToken(TokenAction _action)
    {
        m_haveActionList.Add(_action);
    }
    #endregion

    #region 액션 카운트 회복
    public void RecoverActionTokenCount()
    {
        for (int i = 0; i < m_haveActionList.Count; i++)
        {
            m_haveActionList[i].RcoverRemainCountInTurn();
        }
    }

    public void RecoverActionCount()
    {
        m_tokenIValues[(int)CharStat.CurActionCount] = m_tokenIValues[(int)CharStat.MaxActionCount];
    }

    public void UseActionCount(int _useCount = 1)
    {
        m_tokenIValues[(int)CharStat.CurActionCount] -= _useCount;
    }

    public void RecoverActionEnergy()
    {
        m_tokenIValues[(int)CharStat.CurActionEnergy] = m_tokenIValues[(int)CharStat.MaxActionEnergy];
    }

    public void UseActionEnergy(int _useEnergy = 1)
    {
        m_tokenIValues[(int)CharStat.CurActionEnergy] -= _useEnergy;
    }
    #endregion

    public void Death()
    {
        //0. 오브젝트 정리
        if (m_object != null)
            m_object.DestroyObject();

        //1. 사망시 처리
        SendQuestCallBack();
        DropItem();

        //2. 데이터 참조 제거
        TokenTile inTile = GameUtil.GetTileTokenFromMap(GetMapIndex());
        inTile.RemoveCharToken(this);
        MgToken.GetInstance().RemoveCharToken(this);
    }

    public NationPolicy NationPolicy;
    protected override void SendQuestCallBack()
    {
        base.SendQuestCallBack();
        if (NationPolicy != null)
            NationPolicy.SendNationCallBack(this);

    }

    public override void CleanToken()
    {
        base.CleanToken();
        Debug.Log("캐릭터 정리 들어감");
        //0. 오브젝트 정리 
        if (m_object != null)
            m_object.DestroyObject();

        //1. 데이터 참조 제거
        TokenTile place = GameUtil.GetTileTokenFromMap(GetMapIndex());
        place.RemoveCharToken(this);
        MgToken.GetInstance().RemoveCharToken(this);
    }

    public bool CheckInventory(OrderCostData _costData)
    {
        if(isMainChar == false)
        {
            //일단 메인캐릭에서만 진행 몬스터Token과 구별하려면 따로 playerToken 클래스를 생성해야함. 
            return false;
        }

        List<TOrderItem> BuildCostList = _costData.GetCostList();
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            Debug.LogFormat("{0}그룹의 {1} 인덱스의 필요수량 {2}", BuildCostList[i].Tokentype, BuildCostList[i].SubIdx, BuildCostList[i].Value);
            TokenType costType = BuildCostList[i].Tokentype;
            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                    if(PlayerCapitalData.g_instance.IsEnough((Capital)BuildCostList[i].SubIdx, BuildCostList[i].Value) == false)
                    {
                        Debug.Log("부족");
                      //  return false;
                    }
                    break;
                default:
                    Debug.Log("고려 파트 아닌 부분");
                    break;
            }
        }
        
        return true;
    }

    public void PayCostData(OrderCostData _costData)
    {
        if (isMainChar == false)
        {
            //일단 메인캐릭에서만 진행 몬스터Token과 구별하려면 따로 playerToken 클래스를 생성해야함. 
            return;
        }

        List<TOrderItem> BuildCostList = _costData.GetCostList();
        for (int i = 0; i < BuildCostList.Count; i++)
        {
            TokenType costType = BuildCostList[i].Tokentype;
            int subIdx = BuildCostList[i].SubIdx;
            int value = BuildCostList[i].Value;
            //각 토큰타입의 지불가능 형태를 따져 불가능하면 바로 false 반환 
            switch (costType)
            {
                case TokenType.Capital:
                    PlayerCapitalData.g_instance.CalCapital((Capital)subIdx, -value);
                    break;
                default:
                    Debug.Log("고려 파트 아닌 부분");
                    break;
            }
        }

    }
}
