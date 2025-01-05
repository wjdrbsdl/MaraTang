
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//명칭은 enum이고 그 효과는 그 명칭에 걸맞게
public enum AccidentEnum
{
    폭파, 화재, 붕괴, 정전, 빙결
}


public class AccidentAdaptor
{
    /*
     * 효과 적용 분류
     * 1.내구도 감소 - 운행은 가능하나 내구도가 상실된 상황 - 수리와 동시에 기능 수행은 가능
     * -> 폭파, 붕괴, 화재
     * 2. 노동 공간 감소 - 일을 할 수 있는 공간이 상실됨 - 해당 슬롯 사라지기 -> 공간 확장공사 필요. 
     * -> 붕괴
     * 3. 노동 효율 감소 - 일을 완수하는데 오래걸림 - 해당 장소에 나태 버프 걸림 - 작업 정산시 효율 감소시켜서 진행
     * -> 정전, 나태, 공포
     * 4. 노동 효과 감소 - 일 완료시 효과 수금시 효율이 떨어짐 - 효과감소 버프 
     * -> 빙결, 기만, 공포
     * 5. 노동 코인 상실 - 비율로 상실 - 일할 공간, 슬롯은 남아있는데 거기 있던 노동코인이 죽음. 
     * -> 화재, 폭파, 붕괴
     */

    //민원에 대응 하지 못했을 때 발생하는 사고들 
    //민원에 실패했다. 그 실패한 Level을 보낸다. 
    //실패 유형도를 결정하고 - 그 유형도의 세기를 Level로 결정한다.
    //결정된 유형도대로 페널티를 적용한다. 
    List<TOrderItem> m_penaltyEffect; // 할당 받은 이펙트들

    public void AdaptPenalty(TokenTile _tile, int _penaltyLevel)
    {
        //1. 벌칙 수준에 따라 벌칙을 결정
        AccidentEnum adaptPenalty = SelectPenalty(_penaltyLevel);
        //2. 벌칙 강도 결정
        int power = CalPenaltyPower(adaptPenalty, _penaltyLevel);
        //3. 케이스별로 적용진행
        AdaptByCase(_tile, adaptPenalty, power);
    }

    private AccidentEnum SelectPenalty(int _penaltyLevel)
    {
        return AccidentEnum.정전;
    }

    private int CalPenaltyPower(AccidentEnum _accident, int _penaltyLevel)
    {
        //벌칙 타입과 벌칙수준에 따라 그 효력을 결정
        return 30;
    }

    private void AdaptByCase(TokenTile _targetTile, AccidentEnum _case, int _power)
    {
        switch (_case)
        {
            case AccidentEnum.폭파:
                break;

        }
    }
}

