﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ComplainManager : Mg<ComplainManager>
{
    public List<Complain> m_complinLIst; //민원 종류들

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

    public void ClassfyComplain()
    {
        //마스터데이터로 부터 민원 종류들 가져와서 분류 

    }

   public Complain OccurComplain(TokenTile _tile)
    {
        Nation nation = _tile.GetNation();
        //해당 장소에서 발생가능한 민원과 그 확률을 다른 스텟과 조율하여 민원 반환
        Complain occurComplain = null;
        //분류된 민원을 가지고, 새로운 민원으로 복사해서 반환 

        return occurComplain;
    }
}

