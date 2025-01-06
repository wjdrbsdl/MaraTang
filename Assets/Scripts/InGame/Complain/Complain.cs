
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public enum ComplaintTypeEnum
{
    Compensation, Nomal, Accident
}

public enum ComplaintRequestType
{
    Hunt, Capital
}

public class Complain
{
    public string Name;
    public ComplaintTypeEnum ComplainType = ComplaintTypeEnum.Accident ; // 컴플레인의 부류 - 자원 요구, 특정 스텟 요구치로 단순 확률싸움
    public ComplaintRequestType RequestType = ComplaintRequestType.Hunt;
    public bool IsContentTarget = false;
    public List<TOrderItem> NeedItems = new(); // 필요한것들
    public int RestTurn; //인내기간
    public List<TOrderItem> SuccesEffect = new(); //성공시 어쩔지
    public int[] MapIndex;

    #region 컴플레인 생성
    public Complain()
    {
        //테스트용 아무거나
        Name = "테스트 컴플레인";
        //1. 범용성 결정
        int ran = Random.Range(0, 2);
        if (ran.Equals(1))
            IsContentTarget = true;

        //2. 타겟팅 여부에 따라 갈래
        //-1. 컨텐츠 타입이면 구역중 하나를 정해서 해당 구역의 내용에 따라 요청타입을 설정, 내부 아이템을 카운팅으로 진행, 해당 구역 갱신기간 재설정
        //-2. 무상관이면 요청 타입을 결정 그에 맞게 재료나 몬스터 사냥 카운트를 진행
        NeedItems.Clear();
        if (IsContentTarget)
        {
            ChunkContentSetting();
        }
        else
        {
            NomalSetting();
        }
        SuccesEffect.Add(new TOrderItem(TokenType.Capital, (int)Capital.Food, 30));
        GamePlayMaster.GetInstance().RegistorComplain(this);
        //토지 할당은 컴플레인 매니저에서 AssingTile 호출해서 진행
    }

    private void ChunkContentSetting()
    {
        //1. 컨텐츠 매니저에게 민원 대상이 될만한 아이템 요구
        TOrderItem needItem = MGContent.GetInstance().GetComplaintChunkItem(out int chunkNum);
        if (needItem.Tokentype.Equals(TokenType.None))
        {
            //구역에서 민원 대상이 될만한게 없으면 그냥 노말세팅으로 전환
            NomalSetting();
            return;
        }
        //2. 할당된 아잍메에 따라 요청 타입 수정
        if (needItem.Tokentype.Equals(TokenType.Capital))
        {
            RequestType = ComplaintRequestType.Capital;
        }

        //3. 필요 재료 추가
        NeedItems.Add(needItem); //
        //4. 남은 턴 설정하고, 구역 컨텐츠의 남은 시간도 갱신
        RestTurn = 20;
        //5. 둘중 긴걸로 진행
        int maxRestTurn = Mathf.Max(MGContent.GetInstance().GetChunk(chunkNum).PreContent.RestTurn, RestTurn);
        MGContent.GetInstance().GetChunk(chunkNum).PreContent.RestTurn = maxRestTurn; //컴플레인 해결을 위해 남은시간 갱신
    }

    private void NomalSetting()
    {
        //1. 요청 타입을 결정
        int ran = Random.Range(0, 2);
        RequestType = (ComplaintRequestType)ran;

        //2. 요청 타입에 따라 필요한것들 세팅
        if (RequestType.Equals(ComplaintRequestType.Capital))
        {
            TOrderItem capitalItem = new TOrderItem(TokenType.Capital, (int)Capital.Food, 30);
            NeedItems.Add(capitalItem);
        }
        else
        {
            //몬스터 Hunt 인경우 몬스터 카운트만 되면됨 pid 무관인데 흠. 
            TOrderItem huntItem = new TOrderItem(TokenType.Char, int.MaxValue, 4);
            NeedItems.Add(huntItem);
        }

        //3. 민원 대응 시간 설정
        RestTurn = 10;
    }
    #endregion

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
        //1. 타일과 민원에 서로를 참조
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

