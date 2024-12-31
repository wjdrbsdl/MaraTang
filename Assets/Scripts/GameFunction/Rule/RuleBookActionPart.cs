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
      
        return GetRotateRangeTile(_caster,_targetPos,_range); //�켱 �ð���� Ÿ���ø� ����
    }

    public List<TokenTile> GetRotateRangeTile(int[] _caster, int[] _targetPos, int _range)
    {
        //Ÿ�� ����, ���� ���� ���, ���� ��ġ�� ���� ���Ե� ���� Ÿ���� ��ȯ�ϴ°�. 
        List<TokenTile> rangedTile = new();
        //����ȸ��
        int distance = GameUtil.GetMinDistance(_caster, _targetPos);

        //1. �ش� �Ÿ��� �ִ� ��� Ÿ���� ã�´�
        //- ���ʺ��� �����ؼ� �ð�������� ���ĵǾ�����. 
        //�������� �ʴ� Ÿ�ϵ� ����Ʈ�� ���� - �����ϰ� �����ϸ� ���� Ÿ�ϼ��� �پ range ȿ���� ��ø�ǹ���
        List<int[]> index = GameUtil.GetTilePosListInRangeNoLand(distance, _caster[0], _caster[1], distance);
        int start = 0;

        //2. Ÿ�������� ��ġ�ϴ� index�� ã�´�.
        for (int i = 0; i < index.Count; i++)
        {
            int[] check = index[i];
            if (_targetPos[0] == check[0] && _targetPos[1] == check[1])
            {
                start = i;
                break;
            }
        }

        //3. ��ġ�ϴ� start���� index�� �����ذ��� �ð�������� Ÿ���� ��ȭ�ϰ� �ȴ�. ���� �� �ε����� ���ٸ� 0���� ���ư���ȴ�. 
        for (int i = 1; i <= _range; i++)
        {
            TokenTile tile = GameUtil.GetTileTokenFromMap(index[start]);
            if (tile != null) //���� Ÿ���� �޾ƿö� �Ÿ��� �ʱ� ������ ���⼭ null�� �ɷ��� ���� ������ �־����. 
            {
                tile.Dye(Color.yellow);
                rangedTile.Add(tile);
            }

            start += 1;
            if (start >= index.Count)
                start = 0;
        }
        //Debug.Log("�Ҵ�� Ÿ�� �� " + rangedTile.Count);
        return rangedTile;
    }
}
