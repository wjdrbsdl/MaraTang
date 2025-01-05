using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ComplainManager : Mg<ComplainManager>
{
    private List<Complain> m_complinLIst; //민원 종류들

    #region 초기화
    public ComplainManager()
    {
        InitiSet();
    }

    public override void InitiSet()
    {
        g_instance = this;
    }

    public override void ReferenceSet()
    {
        //마스터데이터로부터 컴플레인 가져오기
        ClassfyComplain();
    }
    #endregion

    private void ClassfyComplain()
    {
        //마스터데이터로 부터 민원 종류들 가져와서 분류 

    }

    bool isDebugWarning = false; //ToDo 한번 알림용 변수

   public Complain OccurComplain(TokenTile _tile)
    {
        Nation nation = _tile.GetNation();
        //해당 장소에서 발생가능한 민원과 그 확률을 다른 스텟과 조율하여 민원 반환
        Complain occurComplain = null; //만드는순간 겜마스터에게 등록됨. 
        //분류된 민원을 가지고, 새로운 민원으로 복사해서 반환 
        if(isDebugWarning == false)
        {
            Debug.LogWarning("임시로 불만 제로");
            isDebugWarning = true;
        }
        
        return null;
    }

    //국가가 전달 되면 해당 국가에서 발생할 민원을 설정
    public void OccurComplain(Nation _nation)
    {
        List<TokenTile> territory = _nation.GetTerritorry();
        for (int i = 1; i < territory.Count; i++) //수도는 제외 1 부터 index
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

