using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MgGodBless : Mg<MgGodBless>
{
    //은총 데이터 모아놓고
    //신전으로부터 은총 요구를 받으면 적당한 은총을 하사
    private List<God> m_activeGodList = new();
    private List<GodBless> m_blessList = new();

    public MgGodBless()
    {
        InitiSet();
    }

    public override void InitiSet()
    {
        g_instance = this;
    }

    public override void ReferenceSet()
    {
        Dictionary<int, God> godDic = MgMasterData.GetInstance().GetGodDic();
        foreach(KeyValuePair<int, God> item in godDic)
        {
            God god = item.Value;
            //석상 발견이 필요없는 티어의 신이라면
            if (god.Tier != 3)
            {
                //해당 클래스는 복사본 없이 원본을 사용해도 무관
                m_activeGodList.Add(god);
            }
        }

        Debug.Log("바로 사용가능한 신 종류 " + m_activeGodList.Count);

        for (int i = 0; i < 5; i++)
        {
            GodBless newBless = new GodBless();
            m_blessList.Add(newBless);
        }
    }

    public GodBless PleaseBless(BlessMainCategory _godClass)
    {
        Debug.Log("가진 것중 하사");
        //요청한 신전의 타입에 따라 
        //등장할 신의 타입 순서를 정하고 
        //각타입에서 활성화된 신을 뽑고
        //그 신에서 가능한 은총을 뽑는다
        //만약 해당 은총이 이미 보유중이면 다음 신으로 차례로 넘긴다 
        //내릴 수 있는 은총이 없다면 null? 반환

        return m_blessList[0];
    }

    public void ActiveGod(int _godID)
    {
        //활성화된 신을 마스터데이터에서 가져와서 활성화리스트에 추가 
    }
}
