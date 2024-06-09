using System.Collections;
using UnityEngine;
using System.Collections.Generic;


public class CapitalRecipe
{
    //속성 넣었을 때 조합식
    public (Capital, int) MixCapital( List<(Capital, int)> _inputCapitals)
    {
        (Capital, int) finished = (0, 0);
        //조합순서에 따라 완제품이 달라지고
        //해당 조합순서는 넣은 재료의 비율에 따라 확률적으로 결정
        List<Capital> order = new(); //넣을 순서 - 넣어진 튜플 인덱스로 결정

        int totalAmount = 0;
        int minAmount = int.MaxValue;//최대 비율 수량
        int typeCount = _inputCapitals.Count;
        //1. 총 수량 및 최소 수량 산출
        for (int i = 0; i < typeCount; i++)
        {
            int curAmount = _inputCapitals[i].Item2;

            //최소 수량 산출 -> 최종 산출량
            if (curAmount < minAmount)
                minAmount = curAmount;

            totalAmount += curAmount;
        }
        //2. 비율대로 투입 순서 정함
        for (int i = 0; i < typeCount; i++)
        {
            int rulletNum = Random.Range(1, totalAmount + 1); // 1부터 총합까지 숫자에서 뽑을 숫자 나옴
          //  Debug.Log("뽑기 숫자" + rulletNum + "/" + totalAmount);
            int cutAmount = 0;
            for (int capitalIndex = 0; capitalIndex < _inputCapitals.Count; capitalIndex++)
            {
                cutAmount += _inputCapitals[capitalIndex].Item2; //첫번째 녀석부터 수량을 더해가며 해당 번호가 뽑혔는지 체크
                if(rulletNum <= cutAmount)
                {
                    //현재 컷어마운트 이내에 뽑은 숫자가 있으면 당첨된거 
                    order.Add(_inputCapitals[capitalIndex].Item1); //해당 재료를 넣는걸로 하고
                    totalAmount -= _inputCapitals[capitalIndex].Item2; //총 량(룰렛범위)에서 이번 수량을 뺌
                    _inputCapitals.RemoveAt(capitalIndex); //해당 재료는 여기서 빼고,
                    break;
                }
            }
            //원하는 재료를 넣었으면 위에서 다시 리셋해서 진행. 마지막에는 1재료만 남고 그재료가 뽑힘.
        }

        finished.Item1 = RecipeMixRule(order);
        finished.Item2 = minAmount;

        return finished;
    }

    public (Capital, int) ChangeCapital((Capital, int) _input, Capital _outCapital)
    {
        int chagedAmount = 55;

        return (_outCapital, chagedAmount);
    }

    private Capital RecipeMixRule(List<Capital> _orderList)
    {
        Capital tempFinished = Capital.Food;

        //투입된 순서로 조합 산출
        tempFinished = _orderList[0]; //일단 첫번째걸로 조합되는걸로

        return tempFinished;
    }

    
}
