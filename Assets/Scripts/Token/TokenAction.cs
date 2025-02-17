﻿using UnityEngine;
using System.Collections.Generic;
using System;

public enum ActionType
{
    Move, Attack, Wrongfulness
}

public enum CharActionStat
{
   MinLich, Lich, Range, MinRatio, MaxCountInTurn, RemainCountInTurn, Power, NeedActionEnergy, NeedActionCount, CoolTime, RemainCool,
    ArmorBreakRatio, ArmorBreakPower, FractureRatio, FracturePower, ChopRatio, ChopPower
}

[System.Serializable]
public class TokenAction : TokenBase
{
    [SerializeField] 
    private ActionType actionType = ActionType.Move;
    private TokenType actionTarget = TokenType.Tile;
    private int[] m_targetPos; //작용할 위치 
    private List<TOrderItem> m_powerRatio = new(); //효과에 적용되는 계수들
    private List<int> m_synergeList;
    private List<int> m_synergeStep;
    private List<TokenBuff> m_buffList = new(); //이스킬 사용시 적용시킬 버프들
    #region 액션 토큰 : 생성부분 추후 테이블 파싱 값으로 생성하기
    public TokenAction()
    {

    }

    public TokenAction(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]);
        m_itemName = valueCode[1];

        int actionTypeIndex = 2;
        actionType = (ActionType)(int.Parse(valueCode[actionTypeIndex]));

        int synergeIndex = actionTypeIndex +1;
        m_synergeList = new();
        m_synergeStep = new();
        GameUtil.ParseIntList(m_synergeList, valueCode, synergeIndex);
        
        for (int i = 0; i < m_synergeList.Count; i++)
        {
            m_synergeStep.Add(0); //시너지 수만큼 적용 단계를 0 으로 설정. 
        }

        int itemInfoIndex = synergeIndex + 1;
        m_itemInfo = valueCode[itemInfoIndex];

        int powerRatioIndex = itemInfoIndex + 1;
        GameUtil.ParseOrderItemList(m_powerRatio, valueCode, powerRatioIndex);


        m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)CharActionStat.RemainCountInTurn] = m_tokenIValues[(int)CharActionStat.MaxCountInTurn];

        
    }

    public TokenAction(TokenAction _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_itemInfo = _masterToken.m_itemInfo;
        m_tokenType = TokenType.Action;
        actionType = _masterToken.actionType;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //마스터 데이터 깊은 복사로 객체 고유 배열 값 생성. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize);
        m_synergeList = _masterToken.m_synergeList;
        m_powerRatio = _masterToken.m_powerRatio;

        m_synergeStep = new();
        for (int i = 0; i < m_synergeList.Count; i++)
        {
            m_synergeStep.Add(0); //시너지 수만큼 미적용 값을 추가 
        }

    }
    #endregion

    #region 타겟 수정 관련
    public ActionType GetActionType()
    {
        return actionType;
    }

    public int[] GetTargetPos()
    {
        return m_targetPos;
    }

    public TokenType GetTargetType()
    {
        return actionTarget;
    }

    public void SetTargetCoordi(int[] _mapIdx)
    {
        m_targetPos = _mapIdx;
    }

    public void ClearTarget()
    {
        m_targetPos =null;
    }
    #endregion

    #region 턴 정산
    public void TurnReset()
    {
        //스킬 액션 단계에서 리셋해야할것들 수행 
        RecoverRemainCountInTurn();
        ResetBuff();
    }

    private void RecoverRemainCountInTurn()
    {
        int coolTime = GetStat(CharActionStat.RemainCool); //남은쿨을 보고
        if (0 < coolTime) //쿨이 돌고 있으면
        {
            CalStat(CharActionStat.RemainCool, -1); //1을 깐다. 
        }

    }

    private void ResetBuff()
    {
        for (int i = 0; i < m_buffList.Count; i++)
        {
            m_buffList[i].Reset(); //액션 토큰에 할당된 버프들 리셋
        }
    }
    #endregion

    #region 스킬계수
    public override void CalStat(Enum _enumIndex, int _value)
    {
        base.CalStat(_enumIndex, _value);
        if (_enumIndex.Equals(CharActionStat.RemainCountInTurn))
        {
           if(GetStat(CharActionStat.RemainCountInTurn)< GetStat(CharActionStat.MaxCountInTurn))
            {
                //사용 했는데, 현재 쿨이 돌지 않고 있다면
                if(GetStat(CharActionStat.RemainCool) == 0) //즉 남은 쿨이 0 이라면
                {
                    int cool = GetStat(CharActionStat.CoolTime);
                    //쿨이 있으면 쿨을 돌리고
                    if (0 < cool)
                        SetStatValue(CharActionStat.RemainCool, cool); //해당 쿨만큼으로 세팅 
                    else //쿨이 없다면 무한 사용 횟수
                        SetStatValue(CharActionStat.RemainCountInTurn, GetStat(CharActionStat.MaxCountInTurn));
                }

            }
           else //남은 횟수의 변화를 줬는데 최대 수치 이상이라면
            {
                SetStatValue(CharActionStat.RemainCool, 0); //쿨을 0 으로 초기화 
            }
            return;
        }

        //쿨 관련 계산이였다면
        if (_enumIndex.Equals(CharActionStat.RemainCool))
        {
            //최대 수치 만큼 회복
            if (GetStat((CharActionStat.RemainCool)) == 0)
                SetStatValue(CharActionStat.RemainCountInTurn, GetStat(CharActionStat.MaxCountInTurn));
        }
    }

    
    public bool AbleUse()
    {
        //1. 남은 횟수가 1 이상이면
        if (0 == m_tokenIValues[(int)CharActionStat.RemainCountInTurn])
        {
            return false;
        }
        return true;
    }

    public int GetFinalRange(TokenChar _char)
    {
        int finalRange = GetStat(CharActionStat.Lich);
        int dex = _char.GetStat(CharStat.Dex);
        int ratio = GetStat(CharActionStat.MinRatio);
        float ratioValue = (finalRange * dex * ratio * 0.01f);
        finalRange =(int)( finalRange + ratioValue);
        return finalRange;
    }

    public List<TOrderItem> GetPowerRatio()
    {
        return m_powerRatio;
    }
    #endregion

    #region 가호 시너지
    public void CheckBlessSynerge(TokenChar _char)
    {
        for (int i = 0; i < m_synergeList.Count; i++)
        {
            BlessSynergeData synerge = MgMasterData.GetInstance().GetBlessSynergeData(m_synergeList[i]);
            int synergeStep = synerge.CheckSynergeStep(_char);
        
            if(synergeStep != m_synergeStep[i])
            {
                //충족된 단계가 현재 적용중인 step가 다른경우 - 올라갔거나 내려갔거나 무튼 변했음

                List<TOrderItem> curEffect = synerge.GetEffectList(m_synergeStep[i]); //현재 적용중인 효과 가져오고
                List<TOrderItem> newEffect = synerge.GetEffectList(synergeStep); //새로 적용될 효과 가져옴
                for (int x = 0; x < curEffect.Count; x++)
                {
                    //기존 효과 제거
                   RemoveEffect(curEffect[x]);
                }
                for (int x = 0; x < newEffect.Count; x++)
                {
                    //새 효과 적용
                    AdaptEffect(newEffect[x]);
                }
                m_synergeStep[i] = synergeStep; //새단계 저장
            }

            Debug.Log(synerge.Name + "시너지 단계 " + synergeStep + " 기존 단계로 활성 ");
        }
    }

    public List<int> GetSynergePidList()
    {
        return m_synergeList;
    }

    private void AdaptEffect(TOrderItem _item)
    {
       // Debug.Log("적용할건 " + GameUtil.GetTokenEnumName(_item));

        //각 시너지 효과를 그타입에 맞게 적용하기
        TokenType adaptType = _item.Tokentype;
        switch (adaptType)
        {
            case TokenType.ActionStat:
                CalStat((CharActionStat)_item.SubIdx, _item.Value);
                break;
            case TokenType.Buff:
                AddBuff(_item);
                break;
                
        }
    }

    private void RemoveEffect(TOrderItem _item)
    {
       // Debug.Log("해제 적용할건 " + GameUtil.GetTokenEnumName(_item));
        //각 시너지 효과를 그타입에 맞게 적용하기
        TokenType adaptType = _item.Tokentype;
        switch (adaptType)
        {
            case TokenType.ActionStat:
                TOrderItem reverse = GameUtil.ReverseItemValue(_item);
                CalStat((CharActionStat)reverse.SubIdx, reverse.Value);
                break;
            case TokenType.Buff:
                RemoveBuff(_item);
                break;

        }
    }
    #endregion

    #region 버프
    public List<TokenBuff> GetBuffList()
    {
        return m_buffList;
    }

    public TokenBuff GetBuffByIndex(int _index)
    {
        return m_buffList[_index];
    }

    private void AddBuff(TOrderItem _buffItem)
    {
        TokenBuff buff = new TokenBuff(MgMasterData.GetInstance().GetBuffData(_buffItem.SubIdx));
        Debug.Log("액션에 추가된 기능 버프 " + GameUtil.GetTokenEnumName(_buffItem));
        m_buffList.Add(buff);
    }

    private void RemoveBuff(TOrderItem _buffItem)
    {
        for (int i = 0; i < m_buffList.Count; i++)
        {
            if (m_buffList[i].GetPid() == _buffItem.SubIdx)
            {
                m_buffList.RemoveAt(i);
            }
        }
    }

    public int HaveBuffIndex(BuffEnum _buff)
    {
        int checkPid = (int)_buff;
        for (int i = 0; i < m_buffList.Count; i++)
        {
            if (m_buffList[i].GetPid() == checkPid)
                return i;
        }
        return FixedValue.No_INDEX_NUMBER;
    }
    #endregion

}
