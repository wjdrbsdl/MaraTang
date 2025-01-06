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

    //국가가 전달 되면 해당 국가에서 발생할 민원을 설정
    public void OccurComplain(Nation _nation)
    {
        if (isDebugWarning == false)
        {
            Debug.LogWarning("임시로 불만 제로");
            isDebugWarning = true;
        }

        
        //1. 민원 발생 수 - 일정 비율에 토지 수를 곱해서 결정 - 일정비율은 변화가능 분위기좋을땐 높은게 좋고, 나쁠땐 낮은게 좋고
        int occurCount = CalComplainCount(_nation);
        //2. 발생 민원 결정
        List<Complain> occurComplainList = FindComplainList(_nation, occurCount);
        //3. 민원 영토에 배정
        AssignComplain(_nation, occurComplainList);
       
    }

    private int CalComplainCount(Nation _nation)
    {
        int territoryCount = _nation.GetTerritorry().Count-1 ;//수도 제거 
        int tempOccurRatio = 30;
        decimal accurateRatio = tempOccurRatio * 0.01m;
        int occurCount = (int)(territoryCount * accurateRatio);
        Debug.Log("정밀 비율" + accurateRatio + " 발생 수치" + occurCount);
        return occurCount;
    }

    private List<Complain> FindComplainList(Nation nation, int _occurCount)
    {
        //민원 발생에 관련될 스텟을 정의
        List<Complain> complainList = new();
        for (int i = 1; i <= _occurCount; i++)
        {
            //민원 발생수만큼 민원을 발생
            Complain newComplain = new Complain();
            complainList.Add(newComplain); //
        }
        return complainList;
    }

    private void AssignComplain(Nation _nation, List<Complain> _complaintList)
    {
        
        List<TokenTile> territory = _nation.GetTerritorry();
        //민원을 국가 영지에 적절하게 배분할것. 
        for (int complainIdx = 0; complainIdx < _complaintList.Count; complainIdx++)
        {
            Complain complaint = _complaintList[complainIdx];
            for (int i = 1; i < territory.Count; i++) //수도는 제외 1 부터 index
            {
                TokenTile tile = territory[i];
                if (tile.HaveComplain())
                {
                    continue;
                }
                complaint.AssignTile(tile);
            }
        }
        
    }
}

