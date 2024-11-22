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

    bool isWarning = false;
   public Complain OccurComplain(TokenTile _tile)
    {
        Nation nation = _tile.GetNation();
        //해당 장소에서 발생가능한 민원과 그 확률을 다른 스텟과 조율하여 민원 반환
        Complain occurComplain = new Complain();
        //분류된 민원을 가지고, 새로운 민원으로 복사해서 반환 
        if(isWarning == false)
        {
            Debug.LogWarning("임시로 불만 제로");
            isWarning = true;
        }
        
        return null;
    }
}

