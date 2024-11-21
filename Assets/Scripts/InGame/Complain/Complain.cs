
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Complain
{
    public string Name;
    public string ComplainType; // 컴플레인의 부류 - 자원 요구, 특정 스텟 요구치로 단순 확률싸움
    public List<TOrderItem> NeedItems; // 필요한것들
    public int RestTurn; //인내기간
    public List<TOrderItem> FailEffect; //실패시 어쩔지
    public List<TOrderItem> SuccesEffect; //성공시 어쩔지
    public int[] MapIndex;
    public Complain()
    {
        //테스트용 아무거나
        Name = "테스트 컴플레인";
        ComplainType = "자원요구";
        NeedItems = new();
        NeedItems.Add(new TOrderItem(TokenType.Capital, (int)Capital.Food, 30));
        RestTurn = 3;
        FailEffect = new();
        SuccesEffect = new();
    }

    public void TurnCount()
    {
        //턴이 다 된경우 실패
        RestTurn -= 1;
        if (RestTurn == 0)
        {
            EffectPenalty();
            RemoveComplain();
        }
            
    }

    public void Respond()
    {
        //컴플레인에 대응하기
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        int RandomNum = Random.Range(0, 2);
        bool Succes = false;
        switch (ComplainType)
        {
            case "자원요구":
                //돈 계산
                Succes = RandomNum == 0; //50% 반반으로 반환
                break;
            case "스텟요구":
                //스텟으로 확률 계산
                Succes = true;
                break;
        }

        //실패는 단번으로 끝날것인가? 예를들어 자원 없는데 시도했다고 끝날 순 없잖아 
        //성공한 경우에 성공 호출
        Debug.Log("컴플레이 대응 성공 여부 " + Succes);
        if (Succes)
        {
            EffectReward();
            RemoveComplain();
        }
    }

    public void EffectReward()
    {
        OrderExcutor excutor = new OrderExcutor();

        //성공시 가능한 효과 모두를 담아서 선택지로 보내서 보상고르게함
        TTokenOrder order = new TTokenOrder(SuccesEffect, true, SuccesEffect.Count);
        excutor.ExcuteOrder(order);
    }

    public void EffectPenalty()
    {
        OrderExcutor excutor = new OrderExcutor();
        //실패했다면 있는거 그대로 다 적용
        for (int i = 0; i < FailEffect.Count; i++)
        {
            excutor.AdaptItem(FailEffect[i]);
        }
    }

    public void AdaptItem(TOrderItem _item)
    {
        //별도로 할까. orderExcute에 다 정의해놓을가
        //char Stat의 경우는 일괄적
        //어떤건 특수적 흠..
    }

    public void SetComplainMapIndex(int[] _pos)
    {
        MapIndex = _pos;
    }

    public void RemoveComplain()
    {
        Debug.Log("컴플레인 제거");
        TokenTile complainTile = GameUtil.GetTileTokenFromMap(MapIndex);
        complainTile.RemoveComplain();
    }
}

