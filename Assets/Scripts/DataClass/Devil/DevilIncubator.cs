using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum DevilState
{
    봉인, 유아, 성인, 토벌
}

public class DevilIncubator
{
    private List<int> m_devilPidList = new List<int>();
    private List<int> m_chunkNum = new();
    private List<TokenTile> m_birthTile = new();
    private List<bool> m_contentMeet = new();
    private List<DevilState> m_devilStateList = new();
    private int m_firstTerm = 30; //카운터 최초 시작 주기
    private int m_baseActiveTerm = 30; //활동 시간 - 해당 시간 동안 발생, 봉인, 전투등이 진행
    private int m_nextTerm = 20;  //악마의 활동이나 토벌, 봉인등이 종료된 후 다음 활동기까지 준비 시간
    private int m_birthRestTurn = 0; //부활까지 남은 시간 -- 턴 조건
    private bool m_turnEnough = true;
    private int birthCount = 0; //부활시킨 수

    private enum BirthCountType
    {
      First,  Birth, Victory
    }

    #region 초기화
    public DevilIncubator()
    {
        SetNextBirthTurn(BirthCountType.First); //첫 탄생 주기로 남은 부활턴 설정
    }

    public void SetBirthRegion(List<int> _chunkList, List<TokenTile> _tileList)
    {
        m_chunkNum = _chunkList;
        m_birthTile = _tileList;
    }

    public void DiceDevilList(int _diceCount)
    {
        //등장할 악마를 골라서 생성 

        //1. 캐릭 마스터 데이터에서 악마 데이터만 추출하여 복사
        Dictionary<int,TokenChar> charDic = MgMasterData.GetInstance().GetCharDic();
        List<int> devilPid = new List<int>();
        foreach(KeyValuePair<int, TokenChar> tokenChar in charDic)
        {
            if (tokenChar.Value.GetCharType().Equals(CharType.Devil))
            {
                devilPid.Add(tokenChar.Key); //pid만 추출
            }
        }

        //2. 등장 악마 랜덤 뽑기
        List<int> randomDevilIdx = GameUtil.GetRandomNum(devilPid.Count, _diceCount);

        //3. 뽑힌 악마 pid로 tokenObj 생성해서 추가
        for (int i = 0; i < randomDevilIdx.Count; i++)
        {
            //악마 pid로 리스트 추가 
            m_devilPidList.Add(devilPid[randomDevilIdx[i]]);
            m_contentMeet.Add(false); //조건 만족 false로 초기화
            m_devilStateList.Add(DevilState.봉인);
            //Debug.Log(m_devilList[i].GetItemName() + "악마 확정");
        }
    }
    #endregion

    #region 턴에 따른 작업
    public void ChangeWorldContent(int _pid, bool _result)
    {
        //해당 퀘스트 컨텐츠 결과값이 들어왔을 때 영향을 받는 악마에게 조건 적용
    }

    public void ChangeWorldTurn(int _currentTurn)
    {
        CountBirthRestTurn();
    }

    private void CountBirthRestTurn()
    {
        //카운터가 진행중이였으면 카운터하고 탄생 체크
        if(0 < m_birthRestTurn)
        {
            m_birthRestTurn -= 1;

            if (m_birthRestTurn == 0)
                m_turnEnough = true;
        }
    }
    #endregion

    #region 악마 탄생, 토벌
    public void BirthDevil()
    {
        Debug.LogWarning("악마 부활 조건 체크 후 부활진행");
        //전체적인 턴 조건 확인
        if (m_turnEnough == false)
            return;

        if(birthCount == m_devilPidList.Count)
        {
            Debug.Log("악마 모두 부활");
            return;
        }

        //악마 수대로 랜덤수 뽑기 - 기존에 뽑은 애는 넘겨야 하므로 
        List<int> randomOrder = GameUtil.GetRandomNum(m_devilPidList.Count, m_devilPidList.Count);

        //이미만들어져있는 애를 엠지토큰 스폰으로 장소에 호출, 및 mgtoken의 charList에 추가 
        for (int i = 0; i < randomOrder.Count; i++)
        {
            int index = randomOrder[i]; //뽑을 악마 인덱스
            //이미 뽑은 애는 차례 넘김
            if (m_devilStateList[index] != DevilState.봉인)
                continue;

            //개별 컨텐츠 조건 확인
            //블라블라 블라라


            //부활이 확정되었으면, 턴 조건은 갱신
            m_turnEnough = false;
            SetNextBirthTurn(BirthCountType.Birth); //다음 부활주기 재세팅

            TokenChar spawnDevil = MgToken.GetInstance().SpawnCharactor(m_birthTile[index].GetMapIndex(), m_devilPidList[index]);
            m_devilStateList[index] = DevilState.유아;

            TokenTile targetNation = MgNation.GetInstance().GetNation(0).GetCapital();
            spawnDevil.SetTargetTile(targetNation);
           // Debug.Log(targetNation.GetItemName() + "을 악마 타겟으로 설정");

            birthCount += 1;
            Debug.Log(MgMasterData.GetInstance().GetCharData(m_devilPidList[index]).GetItemName() + "악마 부활");
            //뽑았으면 종료
            break;
        }
      
    }

    //악마 토벌시 호출
    public void RemoveDevil()
    {
        SetNextBirthTurn(BirthCountType.Victory); 
    }

    //다음 발생주기 턴 정하는 부분
    private void SetNextBirthTurn(BirthCountType _birthType)
    {
        switch (_birthType)
        {
            //기존 악마 토벌이나 봉인으로 승리했을땐, 남은 주기의 3분의1에 다음 준비기간을 추가
            case BirthCountType.Victory:
                m_birthRestTurn = m_birthRestTurn / 3 + m_nextTerm;
                break;
            //악마가 탄생했을때는 해당 악마 활동 주기 만큼 다음 악마 도래까지 설정
            case BirthCountType.Birth:
                m_birthRestTurn = m_baseActiveTerm;
                break;
            case BirthCountType.First:
                m_birthRestTurn = m_firstTerm;
                break;
        }

    }

    public void SetRestBrithTurn(int _turn)
    {
        m_birthRestTurn = _turn;
    }
    #endregion


}

