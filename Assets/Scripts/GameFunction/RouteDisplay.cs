using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RouteDisplay 
{
    //가려는 길을 표시해주는 역할
    private List<TokenTile> preRoutes;
    private List<TokenTile> curRotues;
    private TMP_Text textSample;//샘플 오브젝트를 받아서 해당 오브젝트를 표기하려는 tokenObject 자식으로 이동. 
    [SerializeField]
    private TMP_Text[] text; //동적으로 필요한 횟수만큼 만들어서 표기 

    public void ShowRoute(List<TokenTile> _tiles)
    {
        ResetPreRoute();
        for (int i = 0; i < _tiles.Count; i++)
        {
            _tiles[i].ShowRouteNumber(i + 1);//1부터 세므로 +1
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

