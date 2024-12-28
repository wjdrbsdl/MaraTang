using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public enum DevilState
{
    봉인, 박동, 유아, 성인, 토벌
}

public class DevilIncubator
{
    [JsonProperty] private List<int> m_devilPidList = new List<int>();
    private List<int> m_chunkNumList = new();
    [JsonProperty] private List<int[]> m_birthTileList = new();
    [JsonProperty] private List<bool> m_contentMeet = new();
    [JsonProperty] private List<DevilState> m_devilStateList = new();
    private int m_firstTerm = 30; //카운터 최초 시작 주기
    private int m_baseActiveTerm = 30; //활동 시간 - 해당 시간 동안 발생, 봉인, 전투등이 진행
    private int m_nextTerm = 20;  //악마의 활동이나 토벌, 봉인등이 종료된 후 다음 활동기까지 준비 시간
    [JsonProperty] private int m_birthRestTurn = 0; //부활까지 남은 시간 -- 턴 조건
    [JsonProperty] private bool m_turnEnough = true;
    [JsonProperty] private int birthCount = 0; //부활시킨 수

    private enum BirthCountType
    {
      First,  Birth, Victory
    }

    #region 초기화 및 초기 설정
    public DevilIncubator()
    {
        SetNextBirthTurn(BirthCountType.First); //첫 탄생 주기로 남은 부활턴 설정
    }

    public void SetBirthRegion(List<int> _chunkList, List<int[]> _tileListMapIndex)
    {
        m_chunkNumList = _chunkList;
        m_birthTileList = _tileListMapIndex;
        for (int i = 0; i < m_devilPidList.Count; i++)
        {

            TileType devilPlace = GetDevilPlace(m_devilPidList[i]); //해당 악마가 탄생할 해처리 타입을 찾기
            TokenTile hatcheryTile = GameUtil.GetTileTokenFromMap(_tileListMapIndex[i]);
            hatcheryTile.ChangePlace(devilPlace);
        }
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

    private TileType GetDevilPlace(int _pid)
    {
        //어떤 악마인지 빼서
        //6   1번폭력
        //7   2번나태
        //8   3번기만
        //9   4번역병
        //10  5번공포
        //11  6번부패
        //12  7번타락
        switch (_pid)
        {
            case 6:
                return TileType.Hatchery폭력;
            case 7:
                return TileType.Hatchery나태;
            case 8:
                return TileType.Hatchery기만;
            case 9:
                return TileType.Hatchery역병;
            case 10:
                return TileType.Hatchery공포;
            case 11:
                return TileType.Hatchery부패;
            case 12:
                return TileType.Hatchery타락;
        }
        return TileType.Nomal;
    }
    #endregion

    #region 월드 변화 적용
    public void ChangeWorldContent(int _pid, bool _result)
    {
        //해당 퀘스트 컨텐츠 결과값이 들어왔을 때 영향을 받는 악마에게 조건 적용
    }

    public void ChangeWorldTurn()
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

    #region 악마 탄생
    public void BirthDevil()
    {
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

            ResetBirthTime();
            m_devilStateList[index] = DevilState.박동;
            birthCount += 1;
            //악마 봉인지에 악마 부활 작업을 시작하는거 
            TokenTile hatchery = GameUtil.GetTileTokenFromMap(m_birthTileList[index]);
            //해당 해처리 준비완료로 돌리고
            Debug.LogFormat("{0} 해처리에서 부활 작업 진행", hatchery.GetTileType());
            hatchery.ReadyInherenceWork(); //부화작업 진행 
            
            return;
        }
      
    }

    private void ResetBirthTime()
    {
        m_turnEnough = false;
        SetNextBirthTurn(BirthCountType.Birth); //다음 부활주기 재세팅
    }

    //악마봉인지서 부활했을때 전달받아야하는곳. 
    public void SetDevilInfo(TokenChar _devil)
    {
        for (int i = 0; i < m_devilPidList.Count; i++)
        {
           if(_devil.GetPid() == m_devilPidList[i])
            {
                m_devilStateList[i] = DevilState.유아;
                break;
            }
        }

        TokenTile targetNation = MgNation.GetInstance().GetNation(0).GetCapital();
        _devil.SetTargetTile(targetNation);
    }
    #endregion

    #region 악마 부활 시기
    //다음 발생주기 턴 정하는 부분
    private void SetNextBirthTurn(BirthCountType _birthType)
    {
        int restTurn = int.MaxValue;
        switch (_birthType)
        {
            //기존 악마 토벌이나 봉인으로 승리했을땐, 남은 주기의 3분의1에 다음 준비기간을 추가
            case BirthCountType.Victory:
                restTurn = m_birthRestTurn / 3 + m_nextTerm;
                break;
            //악마가 탄생했을때는 해당 악마 활동 주기 만큼 다음 악마 도래까지 설정
            case BirthCountType.Birth:
                restTurn = m_baseActiveTerm;
                break;
            case BirthCountType.First:
                restTurn = m_firstTerm;
                break;
        }
        SetRestBrithTurn(restTurn);
    }

    public void SetRestBrithTurn(int _turn)
    {
        m_birthRestTurn = _turn;
    }
    #endregion

    //악마 토벌시 호출
    public void AllKillDevil()
    {
        SetNextBirthTurn(BirthCountType.Victory); 
    }



}

