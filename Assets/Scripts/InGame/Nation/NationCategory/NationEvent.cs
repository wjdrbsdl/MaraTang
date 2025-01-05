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
        compleMg.OccurComplain(m_nation);
    }

}
