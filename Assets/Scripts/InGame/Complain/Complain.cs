using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Complain
{
    public string Name;
    public string Type; // 컴플레인의 부류 - 자원 요구, 특정 스텟 요구치로 단순 확률싸움
    public List<TOrderItem> NeedItems; // 필요한것들
    public int RestTurn; //인내기간
    public List<TOrderItem> FailEffect; //실패시 어쩔지
    public List<TOrderItem> SuccesEffect; //성공시 어쩔지

    public Complain()
    {
        //테스트용 아무거나
        Name = "테스트 컴플레인";
        NeedItems = new();
        NeedItems.Add(new TOrderItem(TokenType.Capital, (int)Capital.Food, 30));
        RestTurn = 3;
        FailEffect = new();
        SuccesEffect = new();
    }

    public bool React()
    {
        //컴플레인에 대응하기
        TokenChar _char = PlayerManager.GetInstance().GetMainChar();
        switch (Type)
        {
            case "자원요구":
                //돈 계산
                return PlayerCapitalData.g_instance.CheckInventory(new TItemListData(NeedItems));
            case "스텟요구":
                //스텟으로 확률 계산
                return true;
        }
        return false;
    }

    public void Effect(bool _isSuccess)
    {
        if (_isSuccess)
        {

        }
    }

    public void AdaptItem(TOrderItem _item)
    {
        //별도로 할까. orderExcute에 다 정의해놓을가
        //char Stat의 경우는 일괄적
        //어떤건 특수적 흠..
    }
}

