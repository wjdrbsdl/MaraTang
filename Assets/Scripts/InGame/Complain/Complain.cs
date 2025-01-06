
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum ComplaintTypeEnum
{
    Compensation, Nomal, Accident
}

public enum ComplainRequestTypeEnum
{
    Hunt, Item
}

public class Complain
{
    public string Name;
    public ComplaintTypeEnum ComplainType = ComplaintTypeEnum.Nomal ; // 컴플레인의 부류 - 자원 요구, 특정 스텟 요구치로 단순 확률싸움
    public List<TOrderItem> NeedItems = new(); // 필요한것들
    public int RestTurn; //인내기간
    public List<TOrderItem> FailEffect = new(); //실패시 어쩔지
    public List<TOrderItem> SuccesEffect = new(); //성공시 어쩔지
    public int[] MapIndex;
    public Complain()
    {
        //테스트용 아무거나
        Name = "테스트 컴플레인";
  
        NeedItems.Add(new TOrderItem(TokenType.Capital, (int)Capital.Food, 30));
     
        SuccesEffect.Add(new TOrderItem(TokenType.Capital, (int)Capital.Food, 30));

        FailEffect.Add(new TOrderItem(TokenType.Capital, (int)Capital.Food, -30));
        RestTurn = 3;
        GamePlayMaster.GetInstance().RegistorComplain(this);
    }

    #region 컴플레인 배정 및 턴 관리
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

    public void AssignTile(TokenTile _tile)
    {
        //기존 진행중이던 작업은 발생한 민원에 따라 영향
        // -> 중대사고인경우 기존 작업이 취소될수도, 노동코인이 상실될수도
        // ->미미한 경우는 해당 작업의 효율, 효과정도가 너프되는 버프에 걸릴 수도
        //1. 배정시 기존 작업에 어떤 영향을 미칠지
        //2. 타일의 스텟 버프등에 어떤 영향을 미칠지 정의 

        
        //3. 타일과 민원에 서로를 참조
        SetComplainMapIndex(_tile.GetMapIndex());
        _tile.SendComplain(this);
        TurnCount(); //세팅시 바로 카운트 진행 - 사건 사고의 경우 남은 턴이 바로 0 이하가 되며 효과가 즉발 
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
        GamePlayMaster.GetInstance().RemoveComplain(this);
    }
    #endregion

    #region 컴플레인 대응 과 결과
    public void Respond()
    {
        //컴플레인에 대응하기
        TokenChar mainChar = PlayerManager.GetInstance().GetMainChar();
        int RandomNum = Random.Range(0, 2);
        bool Succes = false;
        //필요한 아이템을 충족하는지 
        for (int i = 0; i < NeedItems.Count; i++)
        {
            //충족 여부 체크
        }

        //실패는 단번으로 끝날것인가? 예를들어 자원 없는데 시도했다고 끝날 순 없잖아 
        //성공한 경우에 성공 호출
       // Debug.Log("컴플레이 대응 성공 여부 " + Succes);
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
        AccidentAdaptor accident = new AccidentAdaptor();
        accident.AdaptPenalty(this);
    }

    public void AdaptItem(TOrderItem _item)
    {
        //별도로 할까. orderExcute에 다 정의해놓을가
        //char Stat의 경우는 일괄적
        //어떤건 특수적 흠..
    }
    #endregion

    public TokenTile GetTile()
    {
        //발생한 타일 
        TokenTile tile = GameUtil.GetTileTokenFromMap(MapIndex);
        return tile;
    }
}

