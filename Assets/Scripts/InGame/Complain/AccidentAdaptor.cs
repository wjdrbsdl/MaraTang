
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//명칭은 enum이고 그 효과는 그 명칭에 걸맞게
public enum AccidentEnum
{
   화재, 붕괴, 정전, 빙결
}


public class AccidentAdaptor
{

    //민원에 대응 하지 못했을 때 발생하는 사고들 
    //민원에 실패했다. 그 실패한 Level을 보낸다. 
    //실패 유형도를 결정하고 - 그 유형도의 세기를 Level로 결정한다.
    //결정된 유형도대로 페널티를 적용한다. 
    List<TOrderItem> m_penaltyEffect; // 할당 받은 이펙트들

    public void AdaptPenalty(Complain _complaint)
    {
        int _penaltyLevel = CalPenaltyLevel(_complaint.ComplainType);
        if (_penaltyLevel == FixedValue.No_VALUE) //민원 유형이 보은인 경우엔 적용할 페널티 레벨이 없어서 리턴. 
            return;

        //1. 벌칙 수준에 따라 벌칙을 결정
        AccidentEnum adaptPenalty = SelectPenalty();
        //2. 벌칙 강도 결정
        int power = CalPenaltyPower(_penaltyLevel);
        //3. 케이스별로 적용진행
        TokenTile tile = _complaint.GetTile();
        if (tile == null)
            return;
        AdaptByCase(tile, adaptPenalty, power);
    }

    private int CalPenaltyLevel(ComplaintTypeEnum _complaintType)
    {
        //1,2,3,4 로서
        //기본 난이도에 추가 난이도 더해서 
        
        switch (_complaintType)
        {
            case ComplaintTypeEnum.Nomal:
                return Random.Range(1, 3); // 1~2
            case ComplaintTypeEnum.Accident:
                return Random.Range(3, 5); // 3~4
        }
        return FixedValue.No_VALUE; //없는 난이도는 0
    }

    private AccidentEnum SelectPenalty()
    {
        //사고 유형들중 하나 반환
        int accidentCount = System.Enum.GetValues(typeof(AccidentEnum)).Length;
        int randomIndex = Random.Range(0, accidentCount);

        return (AccidentEnum)randomIndex;
    }

    private int CalPenaltyPower(int _penaltyLevel)
    {
        //벌칙 타입과 벌칙수준에 따라 그 효력을 결정

        if (_penaltyLevel.Equals(1))
        {
            return Random.Range(1,3);
        }
        if (_penaltyLevel.Equals(2))
        {
            return Random.Range(3, 5);
        }
        if (_penaltyLevel.Equals(3))
        {
            return Random.Range(5, 8);
        }
        if (_penaltyLevel.Equals(4))
        {
            return Random.Range(8, 11);
        }
        return FixedValue.No_VALUE;
    }

    private void AdaptByCase(TokenTile _targetTile, AccidentEnum _case, int _power)
    {
        decimal ratio = _power * 0.1m; // 1~10 인수치에 %로 0.1을 곱해서 진행 
        Debug.Log(_case + "사고발생");
        switch (_case)
        {
            case AccidentEnum.붕괴://장소 내구도 감소
                //power에 따라 최대 내구도의 비율로 상실
                int maxHp = _targetTile.GetStat(ETileStat.MaxDurability);
                int damage = (int)(maxHp * ratio);
                _targetTile.AttackTile(damage); //붕괴로 인한 피해
                break;
            case AccidentEnum.화재: //노동 코인 손실
                //power에 따라 현재 할당된 노동 코인의 비율로 상실로 해당 타일의 노동코인에 영향
                int laborCount = _targetTile.GetLaborCoinCount();
                int lossCount = (int)(laborCount * ratio);
                break;
            case AccidentEnum.정전: //작업 효율 감소
                //power에 따라 n턴 동안 작업 효율이 감소 되는 디버프를 해당 장소에 검
                break;
            case AccidentEnum.빙결: //작업 효과 감소
                //power에 따라 n 턴 동안 작업 완료시 얻는 효과가 감소 되는 디버프를 해당 장소에 검
                break;
        }
    }
}

