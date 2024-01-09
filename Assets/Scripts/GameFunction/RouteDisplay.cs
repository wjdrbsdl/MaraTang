using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RouteDisplay 
{
    //������ ���� ǥ�����ִ� ����
    private List<TokenTile> preRoutes;
    private List<TokenTile> curRotues;
    private TMP_Text textSample;//���� ������Ʈ�� �޾Ƽ� �ش� ������Ʈ�� ǥ���Ϸ��� tokenObject �ڽ����� �̵�. 
    [SerializeField]
    private TMP_Text[] text; //�������� �ʿ��� Ƚ����ŭ ���� ǥ�� 

    public void ShowRoute(List<TokenTile> _tiles)
    {
        ResetPreRoute();
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].ShowRouteNumber(i + 1);//1���� ���Ƿ� +1
        }
        preRoutes = _tiles;
    }

    public void ResetPreRoute()
    {
        if (preRoutes == null)
            return;

        for (int i = 0; i < preRoutes.Count; i++)
        {
            preRoutes[i].OffRouteNumber();
        }

    }
}

