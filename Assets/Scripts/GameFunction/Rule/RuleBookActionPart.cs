using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class RuleBookActionPart
{
    public void UseActionSlot()
    {

    }

    public List<TokenTile> GetActionRangeTile(int[] _caster, int[] _targetPos, int _range)
    {
      
        return GetRotateRangeTile(_caster,_targetPos,_range); //우선 시계방향 타겟팅만 구현
    }

    public List<TokenTile> GetRotateRangeTile(int[] _caster, int[] _targetPos, int _range)
    {
        //타겟 지점, 범위 적용 방식, 범위 수치에 따라 포함될 범위 타일을 반환하는곳. 
        List<TokenTile> rangedTile = new();
        //왼쪽회전
        int distance = GameUtil.GetMinDistance(_caster, _targetPos);

        //1. 해당 거리에 있는 모든 타일을 찾는다
        //- 왼쪽부터 시작해서 시계방향으로 정렬되어있음. 
        //존재하지 않는 타일도 리스트에 포함 - 제거하고 진행하면 남은 타일수가 줄어서 range 효과가 중첩되버림
        List<int[]> index = GameUtil.GetTilePosListInRangeNoLand(distance, _caster[0], _caster[1], distance);
        int start = 0;

        //2. 타겟지점이 일치하는 index를 찾는다.
        for (int i = 0; i < index.Count; i++)
        {
            int[] check = index[i];
            if (_targetPos[0] == check[0] && _targetPos[1] == check[1])
            {
                start = i;
                break;
            }
        }

        //3. 일치하는 start부터 index를 진행해가면 시계방향으로 타일을 소화하게 된다. 만약 끝 인덱스를 갔다면 0으로 돌아가면된다. 
        for (int i = 1; i <= _range; i++)
        {
            TokenTile tile = GameUtil.GetTileTokenFromMap(index[start]);
            if (tile != null) //범위 타일을 받아올때 거르지 않기 때문에 여기서 null을 걸러서 최종 범위에 넣어야함. 
            {
                tile.Dye(Color.yellow);
                rangedTile.Add(tile);
            }

            start += 1;
            if (start >= index.Count)
                start = 0;
        }
        //Debug.Log("할당된 타일 수 " + rangedTile.Count);
        return rangedTile;
    }
}
