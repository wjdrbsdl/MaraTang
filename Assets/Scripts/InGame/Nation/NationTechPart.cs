using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NationTechPart 
{
    private List<int> m_doneTech; // 완료한 테크 Pid
    private List<TOrderItem> m_nationStatEffectList;
    public NationTechPart()
    {
        m_doneTech = new();
        m_nationStatEffectList = new();
    }

    public void CompleteTech(int _techPid)
    {
        if (IsDoneTech(_techPid) == false) //배우지 않은 녀석이면
            m_doneTech.Add(_techPid);
    }

    public bool IsDoneTech(int _techPid)
    {
        if (m_doneTech.IndexOf(_techPid) >= 0)
            return true;

        return false;
    }

    public List<int> GetTechList()
    {
        return m_doneTech;
    }

    public void CalTechEffect()
    {
        //테크 효과 torderItem 으로 모아놓은곳. 
        int[] nationStatValues = GameUtil.EnumLengthArray(typeof(NationStatEnum));
        for (int i = 0; i < m_doneTech.Count; i++)
        {
            NationTechData techData = MgMasterData.GetInstance().GetTechData(m_doneTech[i]); //테크pid로 마스터 데이터 가져옴
            List<TOrderItem> effectList = techData.TechEffectData.GetItemList(); //각 테크의 효과 데이터를 가져옴
            for (int effectIndex = 0; effectIndex < effectList.Count; effectIndex++)
            {
                TOrderItem effect = effectList[effectIndex];
                TokenType effectType = effect.Tokentype; //효과 타입으로 나누고

                //각 효과 타입에 맞게 값을 모을 배열에 값을 추가 
                switch (effectType)
                {
                    case TokenType.NationStat:
                        int subIndex = effect.SubIdx; //nationStat에서 index
                        nationStatValues[subIndex] += effect.Value;
                        break;

                }
            }
        }
        CalNationStatPart(nationStatValues);
    }

    private void CalNationStatPart(int[] _nationStatValues)
    {
        //벨류 배열을 가지고 다시 TOrderItem List로 반환
        TItemListData makeItemListData = new TItemListData(_nationStatValues, TokenType.NationStat);
        m_nationStatEffectList = makeItemListData.GetItemList();

        for (int i = 0; i < m_nationStatEffectList.Count; i++)
        {
           // Debug.LogFormat("{0}번 타입의 {1}번 류를 {2}만큼 상승", m_nationStatEffectList[i].Tokentype, m_nationStatEffectList[i].SubIdx, m_nationStatEffectList[i].Value);
        }
    }


}
