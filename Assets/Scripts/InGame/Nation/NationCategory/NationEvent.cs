using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NationEvent
{
    private Nation m_nation;
    private List<Complain> m_occuredComplainList; //현재 국가에 발생한 민원들


    public NationEvent(Nation _nation)
    {
        m_nation = _nation;
    }

    public void WatchEvent()
    {
        List<TokenTile> territory = m_nation.GetTerritorry();
        ComplainManager compleMg = ComplainManager.GetInstance();
        //Debug.Log(m_nation.GetNationNum() + " 사건 발생 유무 확인");
        for (int i = 0; i < territory.Count; i++)
        {
            TokenTile tile = territory[i];
            if (tile.HaveComplain())
            {
                continue;
            }
            Complain complain = compleMg.OccurComplain(tile);
            if (complain == null)
            {
                continue;
            }
            //임시로 여기서 좌표 새김, Complain 생성자에서 매개변수로 받은 위치로 생성하는게 좋아보임. 
            complain.SetComplainMapIndex(tile.GetMapIndex());
            tile.SendComplain(complain);
                
        }
    }

  
}
