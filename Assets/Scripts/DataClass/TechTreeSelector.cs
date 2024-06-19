using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TechTreeSelector
{
    //국가에서 테크 발전 계획을 수립하기 위해 우선도에 따라 남은 계획을 정렬하여 반환하는곳
    private List<NationTechTree> techTree;

    public TechTreeSelector()
    {
        techTree = MgMasterData.GetInstance().GetTechDic().Select(dic => dic.Value).ToList(); //딕셔너리의 nationTech만 가지고 리스트로 전환
        
    }

    public int GetTechPidByNotDone(List<int> _doneTech)
    {
        for (int i = 0; i < techTree.Count; i++)
        {
            int techPid = techTree[i].GetPid();
            
            //배웠던거라면 넘기고
            if (_doneTech.IndexOf(techPid) >= 0)
                continue;

            //아니라면
            return techPid; //해당 기술의 pid를 반환
        }

        return FixedValue.No_INDEX_NUMBER; //배울게 없으면 없는 인덱스 반환
    }

    public int GetTechPidByClass(int _class, List<int> _doneTech)
    {
        for (int i = 0; i < techTree.Count; i++)
        {
            int treeClass = techTree[i].GetTechValue(TechTreeStat.Class);
            //다른 클래스면 넘기고
            if (treeClass.Equals(_class) == false)
                continue;
            //배웠던거라면 넘기고
            if (_doneTech.IndexOf(treeClass) >= 0)
                continue;

            //아니라면
            return techTree[i].GetPid(); //해당 기술의 pid를 반환
        }

        return FixedValue.No_INDEX_NUMBER; //배울게 없으면 없는 인덱스 반환
    }
}
