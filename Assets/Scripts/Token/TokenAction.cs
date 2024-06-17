using UnityEngine;
using System.Collections.Generic;
using System;

public enum ActionType
{
    Move, Attack
}

public enum CharActionStat
{
   MinRange, Range, MinRatio, MaxCountInTurn, RemainCountInTurn, Power, NeedActionEnergy, NeedActionCount, Value2,
   CoolTime, RemainCool
}

public enum TileActionStat
{
    NeedActionEnergy, NeedActionCount, TileActionType, SubValue
}

public enum TileActionType
{
    Harvest = 1, Build, CapitalChef, LandUsage, Destroy
}


[System.Serializable]
public class TokenAction : TokenBase
{
    [SerializeField] 
    private ActionType actionType = ActionType.Move;
    private TokenType actionTarget = TokenType.Tile;
    private int[] m_targetPos; //작용할 위치 
    private TokenBase m_caster; //사용자\

    #region 액션 토큰 : 생성부분 추후 테이블 파싱 값으로 생성하기
    public TokenAction()
    {

    }

    public TokenAction(List<int[]> matchCode, string[] valueCode)
    {
        m_tokenPid = int.Parse(valueCode[0]);
        m_itemName = valueCode[1];
        actionType = (ActionType)(int.Parse(valueCode[2]));
        m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        GameUtil.InputMatchValue(ref m_tokenIValues, matchCode, valueCode);
        m_tokenIValues[(int)CharActionStat.RemainCountInTurn] = m_tokenIValues[(int)CharActionStat.MaxCountInTurn];
 
    }

    public TokenAction(TokenAction _masterToken)
    {
        m_tokenPid = _masterToken.m_tokenPid;
        m_itemName = _masterToken.m_itemName;
        m_tokenType = TokenType.Action;
        actionType = _masterToken.actionType;
        int arraySize = _masterToken.m_tokenIValues.Length;
        m_tokenIValues = new int[arraySize];
        //마스터 데이터 깊은 복사로 객체 고유 배열 값 생성. 
        System.Array.Copy(_masterToken.m_tokenIValues, m_tokenIValues, arraySize);
    }


    public TokenAction MakeTestAction(ActionType _type)
    {
        TokenAction actionToken = new TokenAction();
        actionToken.m_tokenIValues = new int[System.Enum.GetValues(typeof(CharActionStat)).Length];
        actionToken.m_targetPos = null;
        actionToken.m_tokenType = TokenType.Action;
        actionToken.actionType = _type;
        actionToken.SetStatValue(CharActionStat.MinRange, 1);
        if (_type.Equals(ActionType.Move))
        {
            actionToken.actionTarget = TokenType.Tile;
            actionToken.SetStatValue(CharActionStat.Range, 3);
            actionToken.SetStatValue(CharActionStat.MinRatio, 2);
            actionToken.SetStatValue(CharActionStat.RemainCountInTurn, 2);
            actionToken.SetStatValue(CharActionStat.MaxCountInTurn, 2);
            actionToken.SetStatValue(CharActionStat.NeedActionCount, 1);
            actionToken.SetStatValue(CharActionStat.CoolTime, 2);
        }
        else if (_type.Equals(ActionType.Attack))
        {
            actionToken.actionTarget = TokenType.Char;
            actionToken.SetStatValue(CharActionStat.Range, 1);
            actionToken.SetStatValue(CharActionStat.MinRatio, 1);
            actionToken.SetStatValue(CharActionStat.RemainCountInTurn, 1);
            actionToken.SetStatValue(CharActionStat.MaxCountInTurn, 1);
            actionToken.SetStatValue(CharActionStat.NeedActionCount, 0);
     
        }
        
        return actionToken;
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

    public void RcoverRemainCountInTurn()
    {
        int coolTime = GetStat(CharActionStat.RemainCool); //남은쿨을 보고
        if(0 < coolTime) //쿨이 돌고 있으면
        {
            CalStat(CharActionStat.RemainCool, -1); //1을 깐다. 
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
        int finalRange = GetStat(CharActionStat.Range);
        int dex = _char.GetStat(CharStat.Dexility);
        int ratio = GetStat(CharActionStat.MinRatio);
        float ratioValue = (finalRange * dex * ratio * 0.01f);
        finalRange =(int)( finalRange + ratioValue);
        return finalRange;
    }
}
